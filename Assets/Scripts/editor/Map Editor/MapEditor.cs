using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    MapData mapData = new MapData();
    enum MapDataType { none, Vertex, Line, Sector};
    MapDataType typeHovered;
    MapDataType typeSelected;
    int hoverID = -1;
    int selectID = -1;
    bool isHolding = false;

    //Line moving variables
    Vector2 mousePosSave;
    Vector2 vertex1Save;
    Vector2 vertex2Save;

    int editMode, selectedTools;
    Texture2D gridBG;

    public string[] editModeName = new string[] { "Vertices\\Lines", "Sector"};

    GUIStyle blackBG = new GUIStyle();
    GUIStyle LightGrayBG = new GUIStyle();

    static Rect EDIT_MODE_AREA = new Rect(16.0f, 96.0f, 96.0f, 64.0f);
    static Rect FILE_FUNCTION_AREA = new Rect(128.0f, 16.0f, 128.0f, 32.0f);
    static Rect WORK_AREA = new Rect(128.0f, 64.0f, 768, 768);
    static Rect SELCTION_EDIT_AREA = new Rect(896, 64, 256, 768);
    const float VERT_SIZE = 10;

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
    } 

    void OnGUI()
    {
        editMode = GUI.SelectionGrid(EDIT_MODE_AREA, editMode, editModeName, 1);
 
        if (GUI.Button(FILE_FUNCTION_AREA, "Export map"))
            MapMeshCreator.CreateMapMesh(ref mapData);

        GUI.BeginGroup(SELCTION_EDIT_AREA, LightGrayBG);
        GUI.EndGroup();

        GUI.BeginGroup(WORK_AREA, blackBG);
        GUI.EndGroup();

        Event e = Event.current;

        //if selectID is higher than -1, then something is currently selected
        //Then we shouldn't allow for any new type to be selected at all
        typeHovered = MapDataType.none;
        hoverID = -1;

        //Paint vertices
        for (int i = 0; i < mapData.verts.Count; i++)
        {
            DrawVert(i, e.mousePosition);
        }

        //Paint lines
        for (int i = 0; i < mapData.lines.Count; i++)
        {
           DrawNodeStraight(i, mapData.verts[mapData.lines[i].startVert], mapData.verts[mapData.lines[i].endVert], e.mousePosition);
        }

        Repaint();

        //If we are not in the work area. Then the followin stuff shouldn't happen.
        if (!WORK_AREA.Contains(e.mousePosition))
        {
            isHolding = false;
            return;
        }

        if(e.type == EventType.mouseDown && e.button == 0)
        {
            if(hoverID != -1)
            {
                typeSelected = typeHovered;
                selectID = hoverID;
                isHolding = true;

                if(typeSelected == MapDataType.Line)
                {
                    mousePosSave = e.mousePosition;
                    vertex1Save = mapData.verts[mapData.lines[selectID].startVert];
                    vertex2Save = mapData.verts[mapData.lines[selectID].endVert];
                }
            }
        }

        else if (e.type == EventType.keyDown && typeSelected == MapDataType.Vertex && e.keyCode == KeyCode.Delete)
        {
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
            else if (typeSelected == MapDataType.Line)
            {
                mapData.verts[mapData.lines[selectID].startVert] = vertex1Save - (mousePosSave - e.mousePosition);
                mapData.verts[mapData.lines[selectID].endVert] = vertex2Save - (mousePosSave - e.mousePosition);
            }
        }
        else if (e.type == EventType.mouseDown && e.button == 1)
        {
            if (typeHovered == MapDataType.Line)
            {
                mapData.AddVertex(e.mousePosition, hoverID);
            }
        }
        else if (e.type == EventType.mouseUp && e.button == 0)
        {
            isHolding = false;
        }
    } 
    void DrawVert(int vertID, Vector2 mPos)
    {
        if (isHolding && typeSelected != MapDataType.Vertex)
            GUI.color = Color.white;
        else if (isHolding && selectID != vertID && typeSelected == MapDataType.Vertex)
            GUI.color = Color.white;
        else if (isHolding && selectID == vertID && typeSelected == MapDataType.Vertex)
            GUI.color = Color.yellow;
        else if (!isHolding && selectID == vertID && typeSelected == MapDataType.Vertex)
        {
            if (new Rect(mapData.verts[vertID].x, mapData.verts[vertID].y, VERT_SIZE, VERT_SIZE).Contains(mPos))
            {
                GUI.color = Color.blue;
                typeHovered = MapDataType.Vertex;
                hoverID = vertID;
            }
            else
                GUI.color = Color.cyan;
        }
        else if (typeHovered == MapDataType.none && new Rect(mapData.verts[vertID].x, mapData.verts[vertID].y, VERT_SIZE, VERT_SIZE).Contains(mPos))
        {
            GUI.color = Color.green;
            typeHovered = MapDataType.Vertex;
            hoverID = vertID;
        }
        else
            GUI.color = Color.white;

        GUI.Box(new Rect(mapData.verts[vertID], new Vector2(VERT_SIZE, VERT_SIZE)), "");
    }
    void DrawNodeStraight(int lineID, Vector2 start, Vector2 end, Vector2 mPos)
    {
        if (isHolding && typeSelected != MapDataType.Line)
            Handles.color = Color.white;
        else if (isHolding && selectID != lineID && typeSelected == MapDataType.Line)
            Handles.color = Color.white;
        else if (isHolding && selectID == lineID && typeSelected == MapDataType.Line)
        {
            Handles.color = Color.yellow;
        }
        else if (!isHolding && selectID == lineID && typeSelected == MapDataType.Line)
        {
            if (ClosestPointOnLine(start + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), end + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mPos))
            {
                Handles.color = Color.blue;
                typeHovered = MapDataType.Line;
                hoverID = lineID;
            }
            else
                Handles.color = Color.cyan;
            
        }

        else if (typeHovered == MapDataType.none && ClosestPointOnLine(start + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), end + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mPos))
        {
            Handles.color = Color.green;
            typeHovered = MapDataType.Line;
            hoverID = lineID;
        }
        else
            Handles.color = Color.white;

        Handles.DrawLine(start + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), end + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2));
    }

    /// <summary>
    /// 
    /// </summary>
   
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
    bool IsPointInPolygon(Vector2 p, Vector2[] polygon)
    {
        double minX = polygon[0].x;
        double maxX = polygon[0].x;
        double minY = polygon[0].y;
        double maxY = polygon[0].y;
        for (int i = 1; i < polygon.Length; i++)
        {
            Vector2 q = polygon[i];
            minX = Math.Min(q.x, minX);
            maxX = Math.Max(q.y, maxX);
            minY = Math.Min(q.y, minY);
            maxY = Math.Max(q.y, maxY);
        }

        if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
        {
            return false;
        }

        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if ((polygon[i].y > p.y) != (polygon[j].y > p.y) &&
                 p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
            {
                inside = !inside;
            }
        }

        return inside;
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
