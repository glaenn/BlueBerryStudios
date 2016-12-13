using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    enum MapDataType { none, Vertex, Line, Sector };

    MapData.MapData mapData = new MapData.MapData();
    MapDataType typeHovered, typeSelected;
    int hoverID, selectID = -1;
    bool isHolding = false;
    Vector2 mousePosSave; //Line moving variables
    List<Vector2> savedVertexOriginalPos = new List<Vector2>();
    List<int> selectedVertexes = new List<int>();

    Texture2D gridBG;
    public string[] editModeName = new string[] { "Vertices\\Lines", "Sector"};
    int editMode;
    GUIStyle blackBG = new GUIStyle();
    GUIStyle LightGrayBG = new GUIStyle();

    static Rect EDIT_MODE_AREA = new Rect(16.0f, 96.0f, 96.0f, 64.0f);
    static Rect FILE_FUNCTION_AREA = new Rect(128.0f, 16.0f, 128.0f, 32.0f);
    static Rect WORK_AREA = new Rect(128.0f, 64.0f, 768, 768);
    static Rect SELCTION_EDIT_AREA = new Rect(896, 64, 256, 768);
    static Rect ELEMENT_EDIT_ID = new Rect(32, 16, 224, 32);
    static Rect ELEMENT_EDIT_ROW1 = new Rect(16, 48, 224, 16);
    static Rect ELEMENT_EDIT_ROW2 = new Rect(16, 64, 224, 16);
    static Rect ELEMENT_EDIT_ROW3 = new Rect(16, 96, 224, 16);
    static Rect ELEMENT_EDIT_ROW4 = new Rect(16, 112, 224, 16);
    const float VERT_SIZE = 10;

    GUIContent[] materialList;
    private ComboBox comboBoxControl = new ComboBox();

    [MenuItem("MyTools/Map editor")]
    static void ShowEditor()
    {
        GetWindow<MapEditor>();
    }

    public void Awake()
    {
        mapData.CreateSector(new Vector2(200, 200), new Vector2(300, 300));
        mapData.CreateSector(new Vector2(350, 350), new Vector2(450, 450));

        Texture2D gridBG = new Texture2D(128 * 8, 128 * 8);
        LoadGridMap(ref gridBG);
        blackBG.normal.background = CreateColorTexture(1, 1, new Color(0.3f, 0.3f, 0.32f, 1.0f));
        LightGrayBG.normal.background = CreateColorTexture(1, 1, new Color(0.9f, 0.9f, 0.9f, 1.0f));

        materialList = new GUIContent[20];
        materialList[0] = new GUIContent("No material - Hollow");

        for(int i = 1; i < materialList.Length; i++)
            materialList[i] = new GUIContent("Material "+i);
    } 

    void OnGUI()
    {
        editMode = GUI.SelectionGrid(EDIT_MODE_AREA, editMode, editModeName, 1);
 
        if (GUI.Button(FILE_FUNCTION_AREA, "Export map"))
            MapMeshCreator.CreateMapMesh(ref mapData);

        GUI.BeginGroup(WORK_AREA, blackBG);
        GUI.EndGroup();

        Event e = Event.current;

        //Recheck what we hover each rendered frame
        typeHovered = MapDataType.none;

        //In vertex and line mode
        if (editMode == 0)
        {
            if (typeSelected == MapDataType.Sector)
                typeSelected = MapDataType.none;

            if (typeSelected == MapDataType.Line && selectID != -1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Line: #" + selectID);
                GUI.Label(ELEMENT_EDIT_ROW1, "Wall Material");
                mapData.lines[selectID].wallMaterialMiddle = comboBoxControl.List(ELEMENT_EDIT_ROW2, materialList[mapData.lines[selectID].wallMaterialMiddle].text, materialList);
                GUI.EndGroup();
            }

            else if (typeSelected == MapDataType.Vertex && selectID != -1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Vertex: #" + selectID);
                GUI.EndGroup();
            }
        }

        //In sector mode
        if (editMode == 1)
        {
            if(typeSelected == MapDataType.Sector && selectID != -1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Sector: #" + selectID);
                GUI.Label(ELEMENT_EDIT_ROW1, "Sector ceiling level: " + (float)mapData.sectors[selectID].ceilingLevel/10 + " m");
                mapData.sectors[selectID].ceilingLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW2, mapData.sectors[selectID].ceilingLevel, -100, 100);
                GUI.Label(ELEMENT_EDIT_ROW3, "Sector floor level: " + (float)mapData.sectors[selectID].floorLevel/10 + " m");
                mapData.sectors[selectID].floorLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW4, mapData.sectors[selectID].floorLevel, -100, 101);
                GUI.EndGroup();

                if(GUI.changed)
                {
                    if (mapData.sectors[selectID].ceilingLevel <= mapData.sectors[selectID].floorLevel)
                        mapData.sectors[selectID].ceilingLevel = mapData.sectors[selectID].floorLevel + 1;
                    else if (mapData.sectors[selectID].floorLevel > mapData.sectors[selectID].ceilingLevel)
                        mapData.sectors[selectID].floorLevel = mapData.sectors[selectID].ceilingLevel - 1;
                }
            }
        }

       //Paint each sector seperatly
       for (int i = 0; i < mapData.sectors.Count; i++)
       {
            bool isHovering = false;
            bool isSelected = false;

            //Check whether the sector is hover upon
            if (editMode == 1) 
            {
                if (IsPointInPolygon(e.mousePosition, mapData.GetSectorVertexes(i)))
                {
                    typeHovered = MapDataType.Sector;
                    hoverID = i;
                    isHovering = true;
                }
                if (typeSelected == MapDataType.Sector && selectID == i)
                    isSelected = true;
            }

            //Paint vertices
            for (int j = 0; j < mapData.sectors[i].verts.Count; j++)
            {
                if(editMode == 0)
                {
                    isHovering = new Rect(mapData.verts[mapData.sectors[i].verts[j]].x, mapData.verts[mapData.sectors[i].verts[j]].y, VERT_SIZE, VERT_SIZE).Contains(e.mousePosition);
                    if (isHovering)
                    {
                        typeHovered = MapDataType.Vertex;
                        hoverID = mapData.sectors[i].verts[j];
                    }
                    if (selectID == mapData.sectors[i].verts[j] && typeSelected == MapDataType.Vertex)
                        isSelected = true;
                    else if (typeSelected != MapDataType.Sector)
                        isSelected = false;
                }
                GUI.color = GetElementColor(isHovering, isSelected);
                GUI.Box(new Rect(mapData.verts[mapData.sectors[i].verts[j]], new Vector2(VERT_SIZE, VERT_SIZE)), "");
            }

            //Paint lines
            for (int j = 0; j < mapData.sectors[i].lines.Count; j++)
            {
                if (editMode == 0 && typeHovered != MapDataType.Vertex)
                {
                    isHovering = ClosestPointOnLine(mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].startVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].endVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), e.mousePosition);
                    if(isHovering)
                    {
                        typeHovered = MapDataType.Line;
                        hoverID = mapData.sectors[i].lines[j];
                    }
                    if (selectID == mapData.sectors[i].lines[j] && typeSelected == MapDataType.Line)
                        isSelected = true;
                    else if(typeSelected != MapDataType.Sector)
                        isSelected = false;
                }

                if (typeHovered == MapDataType.Vertex)
                    isHovering = false;
                if (typeSelected== MapDataType.Vertex)
                    isSelected= false;

                Handles.color = GetElementColor(isHovering, isSelected);
                Handles.DrawLine(mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].startVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].endVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2));
            }
        }

        Repaint();

        //If we are not in the work area. Then the followin stuff shouldn't happen.
        if (!WORK_AREA.Contains(e.mousePosition) || (e.type == EventType.mouseUp && e.button == 0))
        {
            selectedVertexes.Clear();
            savedVertexOriginalPos.Clear();
            isHolding = false;
            return;
        }
        if(e.type == EventType.mouseUp && e.button == 1 && editMode == 1)
        {
            mapData.CreateSector(e.mousePosition, mousePosSave);
        }


        if (e.type == EventType.mouseDown && e.button == 0 && hoverID != -1)
        {
            typeSelected = typeHovered;
            selectID = hoverID;
            isHolding = true;
            mousePosSave = e.mousePosition;

            if (typeSelected == MapDataType.Line)
            {
                savedVertexOriginalPos.Add(mapData.verts[mapData.lines[selectID].startVert]);
                savedVertexOriginalPos.Add(mapData.verts[mapData.lines[selectID].endVert]);
                selectedVertexes.Add(mapData.lines[selectID].startVert);
                selectedVertexes.Add(mapData.lines[selectID].endVert);
            }
            else if (typeSelected == MapDataType.Sector)
            {
                for (int i = 0; i < mapData.sectors[selectID].verts.Count; i++)
                {
                    savedVertexOriginalPos.Add(mapData.verts[mapData.sectors[selectID].verts[i]]);
                    selectedVertexes.Add(mapData.sectors[selectID].verts[i]);
                }
            }
        }
        else if (e.type == EventType.keyDown && e.keyCode == KeyCode.Delete)
        {
            selectedVertexes.Clear();
            savedVertexOriginalPos.Clear();
            isHolding = false;

            if (typeSelected == MapDataType.Vertex)
                mapData.RemoveVertex(selectID);
            else if (typeSelected == MapDataType.Line)
                mapData.RemoveLine(selectID);
            else if (typeSelected == MapDataType.Sector)
                mapData.RemoveSector(selectID);

            selectID = -1;
            typeSelected = MapDataType.none;
        }

        else if (e.type == EventType.MouseDrag && e.button == 0 && selectID != -1 && isHolding)
        {
            if (typeSelected == MapDataType.Vertex)
            {
                mapData.verts[selectID] = e.mousePosition;
            }

            else if (typeSelected == MapDataType.Line || typeSelected == MapDataType.Sector)
            {
                for (int i = 0; i < savedVertexOriginalPos.Count; i++)
                {
                    mapData.verts[selectedVertexes[i]] = savedVertexOriginalPos[i] - (mousePosSave - e.mousePosition);
                }
            }
        }

        else if (e.type == EventType.mouseDown && e.button == 1)
        {
            if (typeHovered == MapDataType.Line)
                mapData.AddVertex(e.mousePosition, hoverID);
            else if (typeHovered == MapDataType.Sector)
                mousePosSave = e.mousePosition;
        }
    }
    
    Color GetElementColor(bool isHovering, bool isSelected = false)
    {
        if (isSelected && isHolding)
            return Color.yellow;
        else if (isHovering && isSelected)
            return Color.Lerp(Color.blue,Color.white,0.75f);
        else if (!isHovering && isSelected)
            return Color.Lerp(Color.blue, Color.white, 0.25f);
        else if (isHovering && !isSelected)
            return Color.green;
        else
            return Color.white;
    }

    void LoadGridMap(ref Texture2D gridMap)
    {
        Texture2D gridTile = Resources.Load<Texture2D>("editorGraphics/gridTile");

        for (int i = 0; i < gridMap.width / gridTile.width; i++)
            for (int j = 0; j < gridMap.width / gridTile.width; j++)
                gridMap.SetPixels(i * gridTile.width, j * gridTile.width, gridTile.width, gridTile.width, gridTile.GetPixels());

        gridMap.Apply();
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

    bool IsPointInPolygon(Vector2 testPoint, Vector2[] polygon)
    {
        bool result = false;
        int j = polygon.Length-1;
        for (int i = 0; i < polygon.Length; i++)
        {
            if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
            {
                if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    bool ClosestPointOnLine (Vector2 startPoint, Vector2 endPoint, Vector2 mousePoint, float maxDistance = 5.0f)
    {
        Vector2 sM = mousePoint - startPoint;
        Vector2 sE = endPoint - startPoint;

        Vector2 distance = Vector2.Dot(sM, sE) / Vector2.Dot(sE, sE) * sE;

        if (Vector2.Dot(sE, distance) < 0 || sE.magnitude < distance.magnitude)
            return false;

        if (Vector2.Distance(mousePoint, startPoint + distance) < maxDistance)
            return true;

        return false;
    }
}