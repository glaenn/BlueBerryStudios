using System.Collections.Generic;
using UnityEngine;

public static class MapMeshCreator
{
    public static void CreateMapMesh(ref MapData mapData)
    {
        Vector3[] wallVerts = new Vector3[mapData.lines.Count * 4];
        Vector2[] wallUVS = new Vector2[mapData.lines.Count * 4];
        Vector3[] floorVerts = new Vector3[mapData.lines.Count];
        Vector2[] floorUVS = new Vector2[mapData.lines.Count];
        Vector3[] ceilingVerts = new Vector3[mapData.lines.Count];
        Vector2[] ceilingUVS = new Vector2[mapData.lines.Count];

        int[] wallTriangles = new int[mapData.lines.Count * 6];
        int[] floorTriangles, ceilingTriangles;

        float uvVerticalStart = 0;
        float uvVerticalEnd = 0;
        for (int i = 0; i < mapData.lines.Count * 4; i++)
        {
            if (i % 4 == 0)
            {
                uvVerticalStart = uvVerticalEnd;
                uvVerticalEnd = Vector2.Distance(mapData.verts[mapData.lines[i / 4].startVert], mapData.verts[mapData.lines[i / 4].endVert]) * 0.05f;

                uvVerticalStart -= Mathf.FloorToInt(uvVerticalStart);
                uvVerticalEnd += uvVerticalStart;
            }

            if (i % 4 < 2)
            {
                wallVerts[i] = new Vector3(mapData.verts[mapData.lines[i / 4].startVert].x, (i % 2) * 30, mapData.verts[mapData.lines[i / 4].startVert].y) * 0.1f;
                wallUVS[i] = new Vector2(uvVerticalStart, i % 2);
            }
            else
            {
                wallVerts[i] = new Vector3(mapData.verts[mapData.lines[i / 4].endVert].x, (i % 2) * 30, mapData.verts[mapData.lines[i / 4].endVert].y) * 0.1f;
                wallUVS[i] = new Vector2(uvVerticalEnd, i % 2);
            }    
        }

        for (int i = 0; i < mapData.lines.Count; i++)
        {
            wallTriangles[(i * 6)] = (i * 4);
            wallTriangles[(i * 6) + 1] = (i * 4) + 2;
            wallTriangles[(i * 6) + 2] = (i * 4) + 1;
            wallTriangles[(i * 6) + 3] = (i * 4) + 1;
            wallTriangles[(i * 6) + 4] = (i * 4) + 2;
            wallTriangles[(i * 6) + 5] = (i * 4) + 3;
        }

        for (int i = 0; i < mapData.verts.Count; i++)
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
}
