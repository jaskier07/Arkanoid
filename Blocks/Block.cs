using System.Windows.Controls;
using System.Windows.Shapes;

namespace Arkanoid
{
    //Blok. Zadaniem gracza jest zbicie wszystkich bloków na planszy. Po tej klasie dziedziczą inne klasy bloków,
    //które różnią się od siebie wyglądem oraz ilości uderzeń potrzebnych do zbicia bloku. Blok może zawierać
    //w sobie bonus, który wyskoczy z niego po zbiciu bloku.
    abstract class Block : GameObject
    {
        //siła - ile razy należy uderzyć blok, aby został zbity
        public int strength;
        //blok może zawierać w sobie bonus
        public Bonus bonus;
        //położenie na siatce bloków
        public Point grid;

        public Block(int gridx, int gridy, int width, int height)
        {
            this.width = width;
            this.height = height;
            grid = new Point(gridx, gridy);

            //utworzenie prostokąta reprezentującego blok
            CreateRectangle();

            //punkt środkowy bloku to jego położenie na siatce bloków * długość bloku + połowa długości bloku
            pos = new Position(grid.x * width + width / 2, grid.y * height + height / 2, width, height);
            //początkowo blok nie ma przypisanego do siebie bonusa
            bonus = null;

            Draw();
        }

        public bool hasBonus()
        {
            return (bonus != null) ? true : false;
        }

        private void CreateRectangle()
        {
            shape = new Rectangle();
            shape.Width = width;
            shape.Height = height;
            Canvas.SetZIndex(shape, 1);
        }

        public void changeGridRowColor()
        {
            //zbity blok zlewa się z tłem
            shape.Fill = null;
        }

        public override void Draw()
        {
            Canvas.SetTop(shape, pos.ty());
            Canvas.SetLeft(shape, pos.lx());
        }
    }
}
