using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioGame
{
    class items
    {
        
        public Image CurrentImage;
        public Rectangle actual;
        public items(int x, int y,int w, int h, Image I)
        {
            actual = new Rectangle(x, y, w, h);
            CurrentImage = I;
        }
    }
    

}
