﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    MapData.MapData mapData = new MapData.MapData();
    bool isHovering, isHolding = false;
    int lastHoveredLine, lastHoveredSector = 0;
    int lastSelectedLine, lastSelectedSector = 0;

    List<int> hoveredVertexes = new List<int>();
    List<int> selectedVertexes = new List<int>();
    Vector2 mousePosSave; //Line moving variables
    List<Vector2> savedVertexOriginalPos = new List<Vector2>();

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

        hoveredVertexes.Clear();
        isHovering = false; 

        //In vertex and line mode
        if (editMode == 0)
        {
            if(selectedVertexes.Count == 1)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Vertex: #" + selectedVertexes[0]);
                GUI.EndGroup();
            }
            else if (selectedVertexes.Count == 2 && lastSelectedLine < mapData.lines.Count)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Line: #" + lastSelectedLine);
                GUI.Label(ELEMENT_EDIT_ROW1, "Wall Material");
                mapData.lines[lastSelectedLine].wallMaterialMiddle = comboBoxControl.List(ELEMENT_EDIT_ROW2, materialList[mapData.lines[lastSelectedLine].wallMaterialMiddle].text, materialList);
                GUI.EndGroup();        
            }
        }

        //In sector mode
        if (editMode == 1)
        {
            if(selectedVertexes.Count > 0 && lastSelectedSector < mapData.sectors.Count)
            {
                GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
                GUI.Label(ELEMENT_EDIT_ID, "Sector: #" + lastSelectedSector);
                GUI.Label(ELEMENT_EDIT_ROW1, "Sector ceiling level: " + (float)mapData.sectors[lastSelectedSector].ceilingLevel / 10 + " m");
                mapData.sectors[lastHoveredSector].ceilingLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW2, mapData.sectors[lastSelectedSector].ceilingLevel, -100, 100);
                GUI.Label(ELEMENT_EDIT_ROW3, "Sector floor level: " + (float)mapData.sectors[lastSelectedSector].floorLevel / 10 + " m");
                mapData.sectors[lastHoveredSector].floorLevel = (int)GUI.HorizontalSlider(ELEMENT_EDIT_ROW4, mapData.sectors[lastSelectedSector].floorLevel, -100, 101);
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
                if (isHovering)
                    break;

                isHovering = new Rect(mapData.verts[mapData.sectors[i].verts[j]].x, mapData.verts[mapData.sectors[i].verts[j]].y, VERT_SIZE, VERT_SIZE).Contains(e.mousePosition);
                if (isHovering)
                    hoveredVertexes.Add(mapData.sectors[i].verts[j]);
            }
            //Check Lines
            for (int j = 0; j < mapData.sectors[i].lines.Count; j++)
            {
                if (isHovering)
                    break;

                isHovering = ClosestPointOnLine(mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].startVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].endVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), e.mousePosition);
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
                GUI.color = GetElementColor(hoveredVertexes.Contains(mapData.sectors[i].verts[j]), selectedVertexes.Contains(mapData.sectors[i].verts[j]));
                GUI.Box(new Rect(mapData.verts[mapData.sectors[i].verts[j]], new Vector2(VERT_SIZE, VERT_SIZE)), "");
                //Lines
                Handles.color = GetElementColor(hoveredVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].startVert) && hoveredVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].endVert),
                                                                        selectedVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].startVert) && selectedVertexes.Contains(mapData.lines[mapData.sectors[i].lines[j]].endVert));
                Handles.DrawLine(mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].startVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mapData.verts[mapData.lines[mapData.sectors[i].lines[j]].endVert] + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2));
            }
        }

        //Paint sector drag
        if (editMode == 1 && isHolding && e.button == 1)
        {
            GUI.color = Color.Lerp(Color.green, Color.black, 0.5f);

            GUI.Box(new Rect(mousePosSave, e.mousePosition-mousePosSave), "");
        }

        Repaint();

        //If we are not in the work area. Then the followin stuff shouldn't happen.
        if (!WORK_AREA.Contains(e.mousePosition) || (e.type == EventType.mouseUp && e.button == 0))
        {
            savedVertexOriginalPos.Clear();
            isHolding = false;
        }

        //Selecting Vertexes
        else if (e.type == EventType.mouseDown && e.button == 0 && hoveredVertexes.Count > 0 && !isHolding)
        {
            selectedVertexes.Clear();
            selectedVertexes.AddRange(hoveredVertexes);
            isHolding = true;
            mousePosSave = e.mousePosition;

            if (editMode == 0)
                lastSelectedLine = lastHoveredLine;
            else if (editMode == 1)
                lastSelectedSector = lastHoveredSector;

            for (int i = 0; i < selectedVertexes.Count; i++)
                savedVertexOriginalPos.Add(mapData.verts[selectedVertexes[i]]);
        }

        //Deleting Vertexes
        else if (e.type == EventType.keyDown && e.keyCode == KeyCode.Delete)
        {
            savedVertexOriginalPos.Clear();
            isHolding = false;
            mapData.RemoveVerts(selectedVertexes);
            selectedVertexes.Clear();
        }

        //Moving vertexes
        else if (e.type == EventType.MouseDrag && e.button == 0 && selectedVertexes.Count > 0 && isHolding)
        {
            for (int i = 0; i < savedVertexOriginalPos.Count; i++)
                mapData.verts[selectedVertexes[i]] = savedVertexOriginalPos[i] - (mousePosSave - e.mousePosition);
        }

        //Create (vertex or sector)
        else if (e.type == EventType.mouseDown && e.button == 1 && !isHolding)
        {
            if (editMode == 0 && hoveredVertexes.Count == 2)
                mapData.AddVertex(e.mousePosition, lastHoveredLine);

            else if (editMode == 1)
            {
                mousePosSave = e.mousePosition;
            }
            isHolding = true;
      
        }
        //Finalize Sector
        else if (e.type == EventType.mouseUp && e.button == 1 && editMode == 1)
        {
            //To make sure that the sector is added properly (will otherwisw turn walls the wrong way around. This is controlled by the vertex order)
            Vector2 pointA = new Vector2(Mathf.Min(mousePosSave.x, e.mousePosition.x), Mathf.Min(mousePosSave.y, e.mousePosition.y));
            Vector2 pointB = new Vector2(Mathf.Max(mousePosSave.x, e.mousePosition.x), Mathf.Max(mousePosSave.y, e.mousePosition.y));

            mapData.CreateSector(pointA - new Vector2(EDIT_MODE_AREA.x, 0), pointB - new Vector2(EDIT_MODE_AREA.x - 10, -10));

            isHolding = false;
        }
    }
    
    Color GetElementColor(bool isHovering, bool isSelected)
    {
        if (isSelected && isHolding)
            return Color.yellow;
        else if (isHovering && isSelected)
            return Color.Lerp(Color.blue,Color.yellow, 0.25f);
        else if (!isHovering && isSelected)
            return Color.Lerp(Color.white, Color.blue, 1.00f);
        else if (isHovering && !isSelected)
            return Color.Lerp(Color.white, Color.blue, 0.50f);
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