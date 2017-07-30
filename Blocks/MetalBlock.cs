namespace Arkanoid
{
    class MetalBlock : Block
    {
        public MetalBlock(int gridx, int gridy, int width, int height) : base(gridx, gridy, width, height)
        {
            strength = 4;
            bmpImage = MainWindow.imgMetal;
            InitializeShape();
        }
    }
}
