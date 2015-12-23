using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK
{
    public class Player
    {
        public static Player Me = new Player();

        public List<int> SelectedTowers = new List<int>();
    }
}
