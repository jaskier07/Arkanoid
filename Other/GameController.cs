using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;

namespace Arkanoid
{
    enum BlockTypes
    {
        none,
        grass,
        ice,
        brick,
        stone,
        metal
    }
    enum GameStates
    {
        menu,
        game,
        pause,
        win,
        failure
    }

    enum Bonuses
    {
        increaseLives,
        decreaseLives,
        swapArrowKeys,
        speedUpBall,
        slowDownBall,
        openChest,
        closeChest
    }

   class GameController
    {
        //rozszerzenia zserializowanych plików, odpowiednio plik nagłówka i plik z danymi bloków
        public const string dataExtension = "_dat.xml";
        public const string blockExtension = "_blc.xml";
        //czas odświeżenia timera w ms
        public const int timePerFrame = 20;
        //wymiary bloku
        public static int blockWidth;
        public static int blockHeight;
        //długość skrzyni, serca
        public const int chestWidth = 150;
        private const int heartWidth = 50;
        //odstęp pomiędzy wyświetlanymi życiami
        private const int livesOffset = 10;

        //listy bloków, aktywnych bonusów, wyświetlanych żyć
        private List<Block> blocks;
        private List<Bonus> activeBonuses;
        private List<Canvas> imgHearts;
        
        //licznik odpowiadający za główną pętlę programu
        private DispatcherTimer timer;
        //rysowanie obrazków na ekranie
        private ImageBrush imgBrush;
        //generator liczb losowych
        private Random rnd;
        //obecny stan gry
        public int gameState;
        //obecna liczba żyć gracza
        private int lives;
        //nazwa obecnego poziomu
        public string filename { get; set; }

        private MainWindow win;
        private Canvas canvasGame;
        private Paddle paddle;
        private Ball ball;
        private Chest chest;


        //-----------------------------------MECHANIKA GRY-----------------------------------

        public GameController(MainWindow win, ref Canvas canvasGame)
        {
            this.win = win;
            this.canvasGame = canvasGame;

            //ustalenie wielkości bloków
            blockWidth = MainWindow.screenWidth / MainWindow.gridColumns;
            blockHeight = (MainWindow.screenHeight - 200) / MainWindow.gridRows;

            imgHearts = new List<Canvas>();
            filename = "defaultLevel";

            //inicjalizacja listy bloków, paletki, piłki, generatora liczb losowych
            blocks = new List<Block>();
            paddle = new Paddle(this, win.platformRect);
            ball = new Ball(this, win.ballCircle, paddle, blocks);
            rnd = new Random();

            //usunięcie chwilowo paletki i piłki z listy dzieci canvas - nie muszą być wyświetlane w menu
            canvasGame.Children.Remove(paddle.shape);
            canvasGame.Children.Remove(ball.shape);

            //inicjalizacja timera
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameLoop);
            timer.Interval = new TimeSpan(0, 0, 0, 0, timePerFrame);

            gameState = (int)GameStates.menu;
        }

        public void StartNewLevel()
        {
            //zatrzymanie licznika, na końcu funkcji jego wznowienie
            if (timer.IsEnabled)
            {
                timer.Stop();
            }

            //inicjalizacja bonusów
            InitializeChestAndBonuses();

            //dodanie ich do elementów gry
            canvasGame.Children.Add(paddle.shape);
            canvasGame.Children.Add(ball.shape);

            //przesunięcie paletki i piłki do pozycji domyślnej
            paddle.MoveToDefaultPosition();
            ball.MoveToDefaultPosition();


            //ustawienie żyć
            LoadLives();
            DrawLives();

            StartTimer();
        }


        private void DrawAndMoveBonuses()
        {
            Bonus bonusToRemove = null;
            foreach (Bonus bonus in activeBonuses)
            {
                bonus.Move();
                //jeśli bonus jest aktywny, rysujemy go
                if (bonus.isActive)
                {
                    bonus.Draw();
                }
                //jeśli nie - po przeiterowaniu przez listę zostanie usunięty
                else
                {
                    bonusToRemove = bonus;
                }
            }
            if (bonusToRemove != null)
            {
                RemoveBonus(bonusToRemove);
            }
        }

        public void HitBlock(Block block)
        {
            //zmniejszenie wytrzymałości bloku
            block.strength -= 1;
            //blok zostaje zbity i znika z planszy
            if (block.strength == 0)
            {
                //blok zlewa się z tłem
                block.changeGridRowColor();
                //usunięcie bloku z listy bloków
                blocks.Remove(block);
                block.Draw();

                //jeśli blok zawierał bonus - zostaje on uaktywniony
                if (block.hasBonus())
                {
                    canvasGame.Children.Add(block.bonus.shape);
                    activeBonuses.Add(block.bonus);
                    block.bonus.isActive = true;
                }
            }

            //jeśli wszystkie bloki zostały zbite - następuje wygrana
            if (blocks.Count == 0)
            {
                Win();
            }
        }

        private void DecreaseLives()
        {
            lives--;
            //jeśli wciąż są życia - następuje ponowne ich narysowanie
            if (lives > 0)
            {
                canvasGame.Children.Remove(imgHearts.Last());
                imgHearts.Remove(imgHearts.Last());
                DrawLives();
            }
            //przegrana
            else
            {
                timer.Stop();
                ClearVisibleElements();
                gameState = (int)GameStates.failure;
                win.ShowMenuFailure();
            }
        }

        private void ClearVisibleElements()
        {
            //usunięcie elementów, które przeszkadzałyby w prawidłowym wyświetleniu menu głównego / wygranej / porażki
            canvasGame.Children.Remove(paddle.shape);
            canvasGame.Children.Remove(ball.shape);
            canvasGame.Children.Remove(chest.shape);

            foreach (Block block in blocks)
            {
                canvasGame.Children.Remove(block.shape);
            }
            foreach (Bonus activeBonus in activeBonuses)
            {
                canvasGame.Children.Remove(activeBonus.shape);
            }
            foreach (Canvas life in imgHearts)
            {
                canvasGame.Children.Remove(life);
            }
        }

        private void DrawLives()
        {
            for (int i = 0; i < lives; i++)
            {
                Canvas.SetTop(imgHearts[i], MainWindow.screenHeight - heartWidth - 30);
                Canvas.SetLeft(imgHearts[i], livesOffset + i * (heartWidth + livesOffset));
            }
        }

        //-----------------------------------DZIAŁANIE BONUSÓW-----------------------------------
        public void IncreaseLivesBonus()
        {
            win.bonusName.Header = "Szczęście: +1 życie!";
            lives += 1;
            LoadLives();
            DrawLives();
        }
         
        public void DecreaseLivesBonus()
        {
            //jeśli gracz miał jedno życie, to pozostaje przy jednym życiu
            if (lives != 1)
            {
                win.bonusName.Header = "Pech: -1 życie!";
                lives -= 1;
                LoadLives();
                DrawLives();
            }
            else
            {
                win.bonusName.Header = "Łaska: życie nie zostało Ci odebrane.";
            }
        }

        public void SwapArrowKeysBonus()
        {
            win.bonusName.Header = "Pech: Zamieniono klawisze!";
            paddle.SwapArrowKeys();
        }

        public void SpeedUpBallBonus()
        {
            win.bonusName.Header = "Pech: Przyspieszono piłkę!";
            ball.Speed += 0.2;
        }

        public void SlowDownBallBonus()
        {
            win.bonusName.Header = "Szczęście: spowolniono piłkę!";
            ball.Speed -= 0.2;
        }

        public void OpenChestBonus()
        {
            if (isChestClosed())
            {
                win.bonusName.Header = "Szczęście: otworzono skrzynię zwycięstwa!";
                chest.Open();
            }
            else
            {
                win.bonusName.Header = "Szczęściarz: skrzynia pozostaje otwarta!";
            }
        }

        public void CloseChestBonus()
        {
            if (isChestClosed())
            {
                win.bonusName.Header = "Pocieszenie: skrzynia i tak była zamknięta.";
            }
            else
            {
                win.bonusName.Header = "Pech: zamknięto skrzynię zwycięstwa!";
                chest.Close();
            }
        }

        public bool isChestClosed()
        {
            return chest.isClosed;
        }

        public void RemoveBonus(Bonus bonus)
        {
            canvasGame.Children.Remove(bonus.shape);
            activeBonuses.Remove(bonus);
        }

        //-----------------------------------STAN GRY-----------------------------------


        public void LifeLost()
        {
            DecreaseLives();
            ball.MoveToDefaultPosition();
            paddle.MoveToDefaultPosition();
        }

        public void Win()
        {
            timer.Stop();
            ClearVisibleElements();
            gameState = (int)GameStates.win;
            win.ShowMenuWin();
        }

        private void StartTimer()
        {
            timer.Start();
            gameState = (int)GameStates.game;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            paddle.Move();
            ball.Move();
            ball.Draw();
            paddle.Draw();
            DrawAndMoveBonuses();
        }

        public void ResumeGame()
        {
            timer.Start();
            gameState = (int)GameStates.game;
        }

        public void PauseGame()
        {
            timer.Stop();
            gameState = (int)GameStates.pause;
        }

        //-----------------------------------INICJALIZACJA ŚWIATA GRY-----------------------------------
         
        
        private void LoadLives()
        {
            //usunięcie żyć z canvas
            foreach (Canvas imgHeart in imgHearts)
            {
                canvasGame.Children.Remove(imgHeart);
            }
            //wyczyszczenie listy żyć i zapełnienie jej na nowo
            imgHearts.Clear();
            imgBrush = new ImageBrush(MainWindow.bmpHeart);

            for (int i = 0; i < lives; i++)
            {
                Canvas imgHeart = new Canvas();
                imgHeart.Width = heartWidth;
                imgHeart.Height = heartWidth - 5;
                imgHeart.Background = imgBrush;

                imgHearts.Add(imgHeart);
                canvasGame.Children.Add(imgHeart);
                Canvas.SetZIndex(imgHeart, 5);
            }
        }

        public void Deserialize(ref List<SerializableBlock> serializedBlocks)
        {

            blocks.Clear();
            //tutaj zostaną wpisane liczby dotyczące liczby różnego rodzaju bloków oraz liczba żyć przewidzianych na etap
            int[] header = new int[MainWindow.blockTypes + 1];

            XmlSerializer deserializer = new XmlSerializer(typeof(int[]));
            //deserializacja nagłówka
            using (TextReader reader = new StreamReader(MainWindow.pathToLevels + filename + dataExtension))
            {
                header = (int[])deserializer.Deserialize(reader);
            }
            lives = header[MainWindow.blockTypes];

            deserializer = new XmlSerializer(typeof(List<SerializableBlock>));
            //deserializacja bloków
            using (TextReader reader = new StreamReader(MainWindow.pathToLevels + filename + blockExtension))
            {
                serializedBlocks = (List<SerializableBlock>)deserializer.Deserialize(reader);
            }
        }
        public Shape CreateBlock(int gridx, int gridy, int color)
        {
            Block block = null;

            switch (color)
            {
                case (int)BlockTypes.grass:
                    block = new GrassBlock(gridx, gridy, blockWidth, blockHeight);
                    break;
                case (int)BlockTypes.ice:
                    block = new IceBlock(gridx, gridy, blockWidth, blockHeight);
                    break;
                case (int)BlockTypes.brick:
                    block = new BrickBlock(gridx, gridy, blockWidth, blockHeight);
                    break;
                case (int)BlockTypes.stone:
                    block = new StoneBlock(gridx, gridy, blockWidth, blockHeight);
                    break;
                case (int)BlockTypes.metal:
                    block = new MetalBlock(gridx, gridy, blockWidth, blockHeight);
                    break;
            }

            blocks.Add(block);
            return block.shape;
        }


        private void InitializeChestAndBonuses()
        {
            //inicjalizacja skrzyni
            chest = new Chest();
            canvasGame.Children.Add(chest.shape);

            activeBonuses = new List<Bonus>();
            //ustawienie domyślnej wartości dla panelu wyświetlającego informacje o ostatnio użytym bonusie
            win.bonusName.Header = "Brak aktywnych bonusów";

            //maksymalna liczba bonusów, jakie mogą pojawić się na planszy
            int bonusesMaxAmount = blocks.Count / 3;
            int bonusesMinAmount = blocks.Count / 6;

            //wylosowanie liczby bonusów z powyższego zakresu
            int bonuses = rnd.Next(bonusesMinAmount, bonusesMaxAmount);

            //dla każdego bonusa wylosowanie jego rodzaju i bloku, do którego zostanie przypisany
            for (int i = 0; i < bonuses; i++)
            {
                bool blockChosen = false;
                int blockNr = 0;
                //losowanie numeru takiego bloku, który jeszcze nie zawiera bonusa
                //jeden blok może mieć co najwyżej jeden bonus
                while (!blockChosen)
                {
                    blockNr = rnd.Next(0, blocks.Count);
                    if (!blocks[blockNr].hasBonus())
                    {
                        blockChosen = true;
                    }
                }
                //wylosowanie bonusu z określonym prawdopodobieństwem
                int randBonus = DrawRandomBonusNr();

                CreateBonus(randBonus, blockNr);
            }
        }

        private int DrawRandomBonusNr()
        {

            double rand = rnd.NextDouble();
            //wylosowanie bonusu z określonym prawdopodobieństwem
            if (rand >= 0.9) return (int)Bonuses.openChest;             //10%
            else if (rand >= 0.8) return (int)Bonuses.closeChest;       //10%
            else if (rand >= 0.7) return (int)Bonuses.swapArrowKeys;    //10%
            else if (rand >= 0.5) return (int)Bonuses.speedUpBall;      //20%
            else if (rand >= 0.3) return (int)Bonuses.slowDownBall;     //20%
            else if (rand >= 0.15) return (int)Bonuses.increaseLives;   //15%
            else return (int)Bonuses.decreaseLives;                     //15%
        }

        private void CreateBonus(int randBonus, int blockNr)
        {
            Bonus bonus = null;

            switch (randBonus)
            {
                case (int)Bonuses.increaseLives:
                    bonus = new IncreaseLivesBonus(this, blocks[blockNr].pos, paddle);
                    break;
                case (int)Bonuses.decreaseLives:
                    bonus = new DecreaseLivesBonus(this, blocks[blockNr].pos, paddle);
                    break;
                case (int)Bonuses.swapArrowKeys:
                    bonus = new SwapArrowKeysBonus(this, blocks[blockNr].pos, paddle);
                    break;
                case (int)Bonuses.speedUpBall:
                    bonus = new SpeedUpBonus(this, blocks[blockNr].pos, paddle);
                    break;
                case (int)Bonuses.slowDownBall:
                    bonus = new SlowDownBonus(this, blocks[blockNr].pos, paddle);
                    break;
                case (int)Bonuses.openChest:
                    bonus = new OpenChestBonus(this, blocks[blockNr].pos, paddle);
                    break;
                case (int)Bonuses.closeChest:
                    bonus = new CloseChestBonus(this, blocks[blockNr].pos, paddle);
                    break;
            }
            //powiązanie bonusu z blokiem
            blocks[blockNr].bonus = bonus;
        }
    }
}
