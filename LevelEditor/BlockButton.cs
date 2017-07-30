using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Arkanoid
{
    //Przycisk reprezentujący blok w edytorze poziomów. Po kliknięciu na element siatki (przycisk) pojawia się 
    //obrazek trawy, reprezentujący ustawienie w tym miejscu bloku typu GreenBlock.
    //Kolejne kliknięcia powodują zamianę poprzedniego obrazka na inny, reprezentujący inny typ bloku.
    //Po odpowiedniej liczbie kliknięć pojawia się na powrót blok pusty, reprezentujący puste miejsce na planszy.
    public class BlockButton : Button
    {
        //rodzaj bloku - grass, metal, brick itd
        public int blockType { get; set; }
        //położenie na siatce
        public int gridx { get; set; }
        public int gridy { get; set; }
        protected BitmapImage bmpImage;
        protected string bmpName;


        public BlockButton(int x, int y, int width, int height)
        {
            gridx = x;
            gridy = y;

            Height = height;
            Width = width;
            //po kliknięciu przycisku nastąpi zmiana jego typu
            Click += ChangeBlockType;

            blockType = (int)BlockTypes.none;

            Draw();
        }

        public void Draw()
        {
            bool setImage = true;
            //narysowanie bloku w zależności od jego typu
            switch (blockType)
            {
                case (int)BlockTypes.none:
                    Background = MainWindow.bgColor;
                    setImage = false;
                    break;
                case (int)BlockTypes.grass:
                    bmpImage = MainWindow.imgGrass;
                    break;
                case (int)BlockTypes.ice:
                    bmpImage = MainWindow.imgIce;
                    break;
                case (int)BlockTypes.brick:
                    bmpImage = MainWindow.imgBrick;
                    break;
                case (int)BlockTypes.stone:
                    bmpImage = MainWindow.imgStone;
                    break;
                case (int)BlockTypes.metal:
                    bmpImage = MainWindow.imgMetal;
                    break;
            }

            if (setImage)
            { 
                Background = new ImageBrush
                {
                    ImageSource = bmpImage
                };
            }            
        }

        public void ChangeBlockType(object sender, RoutedEventArgs e)
        {
            //zwiększa o 1 typ bloku i dokonuje operacji modulo liczby typów bloków
            blockType++;
            blockType %= MainWindow.blockTypes;

            Draw();
        }
    }
}
