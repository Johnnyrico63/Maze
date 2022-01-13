using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AmazingMaze.Model
{
    /// <summary>
    /// Spielfeld
    /// </summary>
    public class Tile : Coordinates
    {
        private int nTileSize = 30;

        public int visualsPosX { 
            get { return PosX * nTileSize - 20; }
        }

        public int visualsPosY {
            get { return PosY * nTileSize - 20; } 
        }

        public enum TileTyp
        {
            WALL,
            PATH,
            START,
            END
        }

        public int TileSize { get { return this.nTileSize; } private set { }}

        public TileTyp Typ { get; set; }
        
        /// <summary>
        /// Visuelles definieren der Fliesen
        /// </summary>
        /// <param name="mTile"></param>
        /// <param name="mRect"></param>
        public void createVisuals(Tile mTile, Rectangle mRect)
        {
            mRect.Fill = mTile.GetColor();
            mRect.RenderTransformOrigin = new System.Windows.Point(0.0 , 0.0);
            mRect.Tag = mTile.Typ;
            mRect.Width = this.nTileSize;
            mRect.Height = this.nTileSize;
            mRect.StrokeThickness = 1;
            mRect.Stroke = Brushes.White;
        }

        /// <summary>
        /// Abrufen der FliesenFarbe basierend auf dem Typ
        /// </summary>
        /// <returns></returns>
        public Brush GetColor()
        {
            switch (Typ)
            {
                case TileTyp.WALL:
                    return Brushes.Black;
                case TileTyp.PATH:
                    return Brushes.Gray;
                case TileTyp.START:
                    return Brushes.Green;
                case TileTyp.END:
                    return Brushes.Red;
            }
            return Brushes.LightGray;
        }

        /// <summary>
        /// Abrufen vom Typ basierend auf dem Buchstaben
        /// </summary>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static TileTyp getTyp(string typ)
        {
            switch (typ)
            {
                case "w":
                    return TileTyp.WALL;

                case "z":
                    return TileTyp.END;

                case "s":
                    return TileTyp.START;
            }
            return TileTyp.PATH;
        }  
    }
}