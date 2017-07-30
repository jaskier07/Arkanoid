using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Arkanoid
{
    //Klasa, po której dziedziczą wszystkie statyczne i dynamiczne elementy gry.
    abstract class GameObject
    {
        //obraz reprezentujący obiekt
        protected BitmapImage bmpImage;
        protected GameController gc;
        //kształt reprezentujący obiekt
        public Shape shape;
        //obecna pozycja obiektu
        public Position pos;
        //wymiary obiektu;
        public int height;
        public int width;

        //narysowanie obiektu
        public abstract void Draw();
        //inicjalizacja kształtu i obrazka reprezentującego obiekt
        public void InitializeShape()
        {
            shape.Height = height;
            shape.Width = width;
            shape.Fill = new ImageBrush
            {
                ImageSource = bmpImage
            };
        }
    }
}
