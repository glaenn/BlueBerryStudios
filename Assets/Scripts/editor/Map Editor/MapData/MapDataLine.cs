using System.Collections.Generic;

namespace MapData
{
    public class MapDataLine
    {
        public int startVert;
        public int endVert;
        public int wallMaterialUpper;
        public int wallMaterialMiddle;
        public int wallMaterialLower;

        public MapDataLine(int startVert, int endVert)
        {
            this.startVert = startVert;
            this.endVert = endVert;
        }
    }

}
