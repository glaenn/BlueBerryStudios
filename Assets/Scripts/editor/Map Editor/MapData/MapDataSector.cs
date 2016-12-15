using System.Collections.Generic;
using UnityEngine;
namespace MapData
{
    public class MapDataSector
    {
        public List<int> verts = new List<int>();
        public List<int> lines = new List<int>();
        public int floorLevel = 0;
        public int ceilingLevel = 30;
        public int floorMaterial = 1;
        public int ceilingMaterial = 2;

        public bool ContainsVertex(int id){return verts.Contains(id);}
        public bool ContainsLine(int id) { return lines.Contains(id); }

        public void AddVertex(int id)
        {
            verts.Add(id);
        }

        public void AddLine(int id)
        {
            lines.Add(id);
        }

        public void InsertVertex(int id)
        {
            for (int i = 0; i < verts.Count; i++)
                if (verts[i] > id)
                    verts[i]++;

            if (verts.Contains(id))
                verts.Insert(verts.IndexOf(id) + 1, id + 1);
        }
    }
}
