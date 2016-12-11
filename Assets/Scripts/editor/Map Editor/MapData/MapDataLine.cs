namespace MapData
{
    public class MapDataLine
    {
        public int startVert;
        public int endVert;
        public int wallMaterialUpper = 1;
        public int wallMaterialMiddle = 1;
        public int wallMaterialLower = 1;

        public MapDataLine(int startVert, int endVert)
        {
            this.startVert = startVert;
            this.endVert = endVert;
        }
    }

}
