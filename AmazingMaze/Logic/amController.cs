using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using AmazingMaze.View;
using AmazingMaze.Model;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace AmazingMaze.Logic
{
    public class amController
    {
        #region Variablen
        public Spieler mSpieler = new Spieler();
        Tile mTile = new Tile();
        List<Tile> mTileList;
        Rectangle mPacman;
        public bool bEndCart = false;
       

        public frmMain mFrmMain;
        #endregion

        /// <summary>
        /// Aufbau & Start des Spiels
        /// </summary>
        /// <param name="mCanvas"></param>
        /// <param name="cbDifficulty"></param>
        public void GameSetUP(Canvas mCanvas, ComboBox cbDifficulty)
        {
            mTileList = new List<Tile>();
            mPacman = new Rectangle();

            // Lade Spielfeld aus Excel oder txt Datei
            if (cbDifficulty.SelectedItem is int)
                loadGameFromExcel((int) cbDifficulty.SelectedValue);
            else if ((string)cbDifficulty.SelectedItem == "Test")
                loadGameFromTxt();
            else
                loadGameFromJSON();
            
            // Spielfeld anzeigen
            showTilesToCanvas(mCanvas);

            // Spieler Blickrichtung setzen, auf Startpunkt setzen und Anzeigen 
            mSpieler.nPlayerOrientation = 2;
            setPlayerToStart();
            fillPacmanRect(mCanvas);
            showPacmanToCanvas(mCanvas);
            
            // Application fokussieren um die Tasteneingabe zu ermitteln & Window anpassen
            mCanvas.Focus();
            resizePlayground(mCanvas);
        }

        /// <summary>
        /// Anpassen des Window und Canvas
        /// </summary>
        /// <param name="mCanvas"></param>
        public void resizePlayground(Canvas mCanvas)
        {
            mCanvas.Width = Math.Sqrt(mTileList.Count) * mTile.TileSize + 20;
            mCanvas.Height = mCanvas.Width;
            Application.Current.MainWindow.SizeToContent = SizeToContent.WidthAndHeight;
        }

        /// <summary>
        /// Visuelles erzeugen der "Fliesen" auf dem Canvas als Spielfeld
        /// </summary>
        /// <param name="mCanvas"></param>
        public void showTilesToCanvas(Canvas mCanvas)
        {
            mCanvas.Background = Brushes.DarkGray;
            foreach (Tile mTileHelper in mTileList)
            {
                Rectangle mRect = new Rectangle();
                mTile.createVisuals(mTileHelper, mRect);
                Canvas.SetTop(mRect, mTileHelper.visualsPosY);
                Canvas.SetLeft(mRect, mTileHelper.visualsPosX);
                mCanvas.Children.Add(mRect);
            }   
        }
        
        /// <summary>
        /// Visualisierung des Spielers als Pacman-Rectangle-Objekt
        /// </summary>
        /// <param name="mCanvas"></param>
        public void showPacmanToCanvas(Canvas mCanvas)
        {
            mPacman.RenderTransform = new RotateTransform(getOrientation(mSpieler.nPlayerOrientation), mTile.TileSize / 2, mTile.TileSize / 2);
            Canvas.SetTop(mPacman, mSpieler.visualsPosY);
            Canvas.SetLeft(mPacman, mSpieler.visualsPosX);
            nextTile(mSpieler.PosX, mSpieler.PosY);
        }

        /// <summary>
        ///  Spieler ( Player) - Objekt wird auf Startpunkt gesetzt
        /// </summary>
        private void setPlayerToStart()
        {
            // --- Linq-Lambda-Expression ohne foreach --
            Tile mHelpTile2 = mTileList.Where(s => s.Typ == Tile.TileTyp.START).FirstOrDefault();
            mSpieler.PosX = mHelpTile2.PosX;
            mSpieler.PosY = mHelpTile2.PosY;

            #region Weitere Linq-Beispiele
            // --- Linq langschreibweise ---
            /*var resultX = from mTile in mTileList
                         where mTile.Typ == Tile.TileTyp.START
                         select mTile.PosX;

            var resultY = from mTile in mTileList
                          where mTile.Typ == Tile.TileTyp.START
                          select mTile.PosY;

            foreach(var result in resultX)
                mSpieler.PosX = result;
            foreach (var result in resultY)
                mSpieler.PosY = result;*/

            // --- Linq-Lambda-Expression inkl. foreach ---
            /*foreach (Tile mHelpTile in mTileList.Where(s => s.Typ == Tile.TileTyp.START)) // um die einzelene Variable zu bekommen -> .Select(t => t.PosX))
            {
                mSpieler.PosX = mHelpTile.PosX;
                mSpieler.PosY= mHelpTile.PosY;
            }*/

            // --- ohne Linq nur Foreach ---
            /* foreach (Tile mTile in mTileList)
            {
                if (mTile.Typ == Tile.TileTyp.START)
                {
                    mSpieler.PosX = mTile.PosX;
                    mSpieler.PosY = mTile.PosY;
                }
            }*/
            /// </summary>
            #endregion
        }

        /// <summary>
        /// Das "Pacman"-Rectangle wird visuell aufbereitet und auf den Canvas gebracht
        /// </summary>
        /// <param name="mCanvas"></param>
        public void fillPacmanRect(Canvas mCanvas)
        { 
            mSpieler.addPlayerPicture(mPacman);
            mPacman.RenderTransformOrigin = new Point(0.0, 0.0);
            mPacman.Width = mTile.TileSize;
            mPacman.Height = mTile.TileSize;
            mCanvas.Children.Add(mPacman); 
        }

        /// <summary>
        /// Ermittlung/Definition der Blickrichtung
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int getOrientation(int i)
        {
            int nRet = 0;
            switch (i)
            {
                case 1:
                    nRet = -90;
                    break;
                case 2:
                    nRet = 0;
                    break;
                case 3:
                    nRet = 90;
                    break;
                case 4:
                    nRet = 180;
                    break;
            }
            return nRet;
        }

        /// <summary>
        /// Befüllen der TileList mit den Daten aus der Excel-Tabelle
        /// </summary>
        /// <param name="difficulty"></param>
        private void loadGameFromExcel(int difficulty)
        {
            string sWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string sFile = String.Format("{0}\\Resources\\Maze.xls", sWorkPath);
            ReadExcelNuget rEN = new ReadExcelNuget();
            rEN.readExcelNuget(sFile, mTileList, difficulty);
            //ReadExcel rE = new ReadExcel();
            //rE.readExcel(sFile, mTileList, difficulty);
            #region alternative Schreibweise
            //rE.excelRead(sWorkPath + "\\Resources\\TestMaze2.xls");
            #endregion
        }

        /// <summary>
        /// Befüllen der TileList mit TestMaze.txt
        /// </summary>
        private void loadGameFromTxt()
        {
            ReadTxt mTR = new ReadTxt();
            string sWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string sFile = String.Format("{0}\\Resources\\TestMaze.txt", sWorkPath);
            mTR.readTxt(sFile, mTileList);
            #region alternative Schreibweise
            //rE.excelRead(sWorkPath + "\\Resources\\TestMaze2.xls");
            #endregion
        }

        private void loadGameFromJSON()
        {
            ReadJSON rJ = new ReadJSON();
            string sWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string sFile = String.Format("{0}\\Resources\\Easy.json", sWorkPath);
            rJ.readJSON(sFile, mTileList);
        }

        /// <summary>
        /// Erzeugung der SpielerBewegung
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mCanvas"></param>
        public void movePlayer(Key e, Canvas mCanvas, bool b3D)
        {
            bool moved = false;
            // Vorbereitung der Kollisionsprüfung - Ermittlung der Fliesen um den Spieler herum
            List<Tile> viewTiles = nextTile(mSpieler.PosX, mSpieler.PosY);

            // Veränderung der "Blickrichtung" & Bewegung
            switch (e)
            {
                case Key.Left:
                    mSpieler.nPlayerOrientation -= 1;
                    break;
                case Key.Right:
                    mSpieler.nPlayerOrientation += 1;
                    break;
                // Vorwärtsbewegung mit Kollisionsprüfung
                case Key.Up:
                    if (mSpieler.nPlayerOrientation == 1 && viewTiles[mSpieler.nPlayerOrientation].Typ != Tile.TileTyp.WALL)
                        mSpieler.PosY -= 1;
                    if (mSpieler.nPlayerOrientation == 2 && viewTiles[mSpieler.nPlayerOrientation].Typ != Tile.TileTyp.WALL)
                        mSpieler.PosX += 1;
                    if (mSpieler.nPlayerOrientation == 3 && viewTiles[mSpieler.nPlayerOrientation].Typ != Tile.TileTyp.WALL)
                        mSpieler.PosY += 1;
                    if (mSpieler.nPlayerOrientation == 4 && viewTiles[mSpieler.nPlayerOrientation].Typ != Tile.TileTyp.WALL)
                        mSpieler.PosX -= 1;

                    moved = true;
                    break;
            }
            
            // Aktualisierung der Blickrichtung bei Ungültigkeit
            if (mSpieler.nPlayerOrientation == 0)
                mSpieler.nPlayerOrientation = 4;
            if (mSpieler.nPlayerOrientation == 5)
                mSpieler.nPlayerOrientation = 1;

            nextTile(mSpieler.PosX, mSpieler.PosY);

            if (b3D)
                mFrmMain.view3D();
            else                
                showPacmanToCanvas(mCanvas);

            Console.WriteLine("{0},{1},{2}", mSpieler.PosX, mSpieler.PosY, mSpieler.nPlayerOrientation);

            // Prüfung ob Ziel erreicht wurde
            if (viewTiles[mSpieler.nPlayerOrientation].Typ == Tile.TileTyp.END && moved)
            {
                endOfGame(mCanvas);
                bEndCart = true;
            }
        }

        public List<Tile> nextTile( int tPosX, int tPosY)
        {
            List<Tile> viewReturnTiles = new List<Tile>();
            Tile centerTile, northTile, westTile, eastTile, southTile;

            centerTile = mTileList.Where(dT => dT.PosY == tPosY && dT.PosX == tPosX).FirstOrDefault();
            northTile = mTileList.Where(dT => dT.PosY == tPosY - 1 && dT.PosX == tPosX).FirstOrDefault();
            westTile = mTileList.Where(dT => dT.PosY == tPosY && dT.PosX == tPosX - 1).FirstOrDefault();
            eastTile = mTileList.Where(dT => dT.PosY == tPosY && dT.PosX == tPosX + 1).FirstOrDefault();
            southTile = mTileList.Where(dT => dT.PosY == tPosY + 1 && dT.PosX == tPosX).FirstOrDefault();

            viewReturnTiles.Add(centerTile);
            viewReturnTiles.Add(northTile);
            viewReturnTiles.Add(eastTile);
            viewReturnTiles.Add(southTile);
            viewReturnTiles.Add(westTile);

            return viewReturnTiles;

        }

        /// <summary>
        /// Spielende und neuladen vom GameMenu
        /// </summary>
        /// <param name="mCanvas"></param>
        private void endOfGame(Canvas mCanvas)
        {
            mCanvas.Children.Clear();
            
            string sWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            ImageBrush endCart = new ImageBrush();
            endCart.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\endCart.jpg", sWorkPath)));

            mCanvas.Background = endCart;

            mCanvas.Width = 550;
            mCanvas.Height = 300;
            Application.Current.MainWindow.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
