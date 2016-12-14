using UnityEngine;
using System.Collections.Generic;

namespace MapData
{
    public class MapData
    {
        //MapData
        public List<MapDataSector> sectors = new List<MapDataSector>();
        public List<Vector2> verts = new List<Vector2>();
        public List<MapDataLine> lines = new List<MapDataLine>();

        public void CreateSector(Vector2 startPoint, Vector2 endPoint)
        {
            sectors.Add(new MapDataSector());
            verts.Add(new Vector2(startPoint.x, startPoint.y));
            sectors[sectors.Count - 1].AddVertex(verts.Count - 1);
            verts.Add(new Vector2(endPoint.x, startPoint.y));
            sectors[sectors.Count - 1].AddVertex(verts.Count - 1);
            verts.Add(new Vector2(endPoint.x, endPoint.y));
            sectors[sectors.Count - 1].AddVertex(verts.Count - 1);
            verts.Add(new Vector2(startPoint.x, endPoint.y));
            sectors[sectors.Count - 1].AddVertex(verts.Count - 1);

            BuildLines();
        }

        public void AddVertex(Vector2 point, int id)
        {
            verts.Insert(lines[id].startVert + 1, point);

            for (int i = 0; i < sectors.Count; i++)
                sectors[i].InsertVertex(lines[id].startVert);

            BuildLines();
        }

        public Vector2[] GetSectorVertexes(int sectorID)
        {
            Vector2[] sectorVertexes = new Vector2[sectors[sectorID].verts.Count];

            for (int i = 0; i < sectorVertexes.Length; i++)
            {
                sectorVertexes[i] = verts[sectors[sectorID].verts[i]];
            }

            return sectorVertexes;
        }


        public void RemoveVerts(List<int> vertsToDelete)
        {
            for(int i = 0; i < vertsToDelete.Count; i++)
            {
                Debug.Log("Deleted vert " + vertsToDelete[i]);
                vertsToDelete[i] -= i;
                verts.RemoveAt(vertsToDelete[i]);
                
                for (int j = 0; j < sectors.Count; j++)
                {
    
                    Debug.Log("Checking sector " + j);

                    if (sectors[j].verts.Contains(vertsToDelete[i]))
                        sectors[j].verts.RemoveAt(sectors[j].verts.IndexOf(vertsToDelete[i]));

                    Debug.Log("Checking Sector size " + j + " is " + sectors[j].verts.Count);

                    if (sectors[j].verts.Count == 0)
                    {
                        sectors.RemoveAt(j);
                    }
                    else
                    {
                        for (int k = 0; k < sectors[j].verts.Count; k++)
                        {
                            Debug.Log("Checking all the verts in the sector " + k);
                            if (sectors[j].verts[k] > vertsToDelete[i])
                            {
                                Debug.Log("Vert " + k + "was moved down by 1");
                                sectors[j].verts[k]--;
                            }
                        }
                    }
                }

            }

            BuildLines();

        }

        public void RemoveVertex(int id)
        {
            for (int j = 0; j < sectors.Count; j++)
            {
                sectors[j].RemoveVertex(id);
            }
            verts.RemoveAt(id);
            BuildLines();        
        }

        public void RemoveSector(int id)
        {
            sectors.RemoveAt(id);
            CleanVertexes();
        }

        public void CleanVertexes()
        {
            bool doClean = true;

            for(int i = 0; i < verts.Count; i++)
            {
                doClean = true;
                for (int j = 0; j < sectors.Count; j++)
                {
                    if (sectors[j].ContainsVertex(i))
                        doClean = false;
                }
                if(doClean)
                {
                    RemoveVertex(i);
                    i--;
                }
            }
        }

        private void BuildLines()
        {
            lines.Clear();
            for(int i = 0; i < sectors.Count; i++)
            {
                sectors[i].lines.Clear();

                for(int j = 0; j < sectors[i].verts.Count; j++)
                {
                    if (j + 1 < sectors[i].verts.Count)
                        lines.Add(new MapDataLine(sectors[i].verts[j], sectors[i].verts[j+1]));
                    else
                        lines.Add(new MapDataLine(sectors[i].verts[j], sectors[i].verts[0]));

                    sectors[i].AddLine(lines.Count-1);
                }
            }
        }
    }
}
