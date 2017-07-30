using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    struct Position
    {
        public int x;
        public int y;
        private int halfWidth;
        private int halfHeight;

        public Position(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            halfWidth = width / 2;
            halfHeight = height / 2;
        }
        public int ty()
        {
            return y - halfHeight;
        }
        public int by()
        {
            return y + halfHeight;
        }
        public int lx()
        {
            return x - halfWidth;
        }
        public int rx()
        {
            return x + halfWidth;
        }
    }
}
