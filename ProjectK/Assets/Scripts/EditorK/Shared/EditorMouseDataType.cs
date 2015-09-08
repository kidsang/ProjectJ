using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorK
{
    public enum EditorMouseDataType
    {
        None,

        // int? index
        MapPathStart,

        // int? index
        MapPathEnd,

        // InfoMap
        //   MapCellFlag flag
        //   int size
        //   bool erase
        TerrainFill,
    }
}
