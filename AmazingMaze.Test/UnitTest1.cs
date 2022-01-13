using AmazingMaze.Model;
using AmazingMaze.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Media;
using static AmazingMaze.Model.Tile;

namespace AmazingMaze.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FarbenPruefen()
        {
            Tile tile = new Tile();
            tile.Typ = TileTyp.WALL;
            System.Windows.Media.Brush color = tile.GetColor();
        }

    }
}
