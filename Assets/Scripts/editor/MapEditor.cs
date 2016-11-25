using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    MapData mapData = new MapData();
    enum TypeSelected { none, Vertex, Line};
    TypeSelected typeSelected;
    int selectID = -1;

    //Line moving variables
    Vector2 mousePosSave;
    Vector2 vertex1Save;
    Vector2 vertex2Save;

    int editMode, selectedTools;
    Texture2D gridBG;

    public string[] editModeName = new string[] { "Vertices\\Lines", "Sector"};

    GUIStyle blackBG = new GUIStyle();

    static Rect EDIT_MODE_AREA = new Rect(16.0f, 96.0f, 96.0f, 64.0f);
    static Rect TOOL_MODE_AREA = new Rect(16.0f, 164.0f, 96.0f, 64.0f);
    static Rect FILE_FUNCTION_AREA = new Rect(128.0f, 16.0f, 128.0f, 32.0f);
    static Rect WORK_AREA = new Rect(128.0f, 64.0f,  96 * 8, 96 * 8);
    const float VERT_SIZE = 10;

    [MenuItem("MyTools/Map editor")]
    static void ShowEditor()
    {
        GetWindow<MapEditor>();
    }

    public void Awake()
    {
        mapData.InitiateBox(new Vector2(200, 200), new Vector2(300, 300));
        Texture2D gridBG = new Texture2D(128 * 8, 128 * 8);
        LoadGridMap(ref gridBG);
        blackBG.normal.background = CreateColorTexture(1, 1, new Color(0.3f, 0.3f, 0.32f, 1.0f));
    } 

    void OnGUI()
    {
        editMode = GUI.SelectionGrid(EDIT_MODE_AREA, editMode, editModeName, 1);
 
        if (GUI.Button(FILE_FUNCTION_AREA, "Export map"))
            ExportMesh();

        GUI.BeginGroup(WORK_AREA, blackBG);
        GUI.EndGroup();

        Event e = Event.current;

        //if selectID is higher than -1, then something is currently selected
        //Then we shouldn't allow for any new type to be selected at all
        if (selectID == -1)
            typeSelected = TypeSelected.none;

        //Paint vertices
        for (int i = 0; i < mapData.verts.Length; i++)
        {
            DrawVert(i, e.mousePosition, e.type == EventType.mouseDown);
        }

        //Paint lines
        for (int i = 0; i < mapData.lines.Length; i++)
        {
            DrawNodeStraight(i, mapData.verts[mapData.lines[i].startVert], mapData.verts[mapData.lines[i].endVert], e.mousePosition, e.type == EventType.mouseDown);
        }

        if (e.type == EventType.MouseDrag && selectID != -1)
        {
            if (typeSelected == TypeSelected.Vertex)
            {
                mapData.verts[selectID] = e.mousePosition;
            }
            else if (typeSelected == TypeSelected.Line)
            {
                mapData.verts[mapData.lines[selectID].startVert] = vertex1Save - (mousePosSave - e.mousePosition);
                mapData.verts[mapData.lines[selectID].endVert] = vertex2Save - (mousePosSave - e.mousePosition);
            }
        }
        else if (e.type == EventType.mouseUp)
            selectID = -1;

        Repaint();
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="start"></param>
   /// <param name="end"></param>
    void DrawNodeStraight(int lineID, Vector2 start, Vector2 end, Vector2 mPos, bool inputAction)
    {
        Handles.color = Color.white;

        if (typeSelected == TypeSelected.none && selectID == -1 && editMode == 0)
        {
            if (ClosestPointOnLine(start + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), end + new Vector2(VERT_SIZE / 2, VERT_SIZE / 2), mPos))
            {
                typeSelected = TypeSelected.Line;
                Handles.color = Color.green;

                if (inputAction)
                {
                    selectID = lineID;
                    mousePosSave = mPos;
                    vertex1Save = mapData.verts[mapData.lines[selectID].startVert];
                    vertex2Save = mapData.verts[mapData.lines[selectID].endVert];
                }
                

            }
        }
        if (typeSelected == TypeSelected.Line && selectID == lineID)
            Handles.color = Color.blue;
 

        Handles.DrawLine(start + new Vector2(VERT_SIZE/2, VERT_SIZE/2), end + new Vector2(VERT_SIZE/2, VERT_SIZE/2));
  
    }
    void DrawVert(int vertID, Vector2 mPos, bool inputAction)
    {
        GUI.color = Color.white;

        if (typeSelected == TypeSelected.none && selectID == -1 && editMode == 0)
        {
            if (new Rect(mapData.verts[vertID].x, mapData.verts[vertID].y,VERT_SIZE, VERT_SIZE).Contains(mPos))
            {   
                typeSelected = TypeSelected.Vertex;
                GUI.color = Color.green;

                if (inputAction)
                    selectID = vertID;
            }
        }

        if(typeSelected == TypeSelected.Vertex && selectID == vertID)
            GUI.color = Color.blue;

        GUI.Box(new Rect(mapData.verts[vertID], new Vector2(VERT_SIZE, VERT_SIZE)), "");   
    }

 
    /// <summary>
    /// 
    /// </summary>
    void ExportMesh()
    {
        Vector3[] wallVerts = new Vector3[mapData.lines.Length*4];
        Vector2[] wallUVS = new Vector2[mapData.lines.Length * 4];
        Vector3[] floorVerts = new Vector3[mapData.lines.Length];
        Vector2[] floorUVS = new Vector2[mapData.lines.Length];
        Vector3[] ceilingVerts = new Vector3[mapData.lines.Length];
        Vector2[] ceilingUVS = new Vector2[mapData.lines.Length];

        int[] wallTriangles = new int[mapData.lines.Length * 6];
        int[] floorTriangles, ceilingTriangles;
   
        int j = 0;
        for(int i = 0; i < mapData.lines.Length; i++)
        {
            wallVerts[j] = new Vector3(mapData.verts[mapData.lines[i].startVert].x, 0, mapData.verts[mapData.lines[i].startVert].y) * 0.1f;
            wallUVS[j] = new Vector2(0, 0);
            j++;
            wallVerts[j] = new Vector3(mapData.verts[mapData.lines[i].startVert].x, 30, mapData.verts[mapData.lines[i].startVert].y) * 0.1f;
            wallUVS[j] = new Vector2(0, 1);
            j++;
            wallVerts[j] = new Vector3(mapData.verts[mapData.lines[i].endVert].x, 0, mapData.verts[mapData.lines[i].endVert].y) * 0.1f;
            wallUVS[j] = new Vector2(Vector2.Distance(mapData.verts[mapData.lines[i].startVert], mapData.verts[mapData.lines[i].endVert])*0.05f, 0);
            j++;
            wallVerts[j] = new Vector3(mapData.verts[mapData.lines[i].endVert].x, 30, mapData.verts[mapData.lines[i].endVert].y) * 0.1f;
            wallUVS[j] = new Vector2(Vector2.Distance(mapData.verts[mapData.lines[i].startVert], mapData.verts[mapData.lines[i].endVert]) * 0.05f, 1);
            j++;
        }

        for (int i = 0; i < mapData.lines.Length; i++)
        {
            wallTriangles[(i * 6)] = (i * 4);
            wallTriangles[(i * 6) + 1] = (i * 4) + 2;
            wallTriangles[(i * 6) + 2] = (i * 4) + 1;
            wallTriangles[(i * 6) + 3] = (i * 4) + 1;
            wallTriangles[(i * 6) + 4] = (i * 4) + 2;
            wallTriangles[(i * 6) + 5] = (i * 4) + 3;
        }

        for (int i = 0; i < mapData.verts.Length; i++)
        {
            floorVerts[i] = new Vector3(mapData.verts[i].x, 0, mapData.verts[i].y) * 0.1f;
            ceilingVerts[i] = new Vector3(mapData.verts[i].x, 30, mapData.verts[i].y) * 0.1f;
        }

        floorTriangles = Poly2Mesh.ReturnTriangles(ref floorVerts, Vector3.up);
        ceilingTriangles = Poly2Mesh.ReturnTriangles(ref ceilingVerts, Vector3.down);

        for (int i = 0; i < floorTriangles.Length; i++)
        {
           floorTriangles[i] += wallVerts.Length;
           ceilingTriangles[i] += wallVerts.Length + floorVerts.Length;
        }

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        verts.AddRange(wallVerts);
        verts.AddRange(floorVerts);
        verts.AddRange(ceilingVerts);
        uvs.AddRange(wallUVS);
        uvs.AddRange(floorUVS);
        uvs.AddRange(ceilingUVS);

        int[][] allTriangles = new int[3][];
        allTriangles[0] = wallTriangles;
        allTriangles[1] = ceilingTriangles;
        allTriangles[2] = floorTriangles;

        MeshExporter.MeshToFile(MeshExporter.CreateMesh(verts.ToArray(), uvs.ToArray(), allTriangles), MeshExporter.CreateMaterial(allTriangles.GetLength(0)), "Map");

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
