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
        int sectorID = 0;
        for (int i = 0; i < mapData.lines.Count * 4; i++)
        {
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
            List<int> newFloorTriangles = new List<int>();
            List<int> newCeilingTriangles = new List<int>();


            //Find all the vertexes in the sector and add them to the list
            foreach (int vertex in mapData.sectors[i].verts)
            {
                sectorFloorVert.Add(new Vector3(mapData.verts[vertex].x, mapData.sectors[i].floorLevel, mapData.verts[vertex].y) * 0.1f);
                sectorCeilingVert.Add(new Vector3(mapData.verts[vertex].x, mapData.sectors[i].ceilingLevel, mapData.verts[vertex].y) * 0.1f);
            }

            //Create triangle based on the Sector vertex list
            newFloorTriangles.AddRange(Poly2Mesh.ReturnTriangles(ref sectorFloorVert, Vector3.up));
            newCeilingTriangles.AddRange(Poly2Mesh.ReturnTriangles(ref sectorCeilingVert, Vector3.down));

            //Make the triangles reference the newly added floor and ceiling vertex in the verts list
            for (int j = 0; j < newFloorTriangles.Count; j++)
            {
                newFloorTriangles[j] += verts.Count;
                newCeilingTriangles[j] += verts.Count+ sectorFloorVert.Count;
            }
            
            verts.AddRange(sectorFloorVert);
            verts.AddRange(sectorCeilingVert);
            floorTriangles.AddRange(newFloorTriangles);
            ceilingTriangles.AddRange(newCeilingTriangles);
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
