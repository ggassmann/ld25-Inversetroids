using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace ld25
{
    static class Wrap
    {
        public static void WrapPos(ref Vector2f pos)
        {
            if(pos.X < 0)
            {
                pos.X += 635;
            }
            if(pos.X > 640)
            {
                pos.X -= 635;
            }
            if (pos.Y < 0)
            {
                pos.Y += 635;
            }
            if (pos.Y > 640)
            {
                pos.Y -= 635;
            }
        }
    }
}
