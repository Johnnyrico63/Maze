using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AmazingMaze.Logic;
using AmazingMaze.Model;

namespace AmazingMaze.View
{
    /// <summary>
    /// Interaction logic for frmMain.xaml
    /// </summary>
    public partial class frmMain : Window
    {
        #region Variablen
        amController aC = new amController();
        public ComboBox cbDifficulty;
        public bool b3D_Active = false;
        string sWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public Spieler mSpieler;
        ReadJSON rJ = new ReadJSON();
        #endregion

        public frmMain()
        {
            InitializeComponent();
            aC.mFrmMain = this;
            loadGameMenu();
        }

        /// <summary>
        /// Erzeugen vom GameMenu mit allen Elementen
        /// </summary>
        public void loadGameMenu()
        {
            Button btStart = new Button();
            cbDifficulty = new ComboBox();

            ImageBrush StartMenuImage = new ImageBrush();
            StartMenuImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\StartMenu.jpg", sWorkPath)));

            btStart.Content = "Start";
            btStart.Width = 200;
            Canvas.SetTop(btStart, 50);
            Canvas.SetLeft(btStart, 50);
            btStart.Background = Brushes.Black;
            btStart.BorderBrush = Brushes.White;
            btStart.Foreground = Brushes.White;

            cbDifficulty.Items.Add("Wähle ein Level");
            cbDifficulty.Items.Add(1);
            cbDifficulty.Items.Add(2);
            cbDifficulty.Items.Add(3);
            cbDifficulty.Items.Add("Test");
            cbDifficulty.Items.Add("JsonTest");

            cbDifficulty.Width = 200;
            Canvas.SetTop(cbDifficulty, 50);
            Canvas.SetLeft(cbDifficulty, 300);

            cbDifficulty.SelectedIndex = 0;
            cbDifficulty.Foreground = Brushes.White;
            cbDifficulty.Resources.Add(SystemColors.WindowBrushKey, Brushes.Black);
            cbDifficulty.Resources.Add(SystemColors.HighlightBrushKey, Brushes.Black);
            cbDifficulty.HorizontalContentAlignment = HorizontalAlignment.Center;
            

            mCanvas.Children.Add(btStart);
            mCanvas.Children.Add(cbDifficulty);

            mCanvas.Background = StartMenuImage;

            mCanvas.Width = 550;
            mCanvas.Height = 300;
            Application.Current.MainWindow.SizeToContent = SizeToContent.WidthAndHeight;

            btStart.Click += new RoutedEventHandler(this.Button_Click);

        }
        
        /// <summary>
        /// Erzeugen der 3D-Ansicht des Spiels auf basis der TileList
        /// </summary>
        public void view3D()
        { 
            Polygon floor = null;
            Polygon leftWall = null;
            Polygon rightWall = null;
            Polygon crossWall = null;
            Polygon endWall = null;
            Polygon bckgCenter = null;

            Rectangle bckgLeft = null;
            Rectangle bckgRight = null;
            Polygon bottomLeft = null;
            Polygon bottomRight = null;
            
            int i;
            int p1x = 450;
            int p1y = 1000 - 125;
            int p2x = 1450;
            int p2y = 1000 - 125;
            int width = 0;
            int offset;

            bool end = false;
            bool deadEndLeft = false;
            bool deadEndRight = false;
            bool finish = false;


            List<Tile> view3DTiles = null;
            Tile centerTile = new Tile(),
                leftTile = new Tile(),
                rightTile = new Tile(),
                rearTile = new Tile(),
                rearLeftTile = new Tile(),
                rearRightTile = new Tile();

            #region Bilder werden anhand der Bild-Dateien im Resources-Ordner geladen
            ImageBrush wallLeftBkgImage = new ImageBrush();
            wallLeftBkgImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\leftWall.png", sWorkPath)));

            ImageBrush lWuImage = new ImageBrush();
            lWuImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\leftWallUpcoming.png", sWorkPath)));

            ImageBrush cWLSImage = new ImageBrush();
            cWLSImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\crosswallLeftSide.png", sWorkPath)));

            ImageBrush cWLSUImage = new ImageBrush();
            cWLSUImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\crosswallLeftSideUpcoming.png", sWorkPath)));

            ImageBrush wallRightBkgImage = new ImageBrush();
            wallRightBkgImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\rightWall.png", sWorkPath)));

            ImageBrush rWuImage = new ImageBrush();
            rWuImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\rightWallUpcoming.png", sWorkPath)));

            ImageBrush cWRSImage = new ImageBrush();
            cWRSImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\crosswallRightSide.png", sWorkPath)));

            ImageBrush cWRSUImage = new ImageBrush();
            cWRSUImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\crosswallRightSideUpcoming.png", sWorkPath)));

            ImageBrush cWImage = new ImageBrush();
            cWImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\crossWall.png", sWorkPath)));

            ImageBrush arcadeImage = new ImageBrush();
            arcadeImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\ArcadeBackground.png", sWorkPath)));

            ImageBrush stopImage = new ImageBrush();
            stopImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\stop.png", sWorkPath)));

            ImageBrush finishImage = new ImageBrush();
            finishImage.ImageSource = new BitmapImage(new Uri(String.Format("{0}\\Resources\\cherryEndWall.png", sWorkPath)));
            #endregion

            #region Erzeugung des Hintergrunds
            Rectangle background = new Rectangle();
            background.Width = 1000;
            background.Height = 500;
            Canvas.SetLeft(background, 450);
            background.Fill = arcadeImage;

            mCanvas.Children.Add(background);

            Rectangle background2 = new Rectangle();
            background2.Width = 1000;
            background2.Height = 500;
            Canvas.SetLeft(background2, 450);
            Canvas.SetTop(background2, 500);
            background2.Fill = Brushes.Black;

            mCanvas.Children.Add(background2);
            #endregion

            // Render-Loop für die kommenden Blöcke
            for (i = 0; i < 8; i++)
            {
                floor = new Polygon();
                leftWall = new Polygon();
                rightWall = new Polygon();
                

                // Ermittlung der umliegenden Tiles und Zuordnung zu local Tiles 
                if (aC.mSpieler.nPlayerOrientation == 1)
                {
                    view3DTiles = aC.nextTile(aC.mSpieler.PosX, aC.mSpieler.PosY);
                    rearTile = view3DTiles[0];
                    rearLeftTile = view3DTiles[4];
                    rearRightTile = view3DTiles[2];

                    view3DTiles = aC.nextTile(aC.mSpieler.PosX, aC.mSpieler.PosY - i - 1);
                    centerTile = view3DTiles[0];
                    leftTile = view3DTiles[4];
                    rightTile = view3DTiles[2];
                   
                }
                if (aC.mSpieler.nPlayerOrientation == 2)
                {
                    view3DTiles = aC.nextTile(aC.mSpieler.PosX, aC.mSpieler.PosY);
                    rearTile = view3DTiles[0];
                    rearLeftTile = view3DTiles[1];
                    rearRightTile = view3DTiles[3];

                    view3DTiles = aC.nextTile(aC.mSpieler.PosX + i + 1, aC.mSpieler.PosY);
                    centerTile = view3DTiles[0];
                    leftTile = view3DTiles[1];
                    rightTile = view3DTiles[3];
                    
                }
                if (aC.mSpieler.nPlayerOrientation == 3)
                {
                    view3DTiles = aC.nextTile(aC.mSpieler.PosX, aC.mSpieler.PosY);
                    rearTile = view3DTiles[0];
                    rearLeftTile = view3DTiles[2];
                    rearRightTile = view3DTiles[4];

                    view3DTiles = aC.nextTile(aC.mSpieler.PosX, aC.mSpieler.PosY + i + 1);
                    centerTile = view3DTiles[0];
                    leftTile = view3DTiles[2];
                    rightTile = view3DTiles[4];
                }
                if (aC.mSpieler.nPlayerOrientation == 4)
                {
                    view3DTiles = aC.nextTile(aC.mSpieler.PosX, aC.mSpieler.PosY);
                    rearTile = view3DTiles[0];
                    rearLeftTile = view3DTiles[3];
                    rearRightTile = view3DTiles[1];

                    view3DTiles = aC.nextTile(aC.mSpieler.PosX - i - 1, aC.mSpieler.PosY);
                    centerTile = view3DTiles[0];
                    leftTile = view3DTiles[3];
                    rightTile = view3DTiles[1];
                }

                // T-Stück
                if (centerTile.Typ == Tile.TileTyp.WALL)
                {
                    endWall = new Polygon();
                    endWall.Points.Add(new Point(p1x, p1y));
                    endWall.Points.Add(new Point(p2x, p2y));
                    endWall.Points.Add(new Point(p2x, p2y - (p2x - p1x)));
                    endWall.Points.Add(new Point(p1x, p1y - (p2x - p1x)));
                    
                    endWall.Fill = cWImage;
                    endWall.Stroke = Brushes.White;
                    endWall.StrokeThickness = 3;
                    
                    end = true;
                }
                // T-Stück Erweiterung Dead-End
                if ((centerTile.Typ == Tile.TileTyp.WALL && deadEndLeft && deadEndRight)||(i==0 && centerTile.Typ == Tile.TileTyp.WALL && rearLeftTile.Typ == Tile.TileTyp.WALL && rearRightTile.Typ == Tile.TileTyp.WALL))
                {
                    endWall.Fill = stopImage;

                    end = true;
                }
                // T-Stück Erweiterung Ziel
                if ((rearTile.Typ == Tile.TileTyp.END || finish) && end)
                {
                    endWall.Fill = finishImage;

                    end = true;
                }
                // T-Stück erweiterung Break
                if (end)
                {
                    mCanvas.Children.Add(endWall);
                    break;
                }
                
                // Boden und Wände der Kommenden Blöcke
                else
                {
                    floor.Points.Add(new Point(p1x, p1y));
                    floor.Points.Add(new Point(p2x, p2y));
                    
                    leftWall.Points.Add(new Point(p1x, p1y));
                    rightWall.Points.Add(new Point(p2x, p2y));

                    width = p2x - p1x;                          // Ermittlung der temporären Breite des Trapezes
                    offset = width / 4;                         // Ermittlung des Einzugs des Trapezes

                    leftWall.Points.Add(new Point(p1x, p1y - width));
                    rightWall.Points.Add(new Point(p2x, p2y - width));

                    // Umrechnung / Aktualisierung der Punkte 
                    p1y = p2y = p2y - offset;
                    p1x = p1x + offset;
                    p2x = p2x - offset;

                    floor.Points.Add(new Point(p2x, p2y));
                    floor.Points.Add(new Point(p1x, p1y));

                    floor.Fill = Brushes.Black;
                    floor.Stroke = Brushes.White;
                    floor.StrokeThickness = 3;

                    // Abprüfung ob Zielerreicht wurde und Darstellung
                    if (centerTile.Typ == Tile.TileTyp.END)
                    {
                        floor.Fill = Brushes.DarkRed;
                        finish = true;
                    }

                    if (centerTile.Typ == Tile.TileTyp.START)
                        floor.Fill = Brushes.DarkGreen;
                    
                    mCanvas.Children.Add(floor);

                    // Erzeugung von Wand bzw. Abzweigung links
                    if (leftTile.Typ == Tile.TileTyp.WALL) //Wand
                    {
                        leftWall.Points.Add(new Point(p1x, p1y - width / 2));
                        leftWall.Points.Add(new Point(p1x, p1y));

                        leftWall.Fill = lWuImage;
                        leftWall.Stroke = Brushes.White;
                        leftWall.StrokeThickness = 3;

                        mCanvas.Children.Add(leftWall);
                        
                        deadEndLeft = true;
                    }
                    else // Abzweigung
                    {
                        leftWall.Points.RemoveAt(leftWall.Points.Count - 1);
                        leftWall.Points.Add(new Point(p1x - offset, p1y));
                        leftWall.Points.Add(new Point(p1x, p1y));

                        leftWall.Fill = Brushes.Black; 
                        leftWall.Stroke = Brushes.White;
                        leftWall.StrokeThickness = 3;

                        if (leftTile.Typ == Tile.TileTyp.START)
                            leftWall.Fill = Brushes.DarkGreen;
                        if (leftTile.Typ == Tile.TileTyp.END)
                            leftWall.Fill = Brushes.DarkRed;

                        crossWall = new Polygon();
                        crossWall.Points.Add(new Point(p1x, p1y));
                        crossWall.Points.Add(new Point(p1x, p1y - width / 2));
                        crossWall.Points.Add(new Point(p1x - offset, p1y - width / 2));
                        crossWall.Points.Add(new Point(p1x - offset, p1y));

                        crossWall.Fill = cWLSUImage; 
                        crossWall.Stroke = Brushes.White;
                        crossWall.StrokeThickness = 3;

                        mCanvas.Children.Add(crossWall);
                        mCanvas.Children.Add(leftWall);
                        
                        deadEndLeft = false;
                    }

                    // Erzeugung von Wand bzw. Abzweigung rechts
                    if (rightTile.Typ == Tile.TileTyp.WALL) 
                    {
                        rightWall.Points.Add(new Point(p2x, p2y - width / 2));
                        rightWall.Points.Add(new Point(p2x, p2y));

                        rightWall.Fill = rWuImage;
                        rightWall.Stroke = Brushes.White;
                        rightWall.StrokeThickness = 3;

                        mCanvas.Children.Add(rightWall);
                        
                        deadEndRight = true;
                    }
                    else
                    {
                        rightWall.Points.RemoveAt(rightWall.Points.Count - 1);
                        rightWall.Points.Add(new Point(p2x + offset, p2y));
                        rightWall.Points.Add(new Point(p2x, p2y));

                        rightWall.Fill = Brushes.Black;
                        rightWall.Stroke = Brushes.White;
                        rightWall.StrokeThickness = 3;

                        if (rightTile.Typ == Tile.TileTyp.START)
                            rightWall.Fill = Brushes.DarkGreen;

                        if (rightTile.Typ == Tile.TileTyp.END)
                            rightWall.Fill = Brushes.DarkRed;

                        crossWall = new Polygon();
                        crossWall.Points.Add(new Point(p2x, p2y));
                        crossWall.Points.Add(new Point(p2x, p2y - width / 2));
                        crossWall.Points.Add(new Point(p2x + offset, p2y - width / 2));
                        crossWall.Points.Add(new Point(p2x + offset, p2y));

                        crossWall.Fill = cWRSUImage; 
                        crossWall.Stroke = Brushes.White;
                        crossWall.StrokeThickness = 3;

                        mCanvas.Children.Add(crossWall);
                        mCanvas.Children.Add(rightWall);
                        
                        deadEndRight = false;
                    }
                }     
            }

            #region Spieler-Tile
            bckgCenter = new Polygon();

            bckgCenter.Points.Add(new Point(450 - 125, 1000));
            bckgCenter.Points.Add(new Point(450, 1000 - 125));
            bckgCenter.Points.Add(new Point(1450, 1000 - 125));
            bckgCenter.Points.Add(new Point(1450 + 125, 1000));

            if (rearTile.Typ == Tile.TileTyp.START)
                bckgCenter.Fill = Brushes.DarkGreen;
            else
                bckgCenter.Fill = Brushes.Black;

            bckgCenter.Stroke = Brushes.White;
            bckgCenter.StrokeThickness = 3;
            #endregion

            #region Seitenwände der Spieler-Tile
            bckgLeft = new Rectangle();
            bckgRight = new Rectangle();
            bottomLeft = new Polygon();
            bottomRight = new Polygon();
            
            // Bildhintergrund links
            bckgLeft.Height = 1000;
            bckgLeft.Width = 450;
            Canvas.SetLeft(bckgLeft, 0);
            Canvas.SetTop(bckgLeft, 0);

            bckgLeft.Fill = cWLSImage;

            if (rearLeftTile.Typ == Tile.TileTyp.WALL)
                bckgLeft.Fill = wallLeftBkgImage;

            //
            bottomLeft.Points.Add(new Point(450 - 125, 1000));
            bottomLeft.Points.Add(new Point(450, 1000 - 125));
            bottomLeft.Points.Add(new Point(0, 1000 - 125));
            bottomLeft.Points.Add(new Point(0, 1000));

            bottomLeft.Fill = Brushes.Transparent;

            if (rearLeftTile.Typ == Tile.TileTyp.START)
                bottomLeft.Fill = Brushes.DarkGreen;
            if (rearLeftTile.Typ == Tile.TileTyp.END)
                bottomLeft.Fill = Brushes.DarkRed;
            if (rearLeftTile.Typ == Tile.TileTyp.PATH)
                bottomLeft.Fill = Brushes.Black;

            // Bildhintergrund rechts
            bckgRight.Height = 1000;
            bckgRight.Width = 450;
            Canvas.SetLeft(bckgRight, 1450);
            Canvas.SetTop(bckgRight, 0);
            
            bckgRight.Fill = Brushes.Black;
            bckgRight.Fill = cWRSImage;

            if (rearRightTile.Typ == Tile.TileTyp.WALL)
                bckgRight.Fill = wallRightBkgImage;

            //
            bottomRight.Points.Add(new Point(1450 - 125, 1000));
            bottomRight.Points.Add(new Point(1450, 1000 - 125));
            bottomRight.Points.Add(new Point(1900, 1000 - 125));
            bottomRight.Points.Add(new Point(1900, 1000));

            bottomRight.Fill = Brushes.Transparent;

            if (rearRightTile.Typ == Tile.TileTyp.START)
                bottomRight.Fill = Brushes.DarkGreen;
            if (rearRightTile.Typ == Tile.TileTyp.END)
                bottomRight.Fill = Brushes.DarkRed;
            if (rearRightTile.Typ == Tile.TileTyp.PATH)
                bottomRight.Fill = Brushes.Black;
            #endregion

            mCanvas.Children.Add(bckgLeft);
            mCanvas.Children.Add(bckgRight);
            mCanvas.Children.Add(bottomLeft);
            mCanvas.Children.Add(bottomRight);
            mCanvas.Children.Add(bckgCenter);

            mCanvas.Width = 1900;
            mCanvas.Height = 1000;
            Application.Current.MainWindow.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && aC.bEndCart)
            {
                loadGameMenu();
                aC.bEndCart = false;
            }
            else if (e.Key == Key.P && b3D_Active == true)
            {
                mCanvas.Children.Clear();
                aC.showTilesToCanvas(mCanvas);
                aC.fillPacmanRect(mCanvas);
                aC.showPacmanToCanvas(mCanvas);
                aC.resizePlayground(mCanvas);
                b3D_Active = false;
            }
            else if (e.Key == Key.P && b3D_Active != true && aC.bEndCart == false)
            {
                mCanvas.Children.Clear();
                b3D_Active = true;
                view3D();
            }
            else if(e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left)
                aC.movePlayer(e.Key, mCanvas, b3D_Active);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mCanvas.Children.Clear();
            aC.GameSetUP(mCanvas,cbDifficulty);
        }


    }
}
