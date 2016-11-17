using UnityEngine;

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
    public struct Lines
    {
        public int startVert;
        public int endVert;
        public int wallMaterialUpper;
        public int WallMaterialMiddle;
        public int WallMaterialLower;
    }

    //MapData
    public Sector[] sectors;
    public Vector2[] verts;
    public Lines[] lines;

    public void InitiateBox(Vector2 startPoint, Vector2 endPoint)
    {
        verts = new Vector2[5];
        lines = new MapData.Lines[5];

        verts[0] = new Vector2(startPoint.x, startPoint.y);
        verts[1] = new Vector2(endPoint.x, startPoint.y);
        verts[2] = new Vector2(endPoint.x, endPoint.y);
        verts[3] = new Vector2(startPoint.x, endPoint.y);
        verts[4] = new Vector2(startPoint.x, Mathf.Lerp(startPoint.y, endPoint.y, 0.5f));

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].startVert = i;

            if (i + 1 < verts.Length)
                lines[i].endVert = i + 1;
            else
                lines[i].endVert = 0;
        }

    }


}
