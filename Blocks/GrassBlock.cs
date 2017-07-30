namespace Arkanoid
{
    class GrassBlock : Block
    {
        public GrassBlock(int gridx, int gridy, int width, int height) : base(gridx, gridy, width, height)
        {
            strength = 1;
            bmpImage = MainWindow.imgGrass;
            InitializeShape();
        }
    }
}
