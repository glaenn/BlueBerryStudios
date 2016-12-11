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

        if (editMode == 0)
        {
            if (typeSelected == MapDataType.Sector)
                typeSelected = MapDataType.none;

            if (typeSelected == MapDataType.Line && selectID != -1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Line: " + selectID);
                GUI.Label(ELEMENT_EDIT_ROW1, "Wall Material");
                mapData.lines[selectID].wallMaterialMiddle = comboBoxControl.List(ELEMENT_EDIT_ROW2, materialList[mapData.lines[selectID].wallMaterialMiddle].text, materialList);
                GUI.EndGroup();
            }
        }

        if (editMode == 1)
        {
            for(int i = 0; i < mapData.sectors.Count; i++)
            {
                Vector2[] sectorVertexes = new Vector2[mapData.sectors[i].verts.Count];

                for(int j = 0; j < sectorVertexes.Length; j++)
                {
                    sectorVertexes[j] = mapData.verts[mapData.sectors[i].verts[j]];
                }

                if( IsPointInPolygon(e.mousePosition, sectorVertexes))
                {
                    typeHovered = MapDataType.Sector;
                    hoverID = i;
                }
            }

            if(typeSelected == MapDataType.Sector && selectID != -1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Sector: " + selectID);
                GUI.Label(ELEMENT_EDIT_ROW1, "Sector ceiling level: " + mapData.sectors[selectID].ceilingLevel);
                mapData.sectors[selectID].ceilingLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW2, mapData.sectors[selectID].ceilingLevel, -100, 100);
                GUI.Label(ELEMENT_EDIT_ROW3, "Sector floor level: " + mapData.sectors[selectID].floorLevel);
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

        //Paint vertices
        for (int i = 0; i < mapData.verts.Count; i++)
        {
            bool isHovering = new Rect(mapData.verts[i].x, mapData.verts[i].y, VERT_SIZE, VERT_SIZE).Contains(e.mousePosition);
            GUI.color = DrawElement(i, MapDataType.Vertex, isHovering);
            GUI.Box(new Rect(mapData.verts[i], new Vector2(VERT_SIZE, VERT_SIZE)), "");
        }

        //Paint lines
        for (int i = 0; i < mapData.lines.Count; i++)
        {
           bool isHovering = ClosestPointOnLine(mapData.verts[mapData.lines[i].startVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mapData.verts[mapData.lines[i].endVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), e.mousePosition);
           Handles.color = DrawElement(i, MapDataType.Line, isHovering);
           Handles.DrawLine(mapData.verts[mapData.lines[i].startVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mapData.verts[mapData.lines[i].endVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2));
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
        else if (e.type == EventType.keyDown && typeSelected == MapDataType.Vertex && e.keyCode == KeyCode.Delete)
        {
            selectedVertexes.Clear();
            savedVertexOriginalPos.Clear();
            isHolding = false;
            mapData.RemoveVertex(selectID);
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
    
    Color DrawElement(int elementID, MapDataType elementType, bool isHovering)
    {     
        if (typeSelected == MapDataType.Sector)
            return Color.cyan;
        if (typeHovered == MapDataType.Sector)
            return Color.green;
        else if (editMode == 1)
            return Color.white;
        else if (isHolding && selectID == elementID && typeSelected == elementType)
            return Color.yellow;
        else if (!isHolding && selectID == elementID && typeSelected == elementType && !isHovering)
            return Color.cyan;
        else if (isHovering && typeHovered == MapDataType.none && !isHolding)
        {
            typeHovered = elementType;
            hoverID = elementID;

            if (selectID == elementID && typeSelected == elementType)
                return Color.blue;
            else
                return Color.green;
        }
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