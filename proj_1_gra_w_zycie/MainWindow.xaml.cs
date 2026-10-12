using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace proj_1_gra_w_zycie
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Board Board { get; private set; }

        public Options CurrentOptions { get; private set; }

        public Statistics Stats { get; private set; }

        public int SimulationSpeed { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => InputBoardSize();
        }

        public void InputBoardSize()
        {
            SizeDialogBox dialogBox = new SizeDialogBox();
            dialogBox.Owner = this;

            bool? dialogResult = dialogBox.ShowDialog();

            if (dialogResult == true)
            {
                int size = dialogBox.BoardSize;
                Board = new Board(size);

                DataContext = this;

                InitializeGame();
            }
            else
            {
                Application.Current.Shutdown();
                Close();
            }
        }

        public void InitializeGame()
        {
            CurrentOptions = new Options();
            Stats = new Statistics();
        }

        private void PopulateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}