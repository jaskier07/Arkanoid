using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Arkanoid
{
    //Bonus. Po zbiciu bloku, może z niego wypaść bonus, który będzie spadał pionowo w dół. Jeśli gracz złapie go paletką,
    //bonus zostanie uaktywniony. Może zadziałać na gracza tak pozytywnie, jak i negatywnie. Rodzaj bonusu jest losowany
    //w GameControllerze. Po tej klasie dziedziczą klasy bonusów, różniących się od siebie działaniem.
    abstract class Bonus : DynamicObject
    {
        private Paddle paddle;
        //czy bonus jest aktywny
        public bool isActive;

        //funkcja działania bonusu
        public abstract void Work();

        public Bonus(GameController gc, Position blockCenter, Paddle paddle)
        {
            width = 50;
            height = 50;
            this.paddle = paddle;
            this.gc = gc;

            defaultSpeed = defaultBonusSpeed;
            Speed = defaultSpeed;

            defaultPos = blockCenter;
            pos = defaultPos;

            CreateRectangle();
        }

        private void CreateRectangle()
        {
            shape = new Rectangle();
            shape.Width = width;
            shape.Height = height;
            Canvas.SetZIndex(shape, 2);

            shape.Fill = new ImageBrush
            {
                ImageSource = MainWindow.imgBonus
            };
        }

        public void HideBonus()
        {
            isActive = false;
            gc.RemoveBonus(this);
        }

        public override void Collision()
        {
            isActive = false;
            Work();
        }

        public override void Draw()
        {
            Canvas.SetTop(shape, pos.ty());
            Canvas.SetLeft(shape, pos.x);
        }

        public override void Move()
        {
            int newPosTy = pos.ty() + (int)Speed;
            //jeśli bonus zetknie się z paletką - znika i zaczyna działać
            if (isPaddleAndBonusOnTheSameHeight() && (CollisionRightEdge() || CollisionLeftEdge()))
            {
                Collision();
            }
            //jeśli bonus wyszedł poza planszę - znika
            else if (newPosTy > MainWindow.screenHeight)
            {
                isActive = false;
            }
            //bonus nie napotkał przeszkody - leci w dół
            else
            {
                pos.y += (int)Speed;
            }
        }

        private bool CollisionRightEdge()
        {
            return pos.rx() > paddle.pos.lx() && pos.rx() < paddle.pos.rx();
        }

        private bool CollisionLeftEdge()
        {
            return pos.lx() > paddle.pos.lx() && pos.lx() < paddle.pos.rx();
        }

        private bool isPaddleAndBonusOnTheSameHeight()
        {
            return pos.by() >= paddle.pos.ty() && pos.ty() <= paddle.pos.by();
        }
    }
}
