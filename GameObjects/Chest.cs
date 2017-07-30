using System.Windows.Controls;
using System.Windows.Shapes;

namespace Arkanoid
{
    //Skrzynia. Gracz trafiając piłką do otwartej skrzyni wygrywa grę. Skrzynia jest początkowo otwarta, jednak określone
    //bonusy mogą ją otworzyć lub ponownie zamknąć.
    class Chest : GameObject
    {
        //czy skrzynia jest zamknięta
        public bool isClosed;

        public Chest()
        { 
            width = GameController.chestWidth;
            shape = new Rectangle();
            Canvas.SetZIndex(shape, 5);
            Close();
        }

        public void Close()
        {
            isClosed = true;
            height = 50;
            bmpImage = MainWindow.imgClosedChest;
            InitializeShape();
            Draw();
        }

        public void Open()
        {
            isClosed = false;
            height = 100;
            bmpImage = MainWindow.imgOpenChest;
            InitializeShape();
            Draw();
        }

        public override void Draw()
        {
            Canvas.SetTop(shape, MainWindow.screenHeight - height - 25);
            Canvas.SetLeft(shape, MainWindow.screenWidth - width);
        }
    }
}
