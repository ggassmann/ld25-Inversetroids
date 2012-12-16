using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace ld25
{
    static class Fonts
    {
        public static Font ExoRegular;
        public static void Init()
        {
            ExoRegular = new Font("rsc/font/Exo-Regular.ttf");
        }
    }
}
