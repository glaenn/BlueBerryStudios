using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    MapData.MapData mapData = new MapData.MapData();
    bool isHovering = false, isHolding = false;
    bool drawGrid = true, snapToGrid = true;
    int lastHoveredLine = 0, lastHoveredSector = 0;
    int lastSelectedLine = 0, lastSelectedSector = 0;
    int gridSize = 2, zoomLevel = 1;

    List<int> hoveredVertexes = new List<int>();
    List<int> selectedVertexes = new List<int>();
    Vector2 mousePosWorkArea;
    Vector2 mousePosSave; //Line moving variables
    List<Vector2> savedVertexOriginalPos = new List<Vector2>();

    public string[] editModeName = new string[] { "Vertices\\Lines", "Sector"};
    int editMode, editModeChangeCheck;
    GUIStyle blackBG = new GUIStyle();
    GUIStyle LightGrayBG = new GUIStyle();

    static Rect EDIT_MODE_AREA = new Rect(16.0f, 96.0f, 96.0f, 64.0f);
    static Rect SHOW_GRID_AREA = new Rect(16.0f, 176.0f, 96.0f, 16.0f);
    static Rect SNAP_GRID_AREA = new Rect(24.0f, 196.0f, 96.0f, 16.0f);
    static Rect SIZE_GRID_TEXT_AREA = new Rect(16.0f, 224.0f, 96.0f, 16.0f);
    static Rect SIZE_GRID_AREA = new Rect(16.0f, 240.0f, 96.0f, 16.0f);
    static Rect ZOOM_TEXT_AREA = new Rect(16.0f, 264.0f, 96.0f, 16.0f);
    static Rect ZOOM_SLIDER_AREA = new Rect(16.0f, 280.0f, 96.0f, 16.0f);

    static Rect FILE_FUNCTION_AREA = new Rect(128.0f, 16.0f, 128.0f, 32.0f);
    static Rect WORK_AREA = new Rect(128.0f, 64.0f, 768, 768);

    static Rect SELCTION_EDIT_AREA = new Rect(896, 64, 256, 768);

    static Rect ELEMENT_EDIT_ID = new Rect(32, 16, 224, 32);
    static Rect ELEMENT_EDIT_ROW1 = new Rect(16, 48, 224, 16);
    static Rect ELEMENT_EDIT_ROW2 = new Rect(16, 64, 224, 16);
    static Rect ELEMENT_EDIT_ROW3 = new Rect(16, 96, 224, 16);
    static Rect ELEMENT_EDIT_ROW4 = new Rect(16, 112, 224, 16);
    static Rect ELEMENT_EDIT_ROW5 = new Rect(16, 144, 224, 16);
    static Rect ELEMENT_EDIT_ROW6 = new Rect(16, 160, 224, 16);
    static Rect ELEMENT_EDIT_ROW7 = new Rect(16, 192, 224, 16);
    static Rect ELEMENT_EDIT_ROW8 = new Rect(16, 208, 224, 16);
    const float VERT_SIZE = 10;

    [MenuItem("MyTools/Map editor")]
    static void ShowEditor()
    {
        GetWindow<MapEditor>();
    }

    public void Awake()
    {
        mapData.CreateSector(new Vector2(128, 128), new Vector2(256, 256));
        blackBG.normal.background = CreateColorTexture(1, 1, new Color(0.3f, 0.3f, 0.32f, 1.0f));
        LightGrayBG.normal.background = CreateColorTexture(1, 1, new Color(0.9f, 0.9f, 0.9f, 1.0f));
    }

    void OnGUI()
    {
        //Edit Mode area
        editMode = GUI.SelectionGrid(EDIT_MODE_AREA, editMode, editModeName, 1);

        //Clear selection when changing modes
        if (GUI.changed && editModeChangeCheck != editMode)
        {
            selectedVertexes.Clear();
            hoveredVertexes.Clear();
            editModeChangeCheck = editMode;
        }

        drawGrid = GUI.Toggle(SHOW_GRID_AREA, drawGrid, "Show Grid");
        if (drawGrid)
            snapToGrid = GUI.Toggle(SNAP_GRID_AREA, snapToGrid, "Snap to Grid");

        GUI.Label(SIZE_GRID_TEXT_AREA, "Grid Size: " + gridSize * 8);
        gridSize = (int)GUI.HorizontalSlider(SIZE_GRID_AREA, gridSize, 2.0f, 8.0f);

        GUI.Label(ZOOM_TEXT_AREA, "Zoom Level: " + zoomLevel);
        zoomLevel = (int)GUI.HorizontalSlider(ZOOM_SLIDER_AREA, zoomLevel, 1.0f, 10.0f);

        //Top Tool bar
        if (GUI.Button(FILE_FUNCTION_AREA, "Export map"))
            MapMeshCreator.CreateMapMesh(ref mapData);

        //Right options

        hoveredVertexes.Clear();
        isHovering = false;

        //In vertex and line mode
        if (editMode == 0)
        {
            if (selectedVertexes.Count == 1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Vertex: #" + selectedVertexes[0]);
                GUI.EndGroup();
            }
            else if (selectedVertexes.Count == 2 && lastSelectedLine < mapData.lines.Count)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Line: #" + lastSelectedLine);
                if (mapData.lines[lastSelectedLine].wallMaterialMiddle == 0)
                    GUI.Label(ELEMENT_EDIT_ROW1, "Wall Material: Hollow");
                else
                    GUI.Label(ELEMENT_EDIT_ROW1, "Wall Material: " + mapData.lines[lastSelectedLine].wallMaterialMiddle);
                mapData.lines[lastSelectedLine].wallMaterialMiddle = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW2, mapData.lines[lastSelectedLine].wallMaterialMiddle, 0, 20);
                GUI.EndGroup();
            }
        }

        //In sector mode
        if (editMode == 1)
        {
            if (selectedVertexes.Count > 0 && lastSelectedSector < mapData.sectors.Count)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Sector: #" + lastSelectedSector);
                GUI.Label(ELEMENT_EDIT_ROW1, "Sector ceiling level: " + (float)mapData.sectors[lastSelectedSector].ceilingLevel / 10 + " m");
                mapData.sectors[lastSelectedSector].ceilingLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW2, mapData.sectors[lastSelectedSector].ceilingLevel, -100, 100);
                GUI.Label(ELEMENT_EDIT_ROW3, "Sector floor level: " + (float)mapData.sectors[lastSelectedSector].floorLevel / 10 + " m");
                mapData.sectors[lastSelectedSector].floorLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW4, mapData.sectors[lastSelectedSector].floorLevel, -100, 101);
                if (mapData.sectors[lastSelectedSector].floorMaterial == 0)
                    GUI.Label(ELEMENT_EDIT_ROW5, "Floor Material: Hollow");
                else
                    GUI.Label(ELEMENT_EDIT_ROW5, "Floor Material: " + mapData.sectors[lastSelectedSector].floorMaterial);
                mapData.sectors[lastSelectedSector].floorMaterial = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW6, mapData.sectors[lastSelectedSector].floorMaterial, 0, 20);
                if (mapData.sectors[lastSelectedSector].ceilingMaterial == 0)
                    GUI.Label(ELEMENT_EDIT_ROW7, "Ceiling Material: Hollow");
                else
                    GUI.Label(ELEMENT_EDIT_ROW7, "Ceiling Material: " + mapData.sectors[lastSelectedSector].ceilingMaterial);
                mapData.sectors[lastSelectedSector].ceilingMaterial = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW8, mapData.sectors[lastSelectedSector].ceilingMaterial, 0, 20);
                GUI.EndGroup();

                if (GUI.changed)
                {
                    if (mapData.sectors[lastSelectedSector].ceilingLevel <= mapData.sectors[lastSelectedSector].floorLevel)
                        mapData.sectors[lastSelectedSector].ceilingLevel = mapData.sectors[lastSelectedSector].floorLevel + 1;
                    else if (mapData.sectors[lastSelectedSector].floorLevel > mapData.sectors[lastSelectedSector].ceilingLevel)
                        mapData.sectors[lastSelectedSector].floorLevel = mapData.sectors[lastSelectedSector].ceilingLevel - 1;
                }
            }
        }

        //Draw Work Area
        Event e = Event.current;
        GUI.BeginGroup(WORK_AREA, blackBG);
        { 
            //Draw Grid
            if (drawGrid)
            {
                Handles.color = Color.gray;

                for (int i = 0; i < WORK_AREA.width / (gridSize * 8); i++)
                    Handles.DrawLine(new Vector2(i * (gridSize * 8), 0), new Vector2(i * (gridSize * 8), WORK_AREA.height));
                for (int i = 0; i < WORK_AREA.height / (gridSize * 8); i++)
                    Handles.DrawLine(new Vector2(0, i * (gridSize * 8)), new Vector2(WORK_AREA.width, i * (gridSize * 8)));

                Handles.color = Color.white;
            }

            //Paint each sector seperatly
            for (int i = 0; i < mapData.sectors.Count; i++)
            {
                //Check whether the sector is hover upon
                if (editMode == 1 && !isHovering)
                {
                    if (IsPointInPolygon(e.mousePosition, mapData.GetSectorVertexes(i)))
                    {
                        hoveredVertexes.AddRange(mapData.sectors[i].verts);
                        lastHoveredSector = i;
                        isHovering = true;
                    }
                }

                //Check vertices
                for (int j = 0; j < mapData.sectors[i].verts.Count; j++)
                {
                    if (isHovering || editMode == 1)
                        break;

                    isHovering = new Rect(mapData.verts[mapData.sectors[i].verts[j]].x - VERT_SIZE / 2, mapData.verts[mapData.sectors[i].verts[j]].y - VERT_SIZE / 2, VERT_SIZE, VERT_SIZE).Contains(e.mousePosition);
                    if (isHovering)
                        hoveredVertexes.Add(mapData.sectors[i].verts[j]);
                }
                //Check Lines
                for (int j = 0; j < mapData.sectors[i].lines.Count; j++)
                {
                    if (isHovering || editMode == 1)
                        break;

                    isHovering = ClosestPointOnLine(mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].startVert], mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].endVert], e.mousePosition);
                    if (isHovering)
                    {
                        hoveredVertexes.Add(mapData.lines[mapData.sectors[i].lines[j]].startVert);
                        hoveredVertexes.Add(mapData.lines[mapData.sectors[i].lines[j]].endVert);
                        lastHoveredLine = mapData.sectors[i].lines[j];
                    }
                }

                //Paint verts and lines
                for (int j = 0; j < mapData.sectors[i].verts.Count; j++)
                {
                    //Verts
                    GUI.color = GetElementColor(hoveredVertexes.Contains(mapData.sectors[i].verts[j]), selectedVertexes.Contains(mapData.sectors[i].verts[j]), isHolding);
                    GUI.Box(new Rect(mapData.verts[mapData.sectors[i].verts[j]] - new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), new Vector2(VERT_SIZE, VERT_SIZE)), "");
                    //Lines
                    Handles.color = GetElementColor(hoveredVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].startVert) && hoveredVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].endVert),
                                                                            selectedVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].startVert) && selectedVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].endVert), isHolding);
                    Handles.DrawLine(mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].startVert], mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].endVert]);
                }
            }

            //Paint sector drag
            if (editMode == 1 && e.button == 1)
            {
                float maxX = Mathf.Max(mousePosSave.x, e.mousePosition.x);
                float maxY = Mathf.Max(mousePosSave.y, e.mousePosition.y);
                float minX = Mathf.Min(mousePosSave.x, e.mousePosition.x);
                float minY = Mathf.Min(mousePosSave.y, e.mousePosition.y);

                GUI.color = Color.Lerp(Color.green, Color.black, 0.5f);
                GUI.Box(new Rect(new Vector2(minX, minY) - (new Vector2(VERT_SIZE, VERT_SIZE) / 2), new Vector2(VERT_SIZE, VERT_SIZE)), "");
                GUI.Box(new Rect(new Vector2(maxX, minY) - (new Vector2(VERT_SIZE, VERT_SIZE) / 2), new Vector2(VERT_SIZE, VERT_SIZE)), "");
                GUI.Box(new Rect(new Vector2(maxX, maxY) - (new Vector2(VERT_SIZE, VERT_SIZE) / 2), new Vector2(VERT_SIZE, VERT_SIZE)), "");
                GUI.Box(new Rect(new Vector2(minX, maxY) - (new Vector2(VERT_SIZE, VERT_SIZE) / 2), new Vector2(VERT_SIZE, VERT_SIZE)), "");

                Handles.color = Color.Lerp(Color.green, Color.black, 0.5f);
                Handles.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
                Handles.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
                Handles.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY));
                Handles.DrawLine(new Vector2(minX, maxY), new Vector2(minX, minY));
            }

            Repaint();

            //On releasing the mouse button 
            if (e.type == EventType.mouseUp && e.button == 0)
            {
                savedVertexOriginalPos.Clear();
                isHolding = false;
            }

            //Selecting Vertexes
            else if (e.type == EventType.mouseDown && e.button == 0 && hoveredVertexes.Count > 0)
            {
                selectedVertexes.Clear();
                selectedVertexes.AddRange(hoveredVertexes);
                isHolding = true;
                mousePosSave = e.mousePosition;

                if (editMode == 0)
                    lastSelectedLine = lastHoveredLine;
                else if (editMode == 1)
                    lastSelectedSector = lastHoveredSector;

                //Snap the first selected Vertex into place
                if (snapToGrid && drawGrid && selectedVertexes.Count == 1)
                    mapData.verts[selectedVertexes[0]] = new Vector2(Mathf.FloorToInt(mapData.verts[selectedVertexes[0]].x / (gridSize * 8)) * (gridSize * 8), Mathf.FloorToInt(mapData.verts[selectedVertexes[0]].y / (gridSize * 8)) * (gridSize * 8));

                for (int i = 0; i < selectedVertexes.Count; i++)
                        savedVertexOriginalPos.Add(mapData.verts[selectedVertexes[i]]);
            }

            //Deleting Vertexes
            else if (e.type == EventType.keyDown && e.keyCode == KeyCode.Delete)
            {
                mapData.RemoveVerts(selectedVertexes);
                selectedVertexes.Clear();
                savedVertexOriginalPos.Clear();
                isHolding = false;
            }

            //Moving vertexes
            else if (e.type == EventType.MouseDrag && e.button == 0 && selectedVertexes.Count > 0)
            {
                Vector2 calculatedMovement = mousePosSave - e.mousePosition;

                //Calculate possbile movement
                for (int i = 0; i < savedVertexOriginalPos.Count; i++)
                {
                    //If the calculated movement will put a vertex outside the work area
                    if (!WORK_AREA.Contains(savedVertexOriginalPos[i] - calculatedMovement + new Vector2(WORK_AREA.x, WORK_AREA.y)))
                    {
                        if (savedVertexOriginalPos[i].x - calculatedMovement.x < 0)
                            calculatedMovement.x += savedVertexOriginalPos[i].x - calculatedMovement.x;
                        else if (savedVertexOriginalPos[i].x - calculatedMovement.x > WORK_AREA.width)
                            calculatedMovement.x += savedVertexOriginalPos[i].x - calculatedMovement.x - WORK_AREA.width;

                        if (savedVertexOriginalPos[i].y - calculatedMovement.y < 0)
                            calculatedMovement.y += savedVertexOriginalPos[i].y - calculatedMovement.y;
                        else if (savedVertexOriginalPos[i].y - calculatedMovement.y > WORK_AREA.height)
                            calculatedMovement.y += savedVertexOriginalPos[i].y - calculatedMovement.y - WORK_AREA.height;
                    }
                }

                if (snapToGrid && drawGrid)
                {
                    calculatedMovement.x = Mathf.FloorToInt(calculatedMovement.x / (gridSize * 8)) * (gridSize * 8);
                    calculatedMovement.y = Mathf.FloorToInt(calculatedMovement.y / (gridSize * 8)) * (gridSize * 8);
                }

                for (int i = 0; i < savedVertexOriginalPos.Count; i++)
                {
                    mapData.verts[selectedVertexes[i]] = savedVertexOriginalPos[i] - calculatedMovement;
                }
            }

            //Create (vertex or sector)
            else if (e.type == EventType.mouseDown && e.button == 1)
            {
                if (editMode == 0 && hoveredVertexes.Count == 2)
                    mapData.AddVertex(e.mousePosition, lastHoveredLine);
                else if (editMode == 1)
                    mousePosSave = e.mousePosition;

                hoveredVertexes.Clear();
                selectedVertexes.Clear();
                isHolding = true;
            }

            //Finalize Sector
            else if (e.type == EventType.mouseUp && e.button == 1 && editMode == 1)
             {
                //To make sure that the sector is added properly (will otherwisw turn walls the wrong way around. This is controlled by the vertex order)
                Vector2 pointA = new Vector2(Mathf.Min(mousePosSave.x, e.mousePosition.x), Mathf.Min(mousePosSave.y, e.mousePosition.y));
                Vector2 pointB = new Vector2(Mathf.Max(mousePosSave.x, e.mousePosition.x), Mathf.Max(mousePosSave.y, e.mousePosition.y));

                mapData.CreateSector(pointA, pointB);
                isHolding = false;
             }

            else if(e.isKey && e.keyCode == KeyCode.Tab && e.type == EventType.keyUp)
            {
                if (editMode == 0)
                    editMode = 1;
                else if (editMode == 1)
                    editMode = 0;

                selectedVertexes.Clear();
                hoveredVertexes.Clear();
            }
        }
        GUI.EndGroup();
    }

    Color GetElementColor(bool isHovering, bool isSelected, bool isHolding)
    {
        if (isSelected && isHolding)
            return Color.green;
        else if (isHovering && isSelected)
            return Color.yellow;
        else if (!isHovering && isSelected)
            return Color.Lerp(Color.white, Color.blue, 1.00f);
        else if (isHovering && !isSelected)
            return Color.Lerp(Color.white, Color.blue, 0.50f);
        else
            return Color.white;
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