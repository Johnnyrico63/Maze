using AmazingMaze.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazingMaze.Logic
{
    public class ReadJSON
    {
        public void readJSON(string sFile, List<Tile> mTileList)
        {
            string json = File.ReadAllText(sFile);

            Tile tile;

            var jTileObj = JsonConvert.DeserializeObject<jTile[,]>(json);
            
           foreach(jTile jT in jTileObj)
            {
                tile = new Tile();
                tile.PosX = jT.xCoordinate+1;
                tile.PosY = jT.yCoordinate+1;
                tile.Typ = Tile.getTyp(getLetter(jT.CellStatus));
                mTileList.Add(tile);
            }            
        }

        public string getLetter(int n)
        {
            switch (n)
            {
                case 0:return "w";
                case 2: return "s";
                case 3: return "z";
            }
            return "p";
        }
    }

    public class jTile
    {
        public int xCoordinate;
        public int yCoordinate;
        public int CellStatus;
        public string CellColor;
    }
}
