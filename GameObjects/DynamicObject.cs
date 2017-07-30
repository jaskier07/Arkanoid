using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    //Klasa opisująca poruszające się obiekty.
    abstract class DynamicObject : GameObject
    {
        public const double defaultBallSpeed = 0.7;
        public const double defaultPaddleSpeed = 22;
        public const double defaultBonusSpeed = 10;
        //prędkość początkowa
        public double defaultSpeed;

        protected double _speed;
        public double Speed
        {
             get { return _speed; }
             set { _speed = value; }
        }
        //pozycja domyślna - tu znajdzie się obiekt po rozpoczęciu gry
        public Position defaultPos { get; set; }
        //ruch
        public abstract void Move();
        //kolizja
        public abstract void Collision();
    }
}
