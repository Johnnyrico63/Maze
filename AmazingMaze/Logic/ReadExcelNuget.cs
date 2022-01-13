using AmazingMaze.Model;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazingMaze.Logic
{
    internal class ReadExcelNuget
    {
        public void readExcelNuget(string sFile, List<Tile> mTileList, int difficulty)
        {
            if (!File.Exists(sFile))
                throw new Exception("File not found");

            Tile tile;

            FileStream stream = File.Open(sFile, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);

            int nRowCnt = 0;

            for (int i = 0; i < excelReader.ResultsCount; i++)
            {
                if (i + 1 == difficulty)
                {
                    //var people = new List<Person>();
                    while (excelReader.Read())
                    {
                        nRowCnt++;
                        for (int nColCnt = 0; nColCnt < excelReader.FieldCount; nColCnt++)
                        {
                            tile = new Tile();
                            tile.PosX = nColCnt + 1;
                            tile.PosY = nRowCnt;
                            tile.Typ = Tile.getTyp((string)excelReader.GetValue(nColCnt));
                            mTileList.Add(tile);
                            Console.WriteLine(excelReader.GetValue(nColCnt));//Get Value returns object
                        }
                    }
                }
                excelReader.NextResult();
            }
        }
    }
}
