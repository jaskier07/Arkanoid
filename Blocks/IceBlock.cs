namespace Arkanoid
{
    class IceBlock : Block
    {
        public IceBlock(int gridx, int gridy, int width, int height) : base(gridx, gridy, width, height)
        {
            strength = 1;
            bmpImage = MainWindow.imgIce;
            InitializeShape();
        }
    }
}
