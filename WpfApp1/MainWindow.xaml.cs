using OrbService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Orb orb;
        private int[,] difficulty;
        private int maxScore;
        private int flipped;
        private bool memo;
        bool zero;
        bool one;
        bool two;
        bool three;
        bool canUseMemo;
        string currentButton;
        ColumnDefinition columnDefinition;
        RowDefinition rowDefinition;

        public MainWindow()
        {
            orb = new();
            difficulty = new int[0, 0];
            maxScore = 1;
            flipped = 0;
            currentButton = "";
            columnDefinition = new();
            rowDefinition = new();
            InitializeComponent();
            GenerateBoard();
            GenerateButtons();
            GenerateMineLabels();
        }

        private void GenerateBoard()
        {
            atLevel.Content = 1;
            GenerateColumns();
            GenerateRows();
        }

        private void GenerateColumns()
        {
            for (int i = 0; i < 8; i++)
            {
                columnDefinition = new();
                GridLength g = new(55);
                if (i == 7)
                    g = new(100);
                columnDefinition.Width = g;
                mainGrid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        private void GenerateRows()
        {
            for (int i = 0; i < 10; i++)
            {
                rowDefinition = new();
                GridLength h = new(70);
                if (i == 1)
                    h = new(110);
                if (i == 3)
                    h = new(56);
                if (i >= 4)
                    h = new(54);
                rowDefinition.Height = h;
                mainGrid.RowDefinitions.Add(rowDefinition);
            }
        }

        private void GenerateButtons()
        {
            Button button = new();
            for (int i = 4; i < 9; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    button = MakeButton(i, j, button);
                    FillButton(i, j, button);
                }
            }
        }

        private Button MakeButton(int row, int column, Button button)
        {
            button = new Button { Name = $"b{row - 4}{column}" };
            button.Height = 50;
            button.Width = 50;
            button.Name = $"b{row - 4}{column}";

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            return button;
        }

        private void FillButton(int row, int column, Button button)
        {
            BitmapImage btm = new(new Uri("/VoltorbFlip_Resources/Cell_Default.png", UriKind.Relative));
            Image img = new();
            img.Source = btm;
            button.Content = img;
            button.Click += new RoutedEventHandler(RevealOnClick);
            button.BorderBrush = Brushes.White;
            mainGrid.Children.Add(button);
            GenerateMemoMarks(button.Name, row, column);
        }

        private void GenerateMemoMarks(string name, int row, int column)
        {
            string picture = "";
            for (int i = 0; i < 4; i++)
            {
                Image img = new();
                img.Height = 10;
                img.Width = 10;
                Grid.SetRow(img, row);
                Grid.SetColumn(img, column);
                img.Name = $"i{name}{i}";

                if (i == 3)
                {
                    picture = "/VoltorbFlip_Resources/Memo/CellNotation/Three.png";
                    img.Margin = new Thickness(25, 0, 0, -25);
                }
                else if (i == 2)
                {
                    picture = "/VoltorbFlip_Resources/Memo/CellNotation/Two.png";
                    img.Margin = new Thickness(-25, 0, 0, -25);
                }
                else if (i == 1)
                {
                    picture = "/VoltorbFlip_Resources/Memo/CellNotation/One.png";
                    img.Margin = new Thickness(25, 0, 0, 25);
                }
                else if (i == 0)
                {
                    picture = "/VoltorbFlip_Resources/Memo/CellNotation/Voltorb.png";
                    img.Margin = new Thickness(-25, 0, 0, 25);
                }
                BitmapImage btm = new(new Uri(picture, UriKind.Relative));

                img.Source = btm;
                img.Visibility = Visibility.Hidden;
                mainGrid.Children.Add(img);
            }
        }

        private void GenerateMineLabels()
        {
            difficulty = orb.SelectDifficulty(Convert.ToInt32(atLevel.Content));
            for (int i = 0; i < 5; i++)
                AddColumnMines(i);

            for (int i = 0; i < 5; i++)
                AddRowMines(i);
        }

        private void AddColumnMines(int column)
        {
            int horizontal = difficulty[column, 0] + difficulty[column, 1] + difficulty[column, 2] + difficulty[column, 3] + difficulty[column, 4];
            int horizontal2 = 0;
            for (int j = 0; j < 5; j++)
            {
                if (difficulty[column, j] == 0)
                    horizontal2++;
                if (difficulty[column, j] > 1)
                    maxScore = maxScore * difficulty[column, j];
            }

            ChangeLabel($"lv{column}", horizontal, column + 4, 5, 0);
            ChangeLabel($"lv{column}2", horizontal2, column + 4, 5, 20);
        }

        private void AddRowMines(int row)
        {
            int vertical = difficulty[0, row] + difficulty[1, row] + difficulty[2, row] + difficulty[3, row] + difficulty[4, row];
            int vertical2 = 0;
            for (int j = 0; j < 5; j++)
            {
                if (difficulty[j, row] == 0)
                    vertical2++;
            }

            ChangeLabel($"lh{row}", vertical, 9, row, 0);
            ChangeLabel($"lh{row}2", vertical2, 9, row, 20);
        }

        private void ChangeLabel(string name, int direction, int row, int column, int thickness)
        {
            Label label = new();
            label.Name = name;
            label.Content = direction;
            label.FontSize = 18;
            label.FontWeight = FontWeights.Black;
            Grid.SetColumn(label, column);
            Grid.SetRow(label, row);
            label.Margin = new Thickness(29, thickness, 0, 5);
            mainGrid.Children.Add(label);
        }

        private void RevealOnClick(object sender, RoutedEventArgs e)
        {
            canUseMemo = true;
            Button? senderGrid = sender as Button;
            int row = Grid.GetRow(senderGrid) - 4;
            int column = Grid.GetColumn(senderGrid);

            if (senderGrid != null)
            {
                if (!memo)
                    ResolveTriggeredButton(senderGrid, row, column);
                else
                {
                    if (senderGrid.Name != null)
                        currentButton = senderGrid.Name;

                    for (int i = 0; i < 4; i++)
                        ImageChangeOnVisibilityChange(i);

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Button? btn = mainGrid.Children.OfType<Button>().FirstOrDefault(q => q.Name == $"b{i}{j}");
                            if (btn != null)
                                btn.BorderBrush = Brushes.Green;
                        }
                    }
                    senderGrid.BorderBrush = Brushes.Orange;
                }
            }
        }

        private void ResolveTriggeredButton(Button senderGrid, int row, int column)
        {
            ResetMemoPad();

            HideMarksOfButton(senderGrid);

            RevealCell(senderGrid, row, column);

            if (difficulty[row, column] == 0)
                ResetBoardOnMine();
            else if (Convert.ToInt32(atLevel.Content) < 9 && levelScore.Content.ToString() == maxScore.ToString())
                ResetBoardOnClearingBoard();
            atLevel.Content = Convert.ToInt32(atLevel.Content);
        }

        private void ResetMemoPad()
        {
            canUseMemo = false;
            BitmapImage btm0 = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/VoltorbUnselected.png", UriKind.Relative));
            zero = false;
            imageZero.Source = btm0;
            BitmapImage btm1 = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/OneUnselected.png", UriKind.Relative));
            one = false;
            imageOne.Source = btm1;
            BitmapImage btm2 = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/TwoUnselected.png", UriKind.Relative));
            two = false;
            imageTwo.Source = btm2;
            BitmapImage btm3 = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/ThreeUnselected.png", UriKind.Relative));
            three = false;
            imageThree.Source = btm3;
        }

        private void HideMarksOfButton(Button senderGrid)
        {
            for (int i = 0; i < 4; i++)
            {
                string image = $"i{senderGrid.Name}{i}";
                Image? imgMark = mainGrid.Children.OfType<Image>().FirstOrDefault(q => q.Name == image);
                if (imgMark != null)
                    imgMark.Visibility = Visibility.Hidden;
            }
        }

        private void RevealCell(Button senderGrid, int row, int column)
        {
            senderGrid.IsEnabled = false;
            senderGrid.Content = difficulty[row, column].ToString();

            if (difficulty[row, column].ToString() == "1")
                MakeImage(senderGrid, "/VoltorbFlip_Resources/Cell_One.png");
            else if (difficulty[row, column].ToString() == "2")
                MakeImage(senderGrid, "/VoltorbFlip_Resources/Cell_Two.png");
            else if (difficulty[row, column].ToString() == "3")
                MakeImage(senderGrid, "/VoltorbFlip_Resources/Cell_Three.png");
            else
                MakeImage(senderGrid, "/VoltorbFlip_Resources/Cell_Three.png");

            if (levelScore.Content.ToString() != "0")
                levelScore.Content = (Convert.ToInt32(levelScore.Content.ToString()) * difficulty[row, column]).ToString();
            else
                levelScore.Content = difficulty[row, column].ToString();

            if (difficulty[row, column] != 0)
                flipped++;
        }

        private void ResetBoardOnMine()
        {
            RevealTheBoard();
            MessageBox.Show("Game over!");
            if (flipped < Convert.ToInt32(atLevel.Content) && flipped > 0)
                atLevel.Content = flipped;
            else if (flipped == 0)
                atLevel.Content = 1;
            maxScore = 1;
            ResetLevel();
        }

        private void ResetBoardOnClearingBoard()
        {
            RevealTheBoard();
            MessageBox.Show($"Board cleared! {levelScore.Content} coins added to total!");
            maxScore = 1;
            currentScore.Content = Convert.ToInt32(currentScore.Content) + Convert.ToInt32(levelScore.Content.ToString());
            if (Convert.ToInt32(atLevel.Content) < 8)
                atLevel.Content = (Convert.ToInt32(atLevel.Content) + 1).ToString();
            ResetLevel();
        }

        private void ImageChangeOnVisibilityChange(int memoNr)
        {
            Image? img = mainGrid.Children.OfType<Image>().FirstOrDefault(q => q.Name == $"i{currentButton}{memoNr}");
            BitmapImage btm;

            if (img != null)
            {
                if (memoNr == 0)
                {
                    if (img.Visibility == Visibility.Visible)
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/VoltorbSelected.png", UriKind.Relative));
                        zero = true;
                    }
                    else
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/VoltorbUnselected.png", UriKind.Relative));
                        zero = false;
                    }
                    imageZero.Source = btm;
                }
                else if (memoNr == 1)
                {
                    if (img.Visibility == Visibility.Visible)
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/OneSelected.png", UriKind.Relative));
                        one = true;
                    }
                    else
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/OneUnselected.png", UriKind.Relative));
                        one = false;
                    }
                    imageOne.Source = btm;
                }
                else if (memoNr == 2)
                {
                    if (img.Visibility == Visibility.Visible)
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/TwoSelected.png", UriKind.Relative));
                        two = true;
                    }
                    else
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/TwoUnselected.png", UriKind.Relative));
                        two = false;
                    }
                    imageTwo.Source = btm;
                }
                else if (memoNr == 3)
                {
                    if (img.Visibility == Visibility.Visible)
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/ThreeSelected.png", UriKind.Relative));
                        three = true;
                    }
                    else
                    {
                        btm = new(new Uri("/VoltorbFlip_Resources/Memo/MemoField/ThreeUnselected.png", UriKind.Relative));
                        three = false;
                    }
                    imageThree.Source = btm;
                }
            }
        }

        private void ResetLevel()
        {
            levelScore.Content = "0";
            difficulty = orb.SelectDifficulty(Convert.ToInt32(atLevel.Content));
            ResetButtons();
            ResetLabels();
            flipped = 0;
        }

        private void ResetLabels()
        {
            for (int i = 0; i < 5; i++)
            {
                int horizontal = 0;
                Label? lbl = mainGrid.Children.OfType<Label>().FirstOrDefault(q => q.Name == $"lv{i}");
                Label? lbl2 = mainGrid.Children.OfType<Label>().FirstOrDefault(q => q.Name == $"lv{i}2");
                for (int j = 0; j < 5; j++)
                {
                    if (difficulty[i, j] == 0)
                        horizontal++;
                }
                if (lbl != null)
                    lbl.Content = difficulty[i, 0] + difficulty[i, 1] + difficulty[i, 2] + difficulty[i, 3] + difficulty[i, 4];
                if (lbl2 != null)
                    lbl2.Content = horizontal;
            }

            for (int i = 0; i < 5; i++)
            {
                int horizontal = 0;
                Label? lbl3 = mainGrid.Children.OfType<Label>().FirstOrDefault(q => q.Name == $"lh{i}");
                Label? lbl4 = mainGrid.Children.OfType<Label>().FirstOrDefault(q => q.Name == $"lh{i}2");
                for (int j = 0; j < 5; j++)
                {
                    if (difficulty[j, i] == 0)
                        horizontal++;
                    if (difficulty[j, i] > 1)
                        maxScore = maxScore * difficulty[j, i];
                }
                if (lbl3 != null)
                    lbl3.Content = difficulty[0, i] + difficulty[1, i] + difficulty[2, i] + difficulty[3, i] + difficulty[4, i];
                if (lbl4 != null)
                    lbl4.Content = horizontal;
            }
        }

        private void ResetButtons()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    BitmapImage btm = new(new Uri("/VoltorbFlip_Resources/Cell_Default.png", UriKind.Relative));
                    Image img = new();
                    img.Source = btm;
                    Button? btn = mainGrid.Children.OfType<Button>().FirstOrDefault(q => q.Name == $"b{i}{j}");
                    if (btn != null)
                    {
                        btn.Content = img;
                        btn.IsEnabled = true;
                    }
                }
            }
        }

        private void RevealTheBoard()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        string image = $"ib{i}{j}{k}";
                        Image? imgMark = mainGrid.Children.OfType<Image>().FirstOrDefault(q => q.Name == image);
                        if (imgMark != null)
                            imgMark.Visibility = Visibility.Hidden;
                    }

                    if (difficulty[i, j].ToString() == "1")
                        MakeButton("/VoltorbFlip_Resources/Cell_One.png", $"b{i}{j}");
                    else if (difficulty[i, j].ToString() == "2")
                        MakeButton("/VoltorbFlip_Resources/Cell_Two.png", $"b{i}{j}");
                    else if (difficulty[i, j].ToString() == "3")
                        MakeButton("/VoltorbFlip_Resources/Cell_Three.png", $"b{i}{j}");
                    else
                        MakeButton("/VoltorbFlip_Resources/Cell_Voltorb.png", $"b{i}{j}");
                }
            }
        }

        private void OpenMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (memo)
            {
                memo = false;
                BitmapImage btm = new(new Uri("/VoltorbFlip_Resources/Memo/OpenMemo.png", UriKind.Relative));
                memoImage.Source = btm;
                buttonZero.Visibility = Visibility.Hidden;
                buttonOne.Visibility = Visibility.Hidden;
                buttonTwo.Visibility = Visibility.Hidden;
                buttonThree.Visibility = Visibility.Hidden;
                buttonBackground.Visibility = Visibility.Hidden;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Button? btn = mainGrid.Children.OfType<Button>().FirstOrDefault(q => q.Name == $"b{i}{j}");
                        if (btn != null)
                        {
                            btn.BorderBrush = Brushes.White;
                            btn.IsEnabled = true;
                        }
                    }
                }
            }
            else
            {
                memo = true;
                BitmapImage btm = new(new Uri("/VoltorbFlip_Resources/Memo/CloseMemo.png", UriKind.Relative));
                memoImage.Source = btm;
                buttonZero.Visibility = Visibility.Visible;
                buttonOne.Visibility = Visibility.Visible;
                buttonTwo.Visibility = Visibility.Visible;
                buttonThree.Visibility = Visibility.Visible;
                buttonBackground.Visibility = Visibility.Visible;
            }
        }

        private void Quit_OnClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Mark3_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(currentButton) || canUseMemo == false) { return; }

            BitmapImage btm;

            if (!three)
                btm = RevealOrHideMark($"i{currentButton}3", "/VoltorbFlip_Resources/Memo/MemoField/ThreeSelected.png", Visibility.Visible);
            else
                btm = RevealOrHideMark($"i{currentButton}3", "/VoltorbFlip_Resources/Memo/MemoField/ThreeUnselected.png", Visibility.Hidden);
            three = !three;
            imageThree.Source = btm;
        }

        private void Mark2_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(currentButton) || canUseMemo == false) { return; }

            BitmapImage btm;
            if (!two)
                btm = RevealOrHideMark($"i{currentButton}2", "/VoltorbFlip_Resources/Memo/MemoField/TwoSelected.png", Visibility.Visible);
            else
                btm = RevealOrHideMark($"i{currentButton}2", "/VoltorbFlip_Resources/Memo/MemoField/TwoUnselected.png", Visibility.Hidden);
            two = !two;
            imageTwo.Source = btm;
        }

        private void Mark1_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(currentButton) || canUseMemo == false) { return; }

            BitmapImage btm;
            if (!one)
                btm = RevealOrHideMark($"i{currentButton}1", "/VoltorbFlip_Resources/Memo/MemoField/OneSelected.png", Visibility.Visible);
            else
                btm = RevealOrHideMark($"i{currentButton}1", "/VoltorbFlip_Resources/Memo/MemoField/OneUnselected.png", Visibility.Hidden);
            one = !one;
            imageOne.Source = btm;
        }

        private void Mark0_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(currentButton) || canUseMemo == false) { return; }

            BitmapImage btm;

            if (!zero)
                btm = RevealOrHideMark($"i{currentButton}0", "/VoltorbFlip_Resources/Memo/MemoField/VoltorbSelected.png", Visibility.Visible);
            else
                btm = RevealOrHideMark($"i{currentButton}0", "/VoltorbFlip_Resources/Memo/MemoField/VoltorbUnselected.png", Visibility.Hidden);
            zero = !zero;
            imageZero.Source = btm;
        }

        private BitmapImage RevealOrHideMark(string image, string imagePath, Visibility visibility)
        {
            Image? imgMark = mainGrid.Children.OfType<Image>().FirstOrDefault(q => q.Name == image);
            BitmapImage btm = new(new Uri(imagePath, UriKind.Relative));
            if (imgMark != null)
                imgMark.Visibility = visibility;
            return btm;
        }
        private void MakeButton(string imagePath, string buttonName)
        {
            Button? btn = mainGrid.Children.OfType<Button>().FirstOrDefault(q => q.Name == buttonName);
            BitmapImage btm = new(new Uri(imagePath, UriKind.Relative));
            Image img = new();
            img.Source = btm;
            if (btn != null)
            {
                btn.Content = img;
            }
        }
        private void MakeImage(Button senderGrid, string imagePath)
        {
            BitmapImage btm = new(new Uri(imagePath, UriKind.Relative));
            Image img = new();
            img.Source = btm;
            senderGrid.Content = img;
        }
    }
}
