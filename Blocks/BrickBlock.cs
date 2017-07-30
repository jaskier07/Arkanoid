namespace Arkanoid
{
    class BrickBlock : Block
    {
        public BrickBlock(int gridx, int gridy, int width, int height) : base(gridx, gridy, width, height)
        {
            strength = 2;
            bmpImage = MainWindow.imgBrick;
            InitializeShape();
        }
    }
}
