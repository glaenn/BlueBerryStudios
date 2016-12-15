using System.Collections.Generic;
using UnityEngine;

public static class MapMeshCreator
{
    public static void CreateMapMesh(ref MapData.MapData mapData)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        
        Dictionary<int, List<int>> horTriangles = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> verTriangles = new Dictionary<int, List<int>>();

        //Create walls
        float uvVerticalStart = 0;
        float uvVerticalEnd = 0;
        int sectorID = 0;

        //Create walls
        for (int i = 0; i < mapData.lines.Count; i++)
        {
            if (mapData.lines[i].wallMaterialMiddle == 0)
            {
                continue;
            }

            for (int j = 0; j < mapData.sectors.Count; j++)
            {
                if (mapData.sectors[j].ContainsLine(i))
                {
                    sectorID = j;
                    break;
                }
            }

            //To make the walls uv blend together
            uvVerticalStart = uvVerticalEnd;
            uvVerticalEnd = Vector2.Distance(mapData.verts[mapData.lines[i].startVert], mapData.verts[mapData.lines[i].endVert]) * 0.05f;
            uvVerticalStart -= Mathf.FloorToInt(uvVerticalStart);
            uvVerticalEnd += uvVerticalStart;

            verts.Add(new Vector3(mapData.verts[mapData.lines[i].startVert].x, mapData.sectors[sectorID].floorLevel, mapData.verts[mapData.lines[i].startVert].y) * 0.1f);
            uvs.Add(new Vector2(uvVerticalStart, mapData.sectors[sectorID].floorLevel * 0.05f));
            verts.Add(new Vector3(mapData.verts[mapData.lines[i].startVert].x, mapData.sectors[sectorID].ceilingLevel, mapData.verts[mapData.lines[i].startVert].y) * 0.1f);
            uvs.Add(new Vector2(uvVerticalStart, mapData.sectors[sectorID].ceilingLevel * 0.05f));
            verts.Add(new Vector3(mapData.verts[mapData.lines[i].endVert].x, mapData.sectors[sectorID].floorLevel, mapData.verts[mapData.lines[i].endVert].y) * 0.1f);
            uvs.Add(new Vector2(uvVerticalEnd, mapData.sectors[sectorID].floorLevel * 0.05f));
            verts.Add(new Vector3(mapData.verts[mapData.lines[i].endVert].x, mapData.sectors[sectorID].ceilingLevel, mapData.verts[mapData.lines[i].endVert].y) * 0.1f);
            uvs.Add(new Vector2(uvVerticalEnd, mapData.sectors[sectorID].ceilingLevel * 0.05f));

            int material = mapData.lines[i].wallMaterialMiddle;

            if (!verTriangles.ContainsKey(material))
                verTriangles.Add(material, new List<int>());

            verTriangles[material].Add(verts.Count-4);
            verTriangles[material].Add(verts.Count-2);
            verTriangles[material].Add(verts.Count-3);
            verTriangles[material].Add(verts.Count-3);
            verTriangles[material].Add(verts.Count-2);
            verTriangles[material].Add(verts.Count-1);
        }

        //Create floors and ceiling for eeach sector
        for (int i = 0; i < mapData.sectors.Count; i++)
        {
            int material;
            
            for (int j = 0; j < 2; j++)
            {
                if (j == 0)
                    material = mapData.sectors[i].floorMaterial;
                else
                    material = mapData.sectors[i].ceilingMaterial;

                if (material == 0)
                    continue;

                List<Vector3> tempVert = new List<Vector3>();
                List<int> tempTriangles = new List<int>();

                foreach (int vertex in mapData.sectors[i].verts)
                {
                    if (j == 0)
                        tempVert.Add(new Vector3(mapData.verts[vertex].x, mapData.sectors[i].floorLevel, mapData.verts[vertex].y) * 0.1f);
                    else
                        tempVert.Add(new Vector3(mapData.verts[vertex].x, mapData.sectors[i].ceilingLevel, mapData.verts[vertex].y) * 0.1f);
                }
                if (j == 0)
                    tempTriangles.AddRange(Poly2Mesh.ReturnTriangles(ref tempVert, Vector3.up));
                else
                    tempTriangles.AddRange(Poly2Mesh.ReturnTriangles(ref tempVert, Vector3.down));

                //Make the triangles reference the newly added floor and ceiling vertex in the verts list
                for (int l = 0; l < tempTriangles.Count; l++)
                {
                    tempTriangles[l] += verts.Count;
                }

             
                if (!horTriangles.ContainsKey(material))
                    horTriangles.Add(material, new List<int>());

                verts.AddRange(tempVert);
                horTriangles[material].AddRange(tempTriangles);
            }
        }
     
        while(uvs.Count < verts.Count)
        {
            uvs.Add(new Vector2(0, 0));
        }
            
        int[][] allTriangles = new int[verTriangles.Count + horTriangles.Count][];
        int indexer = 0; 

        foreach (KeyValuePair<int, List<int>> entry in verTriangles)
        {
            allTriangles[indexer] = entry.Value.ToArray();
            indexer++;
        }

        foreach (KeyValuePair<int, List<int>> entry in horTriangles)
        {
            allTriangles[indexer] = entry.Value.ToArray();
            indexer++;
        }

        MeshExporter.MeshToFile(MeshExporter.CreateMesh(verts.ToArray(), uvs.ToArray(), allTriangles), MeshExporter.CreateMaterial(allTriangles.GetLength(0)), "Map");
    }
}
