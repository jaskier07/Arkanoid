using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Arkanoid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //wymiary
        //wysokość i długość ekranu gry
        public const int screenHeight = 900;
        public const int screenWidth = 700;
        //wysokość belki menu
        public const int menuHeight = 25;
        //ilość kolumn i rzędów siatki bloków
        public const int gridColumns = 10;
        public const int gridRows = 25;
        //wymiary combo boxa
        private int cBoxWidth = 300;
        private int cBoxHeight = 30;
        //wysokość, do której sięga siatka bloków
        public static int blockGridHeight;
        //liczba kolorów
        public static int blockTypes = Enum.GetNames(typeof(BlockTypes)).Length;
        private int titleLabelWidth;

        //kolory
        public static SolidColorBrush whiteColor;
        public static SolidColorBrush bgColor;
        public static SolidColorBrush blueColor;
        public static SolidColorBrush greenColor;
        public static SolidColorBrush redColor;
        public static SolidColorBrush yellowColor;
        public static SolidColorBrush yellowGreenColor;

        //nazwy obrazków
        public const string imgWinName = "bgwin.png";
        public const string imgFailureName = "bgfailure.png";
        public const string imgGameBgName = "bggame.png";
        public const string imgMenuName = "bgmenu.png";
        public const string imgPaddleRedName = "paddleRed.png";
        public const string imgHeartName = "heart.png";
        public const string imgBonusName = "bonus.png";
        public const string imgClosedChestName = "closedChest.png";
        public const string imgOpenedChestName = "openChest.png";
        public const string imgGrassName = "grass.png";
        public const string imgIceName = "ice.png";
        public const string imgStoneName = "stone.png";
        public const string imgMetalName = "metal.png";
        public const string imgBrickName = "brick.png";
        public const string imgPaddleName = "paddle.png";

        //bitmapy obrazków
        public static BitmapImage bmpImageFailure;
        public static BitmapImage bmpImageMenu;
        public static BitmapImage bmpImageGame;
        public static BitmapImage bmpImageWin;
        public static BitmapImage imgBonus;
        public static BitmapImage imgPaddleRed;
        public static BitmapImage imgClosedChest;
        public static BitmapImage imgOpenChest;
        public static BitmapImage bmpHeart;
        public static BitmapImage imgGrass;
        public static BitmapImage imgIce;
        public static BitmapImage imgStone;
        public static BitmapImage imgMetal;
        public static BitmapImage imgBrick;
        public static BitmapImage imgPaddle;

        //ścieżki
        public static string pathToLevels;
        public static string pathToImages;
        public static string projectDir;

        //elementy UI
        private Rectangle bgRectangle;
        private ComboBox cBoxLevels;
        private Label infoLabel;
        private Label titleLabel;
        private FontFamily cambria;

        private List<string> levelNames;
        private GameController gc;

        public MainWindow()
        {            
            InitializeComponent();
            InitializeColors();
            InitializeDockPanel();
            LoadPathsAndImages();

            gc = new GameController(this, ref canvasGame);

            InitializeBackground();
            FindLevels();
            InitializeMenuElements();
            ShowMenuElements();
        }



        public void FindLevels()
        {
            menuLevels.Items.Clear();
            levelNames = new List<string>();
            string[] paths = Directory.GetFiles(MainWindow.pathToLevels);

            foreach (string path in paths)
            {
                //znajduje wszystkie nazwy plików poziomów w katalogu levels (.xml)
                if (path.EndsWith(GameController.dataExtension))
                {
                    //obcięcie ścieżki, rozszerzenia
                    string levelName = System.IO.Path.GetFileNameWithoutExtension(path);
                    levelName = levelName.Substring(0, levelName.Length - 4);
                    //dodanie nowego elementu menu
                    MenuItem menuLevel = new MenuItem();
                    menuLevel.Header = levelName;
                    menuLevel.Click += HandleLevelClick;
                    menuLevel.FontSize = 13;
                    
                    menuLevels.Items.Add(menuLevel);
                    levelNames.Add(levelName);
                }
            }
        }

        //-----------------------------------EVENT HANDLERY-----------------------------------

        private void StartNewGame(object sender, EventArgs e)
        {
            //wczytanie bloków
            canvasGame.Children.Clear();
            List<SerializableBlock> serializedBlocks = new List<SerializableBlock>();
            gc.Deserialize(ref serializedBlocks);

            foreach (SerializableBlock block in serializedBlocks)
            {
                canvasGame.Children.Add(gc.CreateBlock(block.gridx, block.gridy, block.blockType));
            }
            gc.gameState = (int)GameStates.game;

            DrawBackground();
            //rozpoczęcie gry w Game Controllerze
            gc.StartNewLevel();
        }

        private void HandleLevelClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            gc.filename = (string)item.Header;

            StartNewGame(sender, e);
        }

        private void ComboBoxSelectionHandler(object sender, SelectionChangedEventArgs e)
        {
            //jeśli wcześniej nie została zaznaczona żadna opcja
            if (cBoxLevels.SelectedItem != null)
            {
                //znalezienie nazwy pliku, bez ścieżki
                gc.filename = cBoxLevels.SelectedValue.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
                //odznaczenie zaznaczonego poziomu
                cBoxLevels.SelectedItem = null;
                //usunięcie elementów menu i rozpoczecie nowej gry
                canvasGame.Children.Remove(cBoxLevels);
                canvasGame.Children.Remove(infoLabel);

                StartNewGame(sender, e);
            }
        }

        private void HandleKeydown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                //otworzenie edytora poziomów
                case Key.E:
                    OpenLevelEditor(sender, e);
                    break;
                //pauza/wznowienie gry
                case Key.P:
                    switch (gc.gameState)
                    {
                        case (int)GameStates.game:
                            gc.PauseGame();
                            break;
                        case (int)GameStates.pause:
                            gc.ResumeGame();
                            break;
                    }
                    break;
                //nowa gra
                case Key.N:
                    StartNewGame(sender, e);                    
                    break;
            }
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenLevelEditor(object sender, EventArgs e)
        {
            LevelEditorWindow winLevelEditor = new LevelEditorWindow(this, gridColumns, gridRows, GameController.blockWidth, GameController.blockHeight, blockGridHeight + GameController.blockHeight);
            winLevelEditor.Show();
            gc.PauseGame();
        }

        //-----------------------------------WYŚWIETLENIE RÓŻNYCH MENU-----------------------------------
        
        private void ShowMenuElements()
        {
            //wyczyszczenie poprzedniej listy poziomów i utworzenie nowej
            cBoxLevels.Items.Clear();

            foreach (string levelName in levelNames)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = levelName;
                item.FontFamily = cambria;
                cBoxLevels.Items.Add(item);
            }

            //wyświetlenie combo boxa
            canvasGame.Children.Add(cBoxLevels);
            Canvas.SetTop(cBoxLevels, screenHeight / 2);
            Canvas.SetLeft(cBoxLevels, (screenWidth - cBoxWidth) / 2);

            //utworzenie belki wyboru poziomu        
            infoLabel.Content = "Wybierz poziom";
            canvasGame.Children.Add(infoLabel);
            Canvas.SetTop(infoLabel, screenHeight / 2 - 50);
            Canvas.SetLeft(infoLabel, (screenWidth - cBoxWidth) / 2);
        }

        public void ShowMenuWin()
        {
            DrawBackground();
            ShowMenuElements();

            infoLabel.Foreground = whiteColor;
            titleLabel.Content = "WYGRAŁEŚ!";
            canvasGame.Children.Add(titleLabel);
            Canvas.SetTop(titleLabel, screenHeight / 2 - 140);
            Canvas.SetLeft(titleLabel, (screenWidth - titleLabelWidth) / 2);
        }

        public void ShowMenuFailure()
        {
            DrawBackground();
            ShowMenuElements();

            infoLabel.Foreground = whiteColor;
            titleLabel.Content = "PRZEGRAŁEŚ!";
            canvasGame.Children.Add(titleLabel);
            Canvas.SetTop(titleLabel, screenHeight / 2 - 140);
            Canvas.SetLeft(titleLabel, (screenWidth - titleLabelWidth) / 2);
        }

        private void DrawBackground()
        {
            //jeśli wcześniej występowało tło, zostaje ono usunięte
            if (canvasGame.Children.Contains(bgRectangle))
            {
                canvasGame.Children.Remove(bgRectangle);
            }

            switch (gc.gameState)
            {
                case (int)GameStates.menu:
                    bgRectangle.Fill = new ImageBrush
                    {
                        ImageSource = bmpImageMenu,
                    };
                    break;
                case (int)GameStates.game:
                    bgRectangle.Fill = new ImageBrush
                    {
                        ImageSource = bmpImageGame,
                    };
                    break;
                case (int)GameStates.failure:
                    bgRectangle.Fill = new ImageBrush
                    {
                        ImageSource = bmpImageFailure,
                    };
                    break;
                case (int)GameStates.win:
                    bgRectangle.Fill = new ImageBrush
                    {
                        ImageSource = bmpImageWin,
                    };
                    break;
            }

            canvasGame.Children.Add(bgRectangle);
            Canvas.SetTop(bgRectangle, 0);
            Canvas.SetLeft(bgRectangle, 0);
        }

        //-----------------------------------INICJALIZACJA OKNA I JEGO PÓL-----------------------------------

        private void InitializeColors()
        {
            bgColor = new SolidColorBrush(Color.FromRgb(47, 45, 54));
            blueColor = new SolidColorBrush(Colors.CornflowerBlue);
            greenColor = new SolidColorBrush(Colors.GreenYellow);
            redColor = new SolidColorBrush(Colors.DarkRed);
            yellowColor = new SolidColorBrush(Colors.Goldenrod);
            yellowGreenColor = new SolidColorBrush(Colors.YellowGreen);
            whiteColor = new SolidColorBrush(Colors.White);
        }

        private void InitializeBlocks()
        {
            //ustalenie miejsca, w którym kończy się siatka
            blockGridHeight = GameController.blockHeight * gridRows;
            //deserializacja poziomu i odtworzenie położenia bloków
            List<SerializableBlock> serializedBlocks = new List<SerializableBlock>();

            gc.Deserialize(ref serializedBlocks);

            foreach (SerializableBlock block in serializedBlocks)
            {
                 canvasGame.Children.Add(gc.CreateBlock(block.gridx, block.gridy, block.blockType));
            }
        }

        private void LoadPathsAndImages()
        {            
            projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            pathToImages = projectDir + "\\img\\";
            pathToLevels = projectDir + "\\levels\\";

            bmpHeart = new BitmapImage(new Uri(pathToImages + imgHeartName, UriKind.Relative));
            imgBonus = new BitmapImage(new Uri(pathToImages + imgBonusName, UriKind.Relative));
            imgPaddleRed = new BitmapImage(new Uri(pathToImages + imgPaddleRedName, UriKind.Relative));
            imgClosedChest = new BitmapImage(new Uri(pathToImages + imgClosedChestName, UriKind.Relative));
            imgOpenChest = new BitmapImage(new Uri(pathToImages + imgOpenedChestName, UriKind.Relative));
            imgPaddle = new BitmapImage(new Uri(pathToImages + imgPaddleName, UriKind.Relative));

            imgBrick = new BitmapImage(new Uri(pathToImages + imgBrickName, UriKind.Relative));
            imgGrass = new BitmapImage(new Uri(pathToImages + imgGrassName, UriKind.Relative));
            imgIce = new BitmapImage(new Uri(pathToImages + imgIceName, UriKind.Relative));
            imgStone = new BitmapImage(new Uri(pathToImages + imgStoneName, UriKind.Relative));
            imgMetal = new BitmapImage(new Uri(pathToImages + imgMetalName, UriKind.Relative));
        }

        private void InitializeBackground()
        {
            bgRectangle = new Rectangle();
            bmpImageGame = new BitmapImage(new Uri(pathToImages + imgGameBgName, UriKind.Relative));
            bmpImageMenu = new BitmapImage(new Uri(pathToImages + imgMenuName, UriKind.Relative));
            bmpImageFailure = new BitmapImage(new Uri(pathToImages + imgFailureName, UriKind.Relative));
            bmpImageWin = new BitmapImage(new Uri(pathToImages + imgWinName, UriKind.Relative));
            bgRectangle.Height = bmpImageGame.PixelHeight;
            bgRectangle.Width = bmpImageGame.PixelWidth;
            Canvas.SetZIndex(bgRectangle, 0);

            DrawBackground();
        }

        private void InitializeMenuElements()
        {
            titleLabelWidth = cBoxWidth + 40;
            cambria = new FontFamily("Cambria");

            cBoxLevels = new ComboBox();
            cBoxLevels.Width = cBoxWidth;
            cBoxLevels.Height = cBoxHeight;
            cBoxLevels.Background = yellowGreenColor;
            cBoxLevels.SelectionChanged += ComboBoxSelectionHandler;

            infoLabel = new Label();
            infoLabel.Width = cBoxWidth;
            infoLabel.Height = 50;
            infoLabel.FontSize = 30;
            infoLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            infoLabel.FontFamily = cambria;
            Canvas.SetZIndex(infoLabel, 2);

            titleLabel = new Label();
            titleLabel.Width = titleLabelWidth;
            titleLabel.Height = 80;
            titleLabel.FontSize = 50;
            titleLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            titleLabel.Foreground = whiteColor;
            titleLabel.FontFamily = cambria;
            Canvas.SetZIndex(infoLabel, 2);
        }

        private void InitializeDockPanel()
        {
            myDockPanel.Width = screenWidth;
            myDockPanel.Height = screenHeight;

            DockPanelTop.Background = Brushes.YellowGreen;
            DockPanelTop.Height = menuHeight;

            MainMenu.Background = yellowGreenColor;
        }
    }
}
