namespace Arkanoid
{
    class StoneBlock : Block
    {
        public StoneBlock(int gridx, int gridy, int width, int height) : base(gridx, gridy, width, height)
        {
            strength = 3;
            bmpImage = MainWindow.imgStone;
            InitializeShape();
        }
    }
}
