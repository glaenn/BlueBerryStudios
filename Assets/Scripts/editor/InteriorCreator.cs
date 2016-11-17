using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class InteriorCreator : EditorWindow
{
    struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Serializable]
    public struct MapData
    {
        public bool[,] floorData;
        public int[,] wallData;
        public int[,] detailData;
        public int[,] materialData;

        public MapData(int sizeX, int sizeY)
        {
            floorData = new bool[sizeX, sizeY];
            wallData = new int[sizeX, sizeY];
            detailData = new int[sizeX, sizeY];
            materialData = new int[sizeX, sizeY];   
        }
    }

    static int MIN_VIEW_SCALE = 2;
    static int MAX_VIEW_SCALE = 16;
    static int MAX_MAP_SIZE_X = 256;
    static int MAX_MAP_SIZE_Y = 256;
    static Rect FILE_MENY_AREA = new Rect(0.0f,0.0f,768.0f,32.0f);
    static Rect TOOL_MENY_AREA = new Rect(0, FILE_MENY_AREA.yMax, 128, 512);
    static Rect VIEW_AREA = new Rect(TOOL_MENY_AREA.xMax, FILE_MENY_AREA.yMax, 512, 512);
    static Rect INFO_AREA = new Rect(VIEW_AREA.xMax, FILE_MENY_AREA.yMax, 128, 512);
    static Rect VIEWTOOL_AREA = new Rect(0, VIEW_AREA.yMax, 768, 64);
   
    MapData mapData = new MapData(MAX_MAP_SIZE_X, MAX_MAP_SIZE_Y);

    int viewScale = 1;
    bool showGrid = false;
    Vector2 scrollPos;

    bool showSelector = true;
    IntVector2 mapSelectPos;

    public string[] editModeName = new string[] { "Paint Floors", " Floor Details", "Wall Details", "Paint Materials" };
    int editMode;

    public string[] paintToolNames = new string[] { "Pencil"," Area", "Bucket" };
    int paintTool;

    public string[] detailFloorToolNames = new string[] { "Delete", " Hole"};
    int detailFloorTool;

    public string[] detailWallToolNames = new string[] { "Delete", " Window", "Door" };
    int detailWallTool;

    public string[] materialsNames = new string[] { "0", " 1", "2","3",",4","5"};
    int materials;

    // Area Tool
    bool isSelectingArea;
    Rect areaToolRect;
    int xCoordBegin, yCoordBegin;

    GUIStyle tealBG = new GUIStyle();
    GUIStyle greenBG = new GUIStyle();
    GUIStyle redBG = new GUIStyle();

    Stack undoData = new Stack();
   
    //Textures for the editor
    Texture2D gridMap, tileSelector, mapTiles, levelMap;

    [MenuItem("MyTools/Interior Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(InteriorCreator));
    }
    void Awake()
    {
        tealBG.normal.background = CreateColorTexture(1, 1, new Color(0.0f, 0.8f, 1.0f, 0.1f));
        greenBG.normal.background = CreateColorTexture(1, 1, new Color(0.1f, 1.0f, 0.1f, 0.1f));
        redBG.normal.background = CreateColorTexture(1, 1, new Color(1.0f, 0.1f, 0.1f, 0.1f));

        levelMap = new Texture2D(MAX_MAP_SIZE_Y * 4, MAX_MAP_SIZE_X * 4);
        levelMap.filterMode = FilterMode.Point;

        gridMap = new Texture2D(MAX_MAP_SIZE_Y * 8, MAX_MAP_SIZE_X * 8);
        gridMap.filterMode = FilterMode.Point;

        tileSelector = Resources.Load<Texture2D>("editorGraphics/tileSelector");
        mapTiles = Resources.Load<Texture2D>("editorGraphics/roadTiles");
        LoadGridMap();

        UpdateMapPreview(new Rect(0, 0, MAX_MAP_SIZE_Y-1, MAX_MAP_SIZE_X-1), mapData);   
    }
    void OnGUI()
    {
        wantsMouseMove = true;
        //////////////////////////////////////////
        //FileMenyMeny Area///////////////////////
        //////////////////////////////////////////
        GUILayout.BeginArea(FILE_MENY_AREA, tealBG);
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUILayout.Space(8);

        if (GUILayout.Button("Save to File", GUILayout.Width(128)))
            SaveToFile();

        else if (GUILayout.Button("Load File", GUILayout.Width(128)))
            LoadFile();

        else if (GUILayout.Button("Export to Mesh", GUILayout.Width(128)))
            ExportToMesh();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
 
        //////////////////////////////////////////
        //ToolMenyMeny Area///////////////////////
        //////////////////////////////////////////
        GUILayout.BeginArea(TOOL_MENY_AREA, greenBG);
        GUILayout.Space(30);
        GUILayout.Space(5);
        GUILayout.Label("Edit Mode");
        editMode = GUILayout.SelectionGrid(editMode, editModeName, 1, GUILayout.MaxWidth(96));
        GUILayout.Space(30);
        GUILayout.Label(editModeName[editMode]);
        GUILayout.Space(5);
        if(editMode == 0 || editMode == 3)
            paintTool = GUILayout.SelectionGrid(paintTool, paintToolNames, 1, GUILayout.MaxWidth(96));
        else if (editMode == 1)
            detailFloorTool = GUILayout.SelectionGrid(detailFloorTool, detailFloorToolNames, 1, GUILayout.MaxWidth(96));
        else if (editMode == 2)
            detailWallTool = GUILayout.SelectionGrid(detailWallTool, detailWallToolNames, 1, GUILayout.MaxWidth(96));
        if (editMode == 3)
        {
            GUILayout.Space(30);
            GUILayout.Label("Material List");
            GUILayout.Space(5);
            materials = GUILayout.SelectionGrid(materials, materialsNames, 1, GUILayout.MaxWidth(96));
        }
        GUILayout.EndArea();

        //////////////////////////////////////////
        //Info Area////////////////////////////////
        //////////////////////////////////////////
        GUILayout.BeginArea(INFO_AREA, greenBG);
        GUILayout.Space(10);
        GUILayout.EndArea();

        //////////////////////////////////////////
        //View Menu Area////////////////////////////////
        //////////////////////////////////////////
        GUILayout.BeginArea(VIEWTOOL_AREA, greenBG);
        GUILayout.Space(10);
        viewScale = EditorGUILayout.IntSlider("Zoom", viewScale, MIN_VIEW_SCALE, MAX_VIEW_SCALE, GUILayout.MaxWidth(512));
        GUILayout.Space(10);
        showGrid = EditorGUILayout.Toggle("Show Grid:*", showGrid);
        GUILayout.EndArea();

        //////////////////////////////////////////
        //ViewArea////////////////////////////////
        //////////////////////////////////////////
        scrollPos = GUI.BeginScrollView(VIEW_AREA, scrollPos, new Rect(0, 0, (MAX_MAP_SIZE_X)*viewScale, (MAX_MAP_SIZE_Y )*viewScale), false, false);
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        EditorGUI.DrawTextureTransparent(new Rect(0, 0, MAX_MAP_SIZE_X * viewScale, MAX_MAP_SIZE_Y * viewScale), levelMap);

        if (showGrid)
            EditorGUI.DrawTextureTransparent(new Rect(0, 0, (MAX_MAP_SIZE_X) * viewScale, (MAX_MAP_SIZE_Y) * viewScale), gridMap);

        if (showSelector)
        {
            if(isSelectingArea)
                EditorGUI.DrawTextureTransparent(new Rect(areaToolRect.x *viewScale, areaToolRect.y * viewScale, (areaToolRect.width+1) * viewScale, (areaToolRect.height+1) * viewScale), tileSelector);
            else
                EditorGUI.DrawTextureTransparent(new Rect(mapSelectPos.x*viewScale, mapSelectPos.y*viewScale, viewScale, viewScale), tileSelector);
        }

         GUI.EndScrollView();

        ///////////////////////////////////////////
        //Évents //////////////////////////////////
        //////////////////////////////////////////
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Z)
        {
            Undo();
        }

        //is within the map field
        if (e.mousePosition.x > VIEW_AREA.xMin && e.mousePosition.y > VIEW_AREA.yMin && e.mousePosition.x < VIEW_AREA.xMax && e.mousePosition.y < VIEW_AREA.yMax)
        {
            mapSelectPos.Set(((int)e.mousePosition.x - (int)VIEW_AREA.xMin + (int)scrollPos.x) / viewScale, ((int)e.mousePosition.y - (int)VIEW_AREA.yMin + (int)scrollPos.y) / viewScale);

            if (editMode == 0)
            {
                // Pencil Mode
                if (paintTool == 0)
                {
                    if (e.type == EventType.MouseDown || (e.type == EventType.MouseDrag))
                    {
                        if (e.button == 0)
                            PaintMapDataArea(new Rect(mapSelectPos.x, mapSelectPos.y, 0, 0),ref mapData, true);
                        else if (e.button == 1)
                            PaintMapDataArea(new Rect(mapSelectPos.x, mapSelectPos.y, 0, 0),ref mapData, false);
                    }
                }
                //Area Mode
                else if (paintTool == 1)
                {
                    if (isSelectingArea)
                    {
                        areaToolRect.x = Mathf.Min(xCoordBegin, mapSelectPos.x);
                        areaToolRect.y = Mathf.Min(yCoordBegin, mapSelectPos.y);
                        areaToolRect.width = (Mathf.Max(xCoordBegin, mapSelectPos.x) - Mathf.Min(xCoordBegin, mapSelectPos.x));
                        areaToolRect.height = (Mathf.Max(yCoordBegin, mapSelectPos.y) - Mathf.Min(yCoordBegin, mapSelectPos.y));
                    }

                    if (e.type == EventType.MouseDown && !isSelectingArea)
                    {
                        if (e.button == 0 || e.button == 1)
                        {
                            isSelectingArea = true;
                            xCoordBegin = mapSelectPos.x;
                            yCoordBegin = mapSelectPos.y;
                        }
                    }

                    else if (e.type == EventType.MouseUp)
                    {
                        if (e.button == 0 && isSelectingArea)
                        {
                            isSelectingArea = false;
                            PaintMapDataArea(areaToolRect, ref mapData, true);
                        }
                        else if (e.button == 1 && isSelectingArea)
                        {
                            isSelectingArea = false;
                            PaintMapDataArea(areaToolRect, ref mapData, false);
                        }
                    }
                }
                //PaintBucketMode Mode
                else if (paintTool == 2)
                {
                    if (e.type == EventType.MouseDown || (e.type == EventType.MouseDrag))
                    {
                        if (e.button == 0)
                            PaintMapDataFill(mapSelectPos.x, mapSelectPos.y, ref mapData.floorData, true);
                        else if (e.button == 1)
                            PaintMapDataFill(mapSelectPos.x, mapSelectPos.y, ref mapData.floorData, false);
                    }
                }
            }

            if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
                showSelector = true;

            Repaint();
        }
        else
        {
            showSelector = false;
            isSelectingArea = false;
        }
    }
    Texture2D CreateColorTexture(int width, int height, Color col)
    {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
      }
    void LoadGridMap()
    {
        Texture2D gridTile = Resources.Load<Texture2D>("editorGraphics/gridTile");

        for (int i = 0; i < gridMap.width / gridTile.width; i++)
            for (int j = 0; j < gridMap.width / gridTile.width; j++)
                gridMap.SetPixels(i * gridTile.width, j * gridTile.width, gridTile.width, gridTile.width, gridTile.GetPixels());

        gridMap.Apply();
    }
    void UpdateMapPreview(Rect area, MapData data)
    {
        for (int i = (int)area.x; i < (int)area.x + (int)area.width + 1; i++)
            for (int j = (int)area.y; j < (int)area.y + (int)area.height + 1; j++)
            {  
                if (data.floorData[i, j])
                    levelMap.SetPixels(i * 4, (MAX_MAP_SIZE_Y - j - 1) * 4, 4, 4, mapTiles.GetPixels(4, 0, 4, 4));
                else
                    levelMap.SetPixels(i * 4, (MAX_MAP_SIZE_Y - j - 1) * 4, 4, 4, mapTiles.GetPixels(0, 0, 4, 4));
            }
        UpdateWalls();
        levelMap.Apply();
    }
    void PaintMapDataArea(Rect area, ref MapData data, bool isAdding = true)
    {
        bool hasChanged = false;

        for (int i = (int)area.x; i < (int)area.x + (int)area.width + 1; i++)
            for (int j = (int)area.y; j < (int)area.y + (int)area.height + 1; j++)
                if (i > 0 && j > 0 && i < MAX_MAP_SIZE_X && j < MAX_MAP_SIZE_Y)
                    if (data.floorData[i, j] != isAdding)
                    {
                        if (!hasChanged)
                        {
                            hasChanged = true;
                            undoData.Push(data.floorData.Clone());
                        }
                        data.floorData[i, j] = isAdding;
                    }
  
        if (hasChanged == true)
            UpdateMapPreview(area,data);

    }
    void PaintMapDataFill(int i, int j, ref bool[,] data, bool isAdding = true)
    { 
        if (data[i, j] != isAdding) //Is this not a floor?
        {
            undoData.Push(data.Clone());
            Stack jobQuery = new Stack();
            jobQuery.Push(new IntVector2(i,j));
            IntVector2 currentCheckPos;
            bool reachLeft, reachRight;

            while(jobQuery.Count > 0)
            {
                currentCheckPos = (IntVector2)jobQuery.Pop();

                //Find highest point
                while (currentCheckPos.y > 1 && data[currentCheckPos.x, currentCheckPos.y-1] != isAdding)
                {
                    currentCheckPos.y--;
                }

                while (currentCheckPos.y < MAX_MAP_SIZE_Y-1 && data[currentCheckPos.x, currentCheckPos.y] != isAdding)
                {
                    reachLeft = false;
                    reachRight = false;

                    if ((currentCheckPos.x - 1) > 0 && data[(currentCheckPos.x - 1), currentCheckPos.y] != isAdding && reachLeft == false)
                    {
                        jobQuery.Push(new IntVector2(currentCheckPos.x - 1, currentCheckPos.y));
                        reachLeft = true;
                    }
                    else
                    {
                        reachRight = false;
                    }

                    if ((currentCheckPos.x + 1) < MAX_MAP_SIZE_X - 1 && data[(currentCheckPos.x + 1), currentCheckPos.y] != isAdding && reachRight == false)
                    {
                        jobQuery.Push(new IntVector2(currentCheckPos.x + 1, currentCheckPos.y));
                    }
                    else
                    {
                        reachLeft = false;
                    }

                    data[currentCheckPos.x, currentCheckPos.y] = isAdding;

                    if(data[currentCheckPos.x, currentCheckPos.y])
                        levelMap.SetPixels(currentCheckPos.x * 4, (MAX_MAP_SIZE_Y - currentCheckPos.y - 1) * 4, 4, 4, mapTiles.GetPixels(0, 0, 4, 4));
                    else if (!data[currentCheckPos.x, currentCheckPos.y])
                        levelMap.SetPixels(currentCheckPos.x * 4, (MAX_MAP_SIZE_Y - currentCheckPos.y - 1) * 4, 4, 4, mapTiles.GetPixels(4, 4, 4, 4));

                    currentCheckPos.y++;
                }
            }

            UpdateWalls();
            levelMap.Apply();
        }
    }
    void UpdateWalls()
    {
        bool paintWall;
        int wallValue;

        for (int i = 0; i < MAX_MAP_SIZE_X; i++)
            for (int j = 0; j < MAX_MAP_SIZE_Y; j++)
            {
                paintWall = false;
                wallValue = 0;
                if (mapData.floorData[i, j] == false) //Not a floor
                {
                    for (int l = -1; l < 2; l++)
                    {
                        for (int k = -1; k < 2; k++)
                        {
                            if ((i + l) >= 0 && (j + k) >= 0 && (i + l) < MAX_MAP_SIZE_X && (j + k) < MAX_MAP_SIZE_Y)
                            {
                                if (mapData.floorData[i + l, j + k] == true) //is floor
                                {
                                    paintWall = true;

                                    if (l == -1 && k == 0)
                                        wallValue += 1;
                                    else if (l == 0 && k == -1)
                                        wallValue += 2;
                                    else if (l == 0 && k == 1)
                                        wallValue += 4;
                                    else if (l == 1 && k == 0)
                                        wallValue += 8;
                                }
                            }
                        }
                    }

                    if (paintWall)
                    {
                        mapData.wallData[i, j] = 3 + wallValue;
                        levelMap.SetPixels(i * 4, (MAX_MAP_SIZE_Y - j - 1) * 4, 4, 4, mapTiles.GetPixels((mapData.wallData[i, j]%8)*4, (mapData.wallData[i, j] / 8) * 4, 4, 4));
                    }
                    else if (!paintWall)
                    {
                        mapData.wallData[i, j] = 0;
                        levelMap.SetPixels(i * 4, (MAX_MAP_SIZE_Y - j - 1) * 4, 4, 4, mapTiles.GetPixels(0, 0, 4, 4));
                    }
                }
            }
    }
    void Undo()
    {
        if (undoData.Count > 0)
        {
            mapData.floorData = (bool[,])undoData.Pop();
            UpdateMapPreview(new Rect(0, 0, MAX_MAP_SIZE_Y - 1, MAX_MAP_SIZE_X - 1), mapData);
        }
    }
    void SaveToFile()
    {
        string filePath = EditorUtility.SaveFilePanelInProject("Save Map", "newMap.map", "map",
                                    "Please enter a file name to save the interior to");
        if (filePath.Length != 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(filePath);
            bf.Serialize(file, mapData);
            file.Close();
        }
    }
    void LoadFile()
    {
        string filePath = EditorUtility.OpenFilePanel("Load Map", "newMap.map", "map");

        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            mapData = (MapData)bf.Deserialize(file);
            file.Close();
            undoData = new Stack(); //Empty the undo data
            UpdateMapPreview(new Rect(0, 0, MAX_MAP_SIZE_X-1, MAX_MAP_SIZE_Y-1), mapData);
        }

    }
    void ExportToMesh()
    {
        //string meshData = "";
        Mesh mesh = new Mesh();
        Queue vertQueue = new Queue();
        Queue uvQueve = new Queue();

        //Center all the points
        int interiorStartX = MAX_MAP_SIZE_X;
        int interiorEndX = 0;
        int interiorStartY = MAX_MAP_SIZE_Y;
        int interiorEndY = 0;

        for (int i = 0; i < MAX_MAP_SIZE_X; i++)
            for (int j = 0; j < MAX_MAP_SIZE_Y; j++)
            {
                if (mapData.floorData[i, j] == true)
                {
                    if (i < interiorStartX)
                        interiorStartX = i;
                    if (i > interiorEndX)
                        interiorEndX = i;

                    if (j < interiorStartY)
                        interiorStartY = j;
                    if (j > interiorEndY)
                        interiorEndY = j;
                }
            }

        bool[,] ignoreList = new bool[MAX_MAP_SIZE_X,MAX_MAP_SIZE_Y];

        //Floors
        for (int i = 0; i < MAX_MAP_SIZE_X; i++)
        {
            for (int j = 0; j < MAX_MAP_SIZE_Y; j++)
            { 
                if (mapData.floorData[i, j] == true && !ignoreList[i, j])
                {
                    int addI = 0;
                    int addJ = 0;

                    vertQueue.Enqueue(new Vector3(i - (interiorEndX + interiorStartX) / 2, 0, j - (interiorEndY + interiorStartY) / 2));
                    uvQueve.Enqueue(new Vector2(0, 0));

                    while (true)
                    {
                        addJ++;
                        if (j + addJ >= MAX_MAP_SIZE_Y)
                            break;
                        if (mapData.floorData[i, j + addJ] != true || ignoreList[i, j + addJ] == true)
                            break;
                    }

                    vertQueue.Enqueue(new Vector3(i - (interiorEndX + interiorStartX) / 2, 0, (j + addJ) - (interiorEndY + interiorStartY) / 2));
                    uvQueve.Enqueue(new Vector2(0, 1 * addJ));

                    while (true)
                    {
                        bool bust = false;
                        addI++;

                        if (mapData.floorData[i + addI, j - 1] == true && !ignoreList[i + addI, +j - 1])
                            break;


                        if (i + addI >= MAX_MAP_SIZE_X)
                            break;
                        for (int k = j; k < j + addJ; k++)
                            if (mapData.floorData[i + addI, k] != true || ignoreList[i + addI, k] == true)
                                bust = true;
                        if (bust)
                            break;
                    }

                    vertQueue.Enqueue(new Vector3((i + addI) - (interiorEndX + interiorStartX) / 2, 0, j - (interiorEndY + interiorStartY) / 2));
                    uvQueve.Enqueue(new Vector2(1 * addI, 0));

                    vertQueue.Enqueue(new Vector3((i + addI) - (interiorEndX + interiorStartX) / 2, 0, (j + addJ) - (interiorEndY + interiorStartY) / 2));
                    uvQueve.Enqueue(new Vector2(1 * addI, 1 * addJ));

                    for (int k = i; k < i + addI; k++)
                        for (int l = j; l < j + addJ; l++)
                            ignoreList[k, l] = true;
                }
            }
        }

        mesh = AddToMesh(mesh, ref vertQueue, ref uvQueve);

        //Walls
        for (int i = 0; i < MAX_MAP_SIZE_X; i++)
        {
            for (int j = 0; j < MAX_MAP_SIZE_Y; j++)
            {
                if (mapData.floorData[i, j] == false && mapData.wallData[i, j] > 0)
                {
                    //Top of walls
                    /*
                    for (int l = 0; l < 2; l++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            vertQueue.Enqueue(new Vector3((i + l) - (interiorEndX + interiorStartX) / 2, 2, (j + k) - (interiorEndY + interiorStartY) / 2));
                            uvQueve.Enqueue(new Vector2(0 + l, 0 + k));
                        }
                    }
                    */

                    int checkBit = mapData.wallData[i, j]-3;

                    if(checkBit >= 8)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                vertQueue.Enqueue(new Vector3(i + 1 - (interiorEndX + interiorStartX) / 2, k * 2, (j + l) - (interiorEndY + interiorStartY) / 2));
                                uvQueve.Enqueue(new Vector2(2 - l, (2 - k * 2)));
                            }
                        }

                        checkBit -= 8;
                    }
                    if (checkBit >= 4)
                    {
                        for (int l = 2; l > 0; l++)
                        {
                            for (int k = 2; k > 0; k++)
                            {
                                vertQueue.Enqueue(new Vector3((i + l) - (interiorEndX + interiorStartX) / 2, (1 - k) * 2, j + 1 - (interiorEndY + interiorStartY) / 2));
                                uvQueve.Enqueue(new Vector2(0 + l, (0 + k * 2)));
                            }
                        }

                        checkBit -= 4;
                    }
                    if (checkBit >= 2)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                vertQueue.Enqueue(new Vector3((i + l) - (interiorEndX + interiorStartX) / 2, k * 2, j - (interiorEndY + interiorStartY) / 2));
                                uvQueve.Enqueue(new Vector2(2 - l, (2 - k * 2)));
                            }
                        }

                        checkBit -= 2;
                    }
                    if (checkBit == 1)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                vertQueue.Enqueue(new Vector3(i - (interiorEndX + interiorStartX) / 2, (1 - k) * 2, (j + l) - (interiorEndY + interiorStartY) / 2));
                                uvQueve.Enqueue(new Vector2(0 + l, (0 + k * 2)));
                            }
                        }
                        checkBit -= 1;
                    }  
                }
            }
        }

        mesh = AddToMesh(mesh, ref vertQueue, ref uvQueve);
 
        Material[] material = new Material[mesh.subMeshCount];
        for(int i = 0; i < material.Length; i++)
        {
            material[i] = new Material(Shader.Find("Diffuse"));
            material[i].name = "material_"+i;
            Debug.Log("add material");
        }

        MeshExporter.MeshToFile(mesh, material, "Interior");
      
    }
    public static Mesh AddToMesh(Mesh mesh, ref Queue vertQueue, ref Queue uvQueve)
    {
        Vector3[] new_vertices = new Vector3[vertQueue.Count];
        Vector2[] new_uvs = new Vector2[uvQueve.Count];
  
        for (int i = 0; i < new_vertices.Length; i++)
        {
            new_vertices[i] = (Vector3)vertQueue.Dequeue();
            new_uvs[i] = (Vector2)uvQueve.Dequeue();
        }

        Vector3[] combined_vertices = new Vector3[mesh.vertices.Length + new_vertices.Length];
        Array.Copy(mesh.vertices, combined_vertices, mesh.vertices.Length);
        Array.Copy(new_vertices, 0, combined_vertices, mesh.vertices.Length, new_vertices.Length);

        Vector2[] combined_uvs = new Vector2[mesh.uv.Length + new_uvs.Length];
        Array.Copy(mesh.uv, combined_uvs, mesh.uv.Length);
        Array.Copy(new_uvs, 0, combined_uvs, mesh.uv.Length, new_uvs.Length);

        int[] triangles = new int[(new_vertices.Length / 4) * 6];

        for (int i = 0; i < (new_vertices.Length / 4); i++)
        {
            triangles[(i * 6)] = (i * 4) + mesh.vertices.Length;
            triangles[(i * 6) + 1] = (i * 4) + 1 + mesh.vertices.Length;
            triangles[(i * 6) + 2] = (i * 4) + 3 + mesh.vertices.Length;
            triangles[(i * 6) + 3] = (i * 4) + mesh.vertices.Length;
            triangles[(i * 6) + 4] = (i * 4) + 3 + mesh.vertices.Length;
            triangles[(i * 6) + 5] = (i * 4) + 2 + mesh.vertices.Length;
        }


        if (mesh.vertices.Length != 0)
            mesh.subMeshCount++;

        mesh.vertices = combined_vertices;
        mesh.uv = combined_uvs;
        mesh.SetTriangles(triangles, mesh.subMeshCount - 1, false);
       
        return mesh;
    }


}
