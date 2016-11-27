using UnityEngine;
using System.Collections.Generic;

public class MapData
{
    public struct Sector
    {
        public int[] verts;
        public int groundLevel;
        public int ceilingLevel;
        public int floorMaterial;
        public int ceilingMaterial;
    }
    public struct Line
    {
        public int startVert;
        public int endVert;
        public int wallMaterialUpper;
        public int wallMaterialMiddle;
        public int wallMaterialLower;

        public Line (int startVert, int endVert)
        {
            this.startVert = startVert;
            this.endVert = endVert;

            wallMaterialUpper = 0;
            wallMaterialMiddle = 0;
            wallMaterialLower = 0;
        }
    }

    //MapData
    public List<Sector> sectors = new List<Sector>();
    public List <Vector2> verts = new List<Vector2>();
    public List<Line> lines = new List<Line>();

    public void CreateSector(Vector2 startPoint, Vector2 endPoint)
    {
        verts.Add(new Vector2(startPoint.x, startPoint.y));
        verts.Add(new Vector2(endPoint.x, startPoint.y));
        verts.Add(new Vector2(endPoint.x, endPoint.y));
        verts.Add(new Vector2(startPoint.x, endPoint.y));

        BuildLines();
    }

    public void AddVertex(Vector2 point, int lineID)
    {
        int placement = lines[lineID].startVert;
        verts.Insert(placement+1, point);
        BuildLines();
    }

    private void BuildLines()
    {
        lines.Clear();
        for (int i = 0; i < verts.Count; i++)
        {
            if (i + 1 < verts.Count)
                lines.Add(new Line(i, i + 1));
            else
                lines.Add(new Line(i, 0));
        }

    }


}
