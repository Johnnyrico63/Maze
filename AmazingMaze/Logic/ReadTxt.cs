using AmazingMaze.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazingMaze.Logic
{
    public class ReadTxt
    {
        public void readTxt(string sFile, List<Tile> mTileList)
        {
            if (!File.Exists(sFile))
                throw new Exception("File not found");

            Tile tile;

            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(sFile);
            //Read the first line of text
            String line = sr.ReadToEnd();

            int rBreak = (int)Math.Sqrt(line.Length);
            int nColCnt = 0;
            int nRowCnt = 1;

            
            foreach (char c in line)
            {
                nColCnt++;
                tile = new Tile();
                tile.PosX = nColCnt;
                tile.PosY = nRowCnt;
                tile.Typ = Tile.getTyp(c.ToString());
                mTileList.Add(tile);
                
                if (nColCnt % rBreak == 0)
                {
                    nRowCnt++;
                    nColCnt = 0;
                }
            }
            sr.Close();
        }
    }
}
