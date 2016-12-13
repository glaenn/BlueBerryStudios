using System.Collections.Generic;
using UnityEngine;

public static class MapMeshCreator
{
    public static void CreateMapMesh(ref MapData.MapData mapData)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        
        List<int> floorTriangles = new List<int>();
        List<int> ceilingTriangles = new List<int>();
        int[] wallTriangles = new int[mapData.lines.Count * 6];

        //Create walls
        float uvVerticalStart = 0;
        float uvVerticalEnd = 0;
        for (int i = 0; i < mapData.lines.Count * 4; i++)
        {
            int sectorID = 0;

            if (i % 4 == 0)
            {
                for(int j = 0; j < mapData.sectors.Count; j++)
                {
                    if (mapData.sectors[j].ContainsVertex(mapData.lines[i / 4].startVert))
                    {
                        sectorID = j;
                        break;
                    }
                }

                uvVerticalStart = uvVerticalEnd;
                uvVerticalEnd = Vector2.Distance(mapData.verts[mapData.lines[i / 4].startVert], mapData.verts[mapData.lines[i / 4].endVert]) * 0.05f;
                uvVerticalStart -= Mathf.FloorToInt(uvVerticalStart);
                uvVerticalEnd += uvVerticalStart;
            }

            if(i% 4 == 0)
            {
                verts.Add(new Vector3(mapData.verts[mapData.lines[i / 4].startVert].x, mapData.sectors[sectorID].floorLevel, mapData.verts[mapData.lines[i / 4].startVert].y) * 0.1f);
                uvs.Add(new Vector2(uvVerticalStart, mapData.sectors[sectorID].floorLevel * 0.05f));
            }
            else if (i % 4 == 1)
            {
                verts.Add(new Vector3(mapData.verts[mapData.lines[i / 4].startVert].x, mapData.sectors[sectorID].ceilingLevel, mapData.verts[mapData.lines[i / 4].startVert].y) * 0.1f);
                uvs.Add(new Vector2(uvVerticalStart, mapData.sectors[sectorID].ceilingLevel * 0.05f));
            }
            else if (i % 4 == 2)
            {
                verts.Add(new Vector3(mapData.verts[mapData.lines[i / 4].endVert].x, mapData.sectors[sectorID].floorLevel, mapData.verts[mapData.lines[i / 4].endVert].y) * 0.1f);
                uvs.Add(new Vector2(uvVerticalEnd, mapData.sectors[sectorID].floorLevel * 0.05f));
            }
            else if (i % 4 == 3)
            {
                verts.Add(new Vector3(mapData.verts[mapData.lines[i / 4].endVert].x, mapData.sectors[sectorID].ceilingLevel, mapData.verts[mapData.lines[i / 4].endVert].y) * 0.1f);
                uvs.Add(new Vector2(uvVerticalEnd, mapData.sectors[sectorID].ceilingLevel * 0.05f));
            }
 
        }
        //Create wall triangles
        for (int i = 0; i < mapData.lines.Count; i++)
        {
            wallTriangles[(i * 6)] = (i * 4);
            wallTriangles[(i * 6) + 1] = (i * 4) + 2;
            wallTriangles[(i * 6) + 2] = (i * 4) + 1;
            wallTriangles[(i * 6) + 3] = (i * 4) + 1;
            wallTriangles[(i * 6) + 4] = (i * 4) + 2;
            wallTriangles[(i * 6) + 5] = (i * 4) + 3;
        }

        //Create floors and ceiling for eeach sector
        for(int i = 0; i < mapData.sectors.Count; i++)
        {
            List<Vector3> sectorFloorVert = new List<Vector3>();
            List<Vector3> sectorCeilingVert = new List<Vector3>();

            //Find all the vertexes in the secor and add them to the list
            foreach (int vertex in mapData.sectors[i].verts)
            {
                sectorFloorVert.Add(new Vector3(mapData.verts[vertex].x, mapData.sectors[i].floorLevel, mapData.verts[vertex].y) * 0.1f);
                sectorCeilingVert.Add(new Vector3(mapData.verts[vertex].x, mapData.sectors[i].ceilingLevel, mapData.verts[vertex].y) * 0.1f);
            }

            //Create triangle based on the Sector vertex list
            floorTriangles.AddRange(Poly2Mesh.ReturnTriangles(ref sectorFloorVert, Vector3.up));
            ceilingTriangles.AddRange(Poly2Mesh.ReturnTriangles(ref sectorCeilingVert, Vector3.down));

            //Make the triangles reference the new adde floor and ceiling vertex in the verts list
            for (int j = 0; j < floorTriangles.Count; j++)
            {
                floorTriangles[j] += verts.Count;
                ceilingTriangles[j] += verts.Count+ sectorFloorVert.Count;
            }
            
            verts.AddRange(sectorFloorVert);
            verts.AddRange(sectorCeilingVert);
        }
     
        while(uvs.Count < verts.Count)
        {
            uvs.Add(new Vector2(0, 0));
        }
            
        int[][] allTriangles = new int[3][];
        allTriangles[0] = wallTriangles;
        allTriangles[1] = ceilingTriangles.ToArray();
        allTriangles[2] = floorTriangles.ToArray();

        MeshExporter.MeshToFile(MeshExporter.CreateMesh(verts.ToArray(), uvs.ToArray(), allTriangles), MeshExporter.CreateMaterial(allTriangles.GetLength(0)), "Map");

    }
}
