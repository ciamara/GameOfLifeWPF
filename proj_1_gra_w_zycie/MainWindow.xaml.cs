using System.ComponentModel;
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
using System.Windows.Threading;
using System;

namespace proj_1_gra_w_zycie
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window, INotifyPropertyChanged
    {
        private DispatcherTimer _simulationTimer;
        private int _simulationSpeed = 0;
        private int _currentYear = 0;
        public Board Board { get; private set; }

        public Options CurrentOptions { get; private set; }

        public Statistics Stats { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CurrentYear
        {
            get => _currentYear;
            set
            {
                if (_currentYear != value)
                {
                    _currentYear = value;
                    NotifyPropertyChanged(nameof(CurrentYear));
                }
            }
        }

        public int SimulationSpeed
        {
            get => _simulationSpeed;
            set
            {
                if (_simulationSpeed != value)
                {
                    _simulationSpeed = value;
                    UpdateTimerInterval();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Tick += SimulationTimer_Tick;

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

                InitializeGame();

                DataContext = this;
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
            CurrentYear = 0;
        }

        private void UpdateTimerInterval()
        {
            if (_simulationTimer == null) return;

            if (_simulationSpeed == 0)
            {
                _simulationTimer.Stop();
            }
            else
            {
                int intervalMs = Math.Max(50, 1100 - (_simulationSpeed * 100));
                _simulationTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);

                if (!_simulationTimer.IsEnabled)
                {
                    _simulationTimer.Start();
                }
            }
        }

        private void SimulationTimer_Tick(object? sender, EventArgs e)
        {
            ExecuteSimulationStep();
        }

        private void ExecuteSimulationStep()
        {
            Board?.Tick(CurrentOptions, Stats);
            CurrentYear++;
            UpdateStatisticsCounts();
        }

        private void UpdateStatisticsCounts()
        {
            if (Board != null && Stats != null)
            {
                Stats.TotalPlants = Board.Fields.Cast<Field>().Count(f => f.HasPlant);
                Stats.TotalAnimals = Board.Fields.Cast<Field>().Count(f => f.HasAnimal);
            }
        }

        private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Field cell)
            {
                cell.HasPlant = !cell.HasPlant;
                UpdateStatisticsCounts();
            }
        }

        private void Cell_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Field cell)
            {
                cell.HasAnimal = !cell.HasAnimal;
                cell.AnimalAge = cell.HasAnimal ? 0 : null;
                UpdateStatisticsCounts();
            }
        }

        private void PopulateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Board.Empty())
            {
                Board.SetRandomPlants();
                Board.SetRandomAnimals(CurrentOptions);
                UpdateStatisticsCounts();
            }
            else
            {
                return;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Board.Clear();
            CurrentYear = 0;
            Stats?.Reset();
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[DEBUG] R: '{CurrentOptions.RText}' (Parsed: {string.Join(",", CurrentOptions.R)}) | " +
                $"W: '{CurrentOptions.WText}' (Parsed: {string.Join(",", CurrentOptions.W)}) | " +
                $"M: {CurrentOptions.M} | L: {CurrentOptions.L}");
            ExecuteSimulationStep();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void SetZoom(double scale)
        {
            BoardScaleTransform.ScaleX = scale;
            BoardScaleTransform.ScaleY = scale;
        }

        private void BoardScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;

                double zoomFactor = e.Delta > 0 ? 1.15 : 0.85;
                double newScale = BoardScaleTransform.ScaleX * zoomFactor;

                if (newScale >= 0.5 && newScale <= 5.0)
                {
                    SetZoom(newScale);
                }
            }
        }
    }
}