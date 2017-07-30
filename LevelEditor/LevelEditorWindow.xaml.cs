using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Arkanoid
{
    /// <summary>
    /// Interaction logic for LevelEditorWindow.xaml
    /// </summary>
    /// 

    //Edytor poziomów. Pozwala na tworzenie nowych poziomów o określonej nazwie, pozycji i rodzaju bloków oraz liczbie żyć.
    public partial class LevelEditorWindow : Window
    {
        //lista przycisków
        private List<BlockButton> buttons;
        //nazwa zapisywanego poziomu
        private string filename = "defaultLevel";
        //liczba żyć zapisywanego poziomu
        private int lives = 3;
        //liczba kolumn i wierszy siatki bloków
        private int gridColumns;
        private int gridRows;
        //wymiary bloków
        private int blockWidth;
        private int blockHeight;
        private MainWindow win;

        public LevelEditorWindow(MainWindow win, int gridColumns, int gridRows, int blockWidth, int blockHeight, int blockGridHeight)
        {
            this.gridColumns = gridColumns;
            this.gridRows = gridRows;
            this.blockWidth = blockWidth;
            this.blockHeight = blockHeight;
            this.win = win;
            buttons = new List<BlockButton>();

            InitializeComponent();
            textBoxFilename.Text = filename;
            textBoxLives.Text = Convert.ToString(lives);

            //wymiary okna
            editorWindow.Width = blockWidth * gridColumns + 15;
            editorWindow.Height = blockGridHeight * gridRows + 100;

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            blocksGrid.ShowGridLines = true;
            blocksGrid.Background = MainWindow.bgColor;

            //utworzenie siatki. najpierw tworzone są kolumny, a następnie zawierające się w nich wiersze
            for (int i = 0; i < gridColumns; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(blockWidth);
                blocksGrid.ColumnDefinitions.Add(column);

                for (int j = 0; j < gridRows; j++)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(blockHeight);
                    blocksGrid.RowDefinitions.Add(row);

                    BlockButton button = new BlockButton(i, j, blockWidth, blockHeight);
                    buttons.Add(button);

                    Grid.SetRow(button, j);
                    Grid.SetColumn(button, i);
                    blocksGrid.Children.Add(button);
                }
            }
        }
        private void HandleKeydown(object sender, KeyEventArgs e)
        {
            //enter zapisuje poziom
            switch (e.Key)
            {
                case Key.Enter:
                    SaveLevel();
                    break;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //walidacja pola liczby żyć - dostępne tylko cyfry 0-9
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveButtonHandler(object sender, RoutedEventArgs e)
        {
            SaveLevel();
        }

        private void SaveLevel()
        {
            //zliczenie, ile występuje bloków różnego rodzaju - na potrzeby nagłówka; dodatkowo liczba żyć
            int[] header = new int[MainWindow.blockTypes + 1];
            CountBlocks(ref header);

            GetDataFromTextBoxes();
            header[MainWindow.blockTypes] = lives;

            //lista bloków zawierających dane niezbędne do serializacji
            List<SerializableBlock> serializedBlocks = new List<SerializableBlock>();

            CreateSerializableList(serializedBlocks);
            Serialize(header, serializedBlocks);

            win.FindLevels();
            //zamknięcie okna po pomyślnym zapisie
            this.Close();            
        }

        private void GetDataFromTextBoxes()
        {
            //jeśli liczba żyć nie została wpisana, wpisana jest domyślna wartość 3
            if (textBoxLives.Text == "")
            {
                lives = 1;
            }
            else
            {
                lives = Convert.ToInt32(textBoxLives.Text);
            }
            //nazwa poziomu
            filename = textBoxFilename.Text;
        }

        private void CreateSerializableList(List<SerializableBlock> serializedBlocks)
        {
            //utworzenie bloków możliwych do zserializowania. zostają do nich wpisane
            //informacje dotyczących tych przycisków, które nie mają ustawionego typu bloku
            foreach (BlockButton block in buttons)
            {
                if (block.blockType != (int)BlockTypes.none)
                {
                    SerializableBlock serializedBlock = new SerializableBlock(block.blockType, block.gridx, block.gridy);
                    serializedBlocks.Add(serializedBlock);
                }
            }
        }

        private void Serialize(int[] blocks, List<SerializableBlock> serializedBlocks)
        {
            
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(int[]));
                //serializacja nagłówka - liczba bloków różnego rodzaju + liczba żyć
                using (TextWriter textWriter = new StreamWriter(MainWindow.pathToLevels + filename + GameController.dataExtension))
                {
                    serializer.Serialize(textWriter, blocks);
                }

                serializer = new XmlSerializer(typeof(List<SerializableBlock>));
                //serializacja bloków
                using (TextWriter textWriter = new StreamWriter(MainWindow.pathToLevels + filename + GameController.blockExtension))
                {
                    serializer.Serialize(textWriter, serializedBlocks);
                }
            }
        }

        private void CountBlocks(ref int[] blocks)
        {
            foreach (BlockButton button in buttons)
            {
                switch (button.blockType)
                {
                    case (int)BlockTypes.grass:
                        blocks[(int)BlockTypes.grass]++;
                        break;
                    case (int)BlockTypes.ice:
                        blocks[(int)BlockTypes.ice]++;
                        break;
                    case (int)BlockTypes.brick:
                        blocks[(int)BlockTypes.brick]++;
                        break;
                    case (int)BlockTypes.stone:
                        blocks[(int)BlockTypes.stone]++;
                        break;
                    case (int)BlockTypes.metal:
                        blocks[(int)BlockTypes.metal]++;
                        break;
                }
            }
        }
        private void CloseApplication(object sender, EventArgs e)
        {
            win.FindLevels();
            Application.Current.Shutdown();
        }
    }
}
