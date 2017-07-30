using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Arkanoid
{
    //Piłka. Gracz odbijając ją przy pomocy paletki zbija bloki. W zależności od tego, w którym miejscu paletki upadnie,
    //odbija się pod innym kątem.
    class Ball : DynamicObject
    {
        private double vx = 0;
        private double vy = 0;
        private Paddle paddle;
        private List<Block> blocks;

        private double bounceAngle;
        private double speedx;
        private double speedy;
        private int r;
        new public double Speed
        {
            get { return _speed; }
            set
            {
                //prędkość piłki zmienia się w trakcie gry, ale nigdy nie będzie większa niż 2 i mniejsza niż 0.5
                if (value <= 0.5) _speed = 0.5;
                else if (value >= 2) _speed = 2;
                else _speed = value;

                if (speedx > 0) speedx = Speed;
                else speedx = -Speed;

                if (speedy > 0) speedy = Speed;
                else speedy = -Speed;
            }
        }

        public Ball(GameController gc, Ellipse ball, Paddle paddle, List<Block> blocks)
        {
            width = 25;
            height = width;
            r = width / 2;
            defaultSpeed = defaultBallSpeed;
            Speed = defaultSpeed;
            //początkowo piłka powinna poruszać się pionowo do góry
            //normalizedRel = 0, więc vy = 1, vx = 0
            //wektor będzie więc skierowany przeciwnie (-1), czyli w górę
            speedx = Speed;
            speedy = -Speed;
            bounceAngle = degToRad(0);

            this.gc = gc;
            this.paddle = paddle;
            this.blocks = blocks;
            shape = ball;

            //piłka pojawi się w połowie planszy, na platformie
            defaultPos = new Position(MainWindow.screenWidth / 2, MainWindow.screenHeight - 100 - (paddle.height + width) / 2, width, height);
            pos = defaultPos;

            //inicjalizacja piłki
            shape.Height = height;
            shape.Width = width;
            shape.StrokeThickness = 1;
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.Fill = new SolidColorBrush(Colors.Tomato);
            Canvas.SetZIndex(shape, 3);           
        }

        public void MoveToDefaultPosition()
        {
            pos = defaultPos;

            Speed = defaultSpeed;
            speedx = Speed;
            speedy = -Speed;

            bounceAngle = degToRad(10);
        }

        public override void Collision()
        {
            bool was = false;
            //piłka porusza się w górę
            if (speedy < 0)
            {
                was = VerticalCollisionTop();
            }
            //piłka porusza się w dół
            else
            {
                was = VerticalCollisionBottom();
            }
            //jeśli już wystąpiła kolizja w pionie, to nastąpiło też odbicie - nie nastąpi więc kolizja w poziomie
            if (!was)
            {
                if (vx < 0)
                {
                    HorizontalCollisionLeft();
                }
                else
                {
                    HorizontalCollisionRight();
                }
            }
        }

        private bool FindCollidingBlock(int newPosx)
        {
            int newPosy = 0;

            //wyznaczenie wysokości y, na której znajdzie się piłka po wykonaniu ruchu
            if (speedx >= 0)
            {
                newPosy = pos.by() + (int)(vy * GameController.timePerFrame);
            }
            else
            {
                newPosy = pos.ty() + (int)(vx * GameController.timePerFrame);
            }

            bool blockFound = false;
            int gridx = newPosx / GameController.blockWidth;
            int gridy = newPosy / GameController.blockHeight;

            //sprawdzenie dla każdego bloku, czy piłka po wykonaniu ruchu wejdzie z nim w kolizję
            foreach (Block block in blocks)
            {
                if (gridx == block.grid.x && gridy == block.grid.y)
                {
                    //będzie kolizja - następuje odbicie w poziomie
                    BounceHorizontally(block);
                    blockFound = true;
                    break;
                }
            }
            //nie będzie kolizji, piłka porusza się dalej
            if (!blockFound)
            {
                MoveHorizontally();
                return false;
            }
            return true;
        }


        private bool HorizontalCollisionLeft()
        {
            int newPosx = pos.lx() + (int)(vx * GameController.timePerFrame);
            //jej lewa krawędź nie napotyka krawędzi planszy
            if (newPosx >= 0)
            {

                return FindCollidingBlock(newPosx);
            }
            //jej lewa krawędź napotka krawędź planszy - następuje odbicie
            else if (newPosx < 0)
            {
                speedx = -speedx;
                pos.x = r;
                return true;
            }
            return false;
        }

        private bool HorizontalCollisionRight()
        {
            int newPosx = pos.rx() + (int)(vx * GameController.timePerFrame);
            //jej prawa krawędź nie napotyka krawędzi planszy
            if (newPosx <= MainWindow.screenWidth)
            {
                return FindCollidingBlock(newPosx);
            }
            //jej prawa krawędź napotka krawędź planszy - następuje odbicie
            else if (newPosx > MainWindow.screenWidth)
            {
                speedx = -speedx;
                pos.x = MainWindow.screenWidth - r;
                return true;
            }

            return false;
        }

        private bool VerticalCollisionTop()
        {
            int newPosition = pos.ty() + (int)(vy * GameController.timePerFrame);
            //jej górna krawędź nie napotyka krawędzi planszy
            if (newPosition >= 0)
            {
                int gridy = newPosition / (GameController.blockHeight);
                bool blockFound = false;

                foreach (Block block in blocks)
                {
                    //szukam tylko tych bloków, które są w zasięgu piłki
                    if (block.grid.y == gridy && BallIntersectsObject(block))
                    {
                        BounceVertically(block);
                        gc.HitBlock(block);
                        blockFound = true;

                        return true;
                    }
                }
                //jeśli nie ma żadnego bloku na drodze piłki, leci dalej
                if (!blockFound)
                {
                    MoveVertically();
                    return false;
                }                
            }
            //jej górna krawędź napotka krawędź planszy - następuje odbicie
            else if (newPosition < 0)
            {
                speedy = -speedy;
                pos.y = r;
                return true;
            }
            return false;
        }


        private bool VerticalCollisionBottom()
        {
            int newPosition = pos.by() + (int)(vy * GameController.timePerFrame);
            //jej dolna krawędź nie napotyka krawędzi planszy
            if (newPosition <= MainWindow.screenHeight)
            {
                if (BallIntersectsObject(paddle))
                {
                    ChangeBounceAngle();
                    BounceVertically(paddle);

                    return true;
                }
                else
                {
                    int gridy = newPosition / (GameController.blockHeight);
                    bool blockFound = false;

                    foreach (Block block in blocks)
                    {
                        //szukam tylko tych bloków, które są w zasięgu piłki
                        if (block.grid.y == gridy && BallIntersectsObject(block))
                        {
                            BounceVertically(block);
                            gc.HitBlock(block);
                            blockFound = true;

                            return true;
                        }
                    }
                    //jeśli nie ma żadnego bloku na drodze piłki, leci dalej
                    if (!blockFound)
                    {
                        MoveVertically();
                        return false;
                    }
                }
            }
            //jej dolna krawędź napotka krawędź planszy
            else if (newPosition > MainWindow.screenHeight)
            {
                //jeśli piłka trafiła w skrzynię i jest ona otwarta - następuje wygrana
                //wygrana
                if (!gc.isChestClosed() && pos.x >= MainWindow.screenWidth - GameController.chestWidth)
                {
                    gc.Win();
                }
                //skrzynia zamknięta - następuje utrata życia
                else
                {
                    gc.LifeLost();
                }                
            }
            return false;
        }

        private bool BallIntersectsObject(GameObject obj)
        {
            double ballDistanceX = Math.Abs(this.pos.x - (obj.pos.x));
            double ballDistanceY = Math.Abs(this.pos.y - (obj.pos.y));

            if (obj is Paddle)
            {
                if (ballDistanceX > (obj.width / 2 + r)) return false;
                if (ballDistanceY > (obj.height / 2 + r)) return false;

                if (ballDistanceX <= obj.width / 2) return true;
                if (ballDistanceY <= obj.height / 2) return true;
            }
            else
            {
                if (ballDistanceX > (obj.width / 2 + r)) return false;
                if (ballDistanceY < (obj.height / 2 + r)) return false;

                if (ballDistanceX <= obj.width / 2) return true;
                if (ballDistanceY >= obj.height / 2) return true;
            }

            double cornerDistance = Math.Pow((ballDistanceX - (double)((double)obj.width / 2.0)), 2.0) + Math.Pow(ballDistanceY - (double)((double)obj.height / 2.0), 2.0);
            return (cornerDistance <= Math.Pow(r, 2.0));
        }
        private void MoveHorizontally()
        {
            pos.x += (int)(vx * GameController.timePerFrame);
        }

        private void MoveVertically()
        {
            pos.y += (int)(vy * GameController.timePerFrame);
        }

        private void BounceVertically(GameObject obj)
        {
            //jeśli porusza się w dół
            if (speedy >= 0)
            {
                pos.y = obj.pos.ty() - r;
            }
            //jeśli porusza się w górę
            else
            {
                pos.y = obj.pos.by() + r;
            }
            //odbicie
            speedy = -speedy;
        }

        private void BounceHorizontally(GameObject obj)
        {
            //jeśli porusza się w prawo
            if (vx >= 0)
            {
                pos.x = obj.pos.lx() - r;
            }
            //porusza się w lewo
            else
            {
                pos.x = obj.pos.rx() + r;
            }
            //odbicie
            speedx = -speedx;
        }

        private double degToRad(double deg)
        {
            return deg * Math.PI / 180;
        }
        private void ChangeBounceAngle()
        {
            double relIntersectX = -paddle.pos.x + this.pos.x;
            double normalizedRelX = relIntersectX / (paddle.width / 2);
            bounceAngle = -degToRad(normalizedRelX * 50);
        }

        public void CorrectVelocity()
        {
            //piłka poruszając się po skosie normalnie poruszałaby się szybciej; aby tego uniknąć, koryguję 
            //ręcznie jej prędkość
            if (Math.Abs(vx) > 0.85)
            {
                vx *= 0.6;
            }
            else if (Math.Abs(vx) > 0.7)
            {
                vx *= 0.65;
            }
            else if (Math.Abs(vx) > 0.55)
            {
                vx *= 0.75;
            }
            else if (Math.Abs(vx) > 0.4)
            {
                vx *= 0.9;
            }

        }

        public override void Draw()
        {
            Canvas.SetTop(shape, pos.ty());
            Canvas.SetLeft(shape, pos.lx());
        }

        public override void Move()
        {
            vx = speedx * (-Math.Sin(bounceAngle));
            vy = speedy * Math.Cos(bounceAngle);

            CorrectVelocity();
            Collision();
        }
    }
}
