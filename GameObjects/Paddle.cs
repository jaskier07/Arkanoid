using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Arkanoid
{
    //Paletka. Gracz steruje nią za pomocą strzałek odbijając piłkę.
    class Paddle : DynamicObject
    {
        //czy klawisze lewo-prawo zostały zamienione
        public bool arrowKeysSwapped;

        public Paddle(GameController gc, Rectangle rect)
        {
            width = 125;
            height = 25;
            this.gc = gc;

            defaultSpeed = defaultPaddleSpeed;
            Speed = defaultSpeed;
            arrowKeysSwapped = false;

            //platforma pojawi się w połowie planszy
            defaultPos = new Position(MainWindow.screenWidth / 2, MainWindow.screenHeight - 100, width, height);
            pos = defaultPos;

            //inicjalizacja prostokąta 
            bmpImage = MainWindow.imgPaddle;
            shape = rect;
            Canvas.SetZIndex(shape, 4);

            InitializeShape();
        }

        public void MoveToDefaultPosition()
        {
            pos = defaultPos;
            //przywraca, jeśli to konieczne, domyślne znaczenie klawiszy lewo/prawo
            if (arrowKeysSwapped) SwapArrowKeys();
            Draw();
        }

        public override void Collision()
        {
            
        }

        public override void Draw()
        {
            Canvas.SetTop(shape, pos.ty());
            Canvas.SetLeft(shape, pos.lx());
        }

        public override void Move()
        {
            //jeśli klawisze nie zostały zamienione, lewy klawisz powoduje ruch w lewo, prawy w prawo
            if (!arrowKeysSwapped)
            {
                if (LeftArrowKeyPressed())
                {
                    MoveLeft();
                }
                if (RightArrowKeyPressed())
                {
                    MoveRight();
                }
            }
            //jeśli klawisze zostały zamienione, lewy klawisz powoduje ruch w prawo, prawy w lewo
            else
            {
                if (LeftArrowKeyPressed())
                {
                    MoveRight();
                }
                if (RightArrowKeyPressed())
                {
                    MoveLeft();
                }
            }
        }

        private void MoveLeft()
        {
            //ruch, jeśli lewa krawędź paletki po wykonaniu ruchu w lewo nie wyjdzie poza krawędź ekranu
            if (pos.lx() - Speed > 0)
            {
                pos.x -= (int)Speed;
            }
            //jeśli wyjdzie, paletka ustawiana jest tak, aby lewą krawędzią stykała się z krawędzią ekranu
            else
            {
                pos.x = width / 2;
            }
        }

        private void MoveRight()
        {
            //rucj, jeśli prawa krawędź paletki po wykonaniu ruchu w prawo nie wyjdzie poza krawędź ekranu
            if (pos.rx() + Speed < MainWindow.screenWidth)
            {
                pos.x += (int)Speed;
            }
            //jeśli wyjdzie, paletka ustawiana jest tak, aby prawą krawędzią stykała się z krawędzią ekranu
            else
            {
                pos.x = MainWindow.screenWidth - width / 2;
            }
        }

        private bool LeftArrowKeyPressed()
        {
            return (Keyboard.GetKeyStates(Key.Left) & KeyStates.Down) > 0;
        }

        private bool RightArrowKeyPressed()
        {
            return (Keyboard.GetKeyStates(Key.Right) & KeyStates.Down) > 0;
        }

        public void SwapArrowKeys()
        {
            if (arrowKeysSwapped)
            {
                arrowKeysSwapped = false;
                //zamienia obrazke paletki aby ułatwić spostrzeżenie zamiany klawiszy
                shape.Fill = new ImageBrush
                {
                    ImageSource = bmpImage
                };
            }
            else
            {
                arrowKeysSwapped = true;
                shape.Fill = new ImageBrush
                {
                    ImageSource = MainWindow.imgPaddleRed
                };
            }
        }
    }
}
