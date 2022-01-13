using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AmazingMaze.Model
{
    public class Spieler : Tile
    {   

        public int nPlayerOrientation { get; set; } //Format y- == 3, y+ == 1, x- == 4, x+ == 2

        public bool alive { get; set; }

        public void addPlayerPicture(Rectangle player)
        {
            string sWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\pacman.png", sWorkPath)));
            player.Fill = pacmanImage;
        }
    }
}
