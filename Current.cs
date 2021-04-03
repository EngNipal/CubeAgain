using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    public static class Current
    {
        public static int[] State { get; set; }
        private static Position _position { get; set; }
        public static Position Position
        {
            get { return _position; }
            internal set
            {
                _position = new Position(State);
            }
        }
    }
}
