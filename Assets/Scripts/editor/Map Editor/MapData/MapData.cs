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

        public void RemoveVertex(int id)
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].RemoveVertex(id);
            }

            verts.RemoveAt(id);
            BuildLines();        
        }

        private void BuildLines()
        {
            lines.Clear();
            for (int i = 0; i < verts.Count; i++)
            {
                if (i + 1 < verts.Count)
                    lines.Add(new MapDataLine(i, i + 1));
                else
                    lines.Add(new MapDataLine(i, 0));
            }
        }
    }
}
