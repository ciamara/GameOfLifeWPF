using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

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

        private bool _isRecording = false;
        public bool IsRecording
        {
            get => _isRecording;
            set
            {
                if (_isRecording != value)
                {
                    _isRecording = value;
                    NotifyPropertyChanged(nameof(IsRecording));
                }
            }
        }
        private int _recordingFrame = 0;

        public int RecordingFrame
        {
            get => _recordingFrame;
            set
            {
                if (_recordingFrame != value)
                {
                    _recordingFrame = value;
                    NotifyPropertyChanged(nameof(RecordingFrame));
                }
            }
        }

        private bool _autoZoom = false;
        public bool AutoZoom
        {
            get => _autoZoom;
            set
            {
                if (_autoZoom != value)
                {
                    _autoZoom = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(AutoZoomText));
                }
            }
        }

        public string AutoZoomText => AutoZoom ? "Auto Zoom: ON" : "Auto Zoom: OFF";

        private List<RenderTargetBitmap> gifFrames = new List<RenderTargetBitmap>();

        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Color? _plantColor = Colors.ForestGreen;
        public Color? PlantColor
        {
            get => _plantColor;
            set
            {
                if (_plantColor != value)
                {
                    _plantColor = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(PlantBrush));
                }
            }
        }

        private Color? _animalColor = Colors.DeepPink;
        public Color? AnimalColor
        {
            get => _animalColor;
            set
            {
                if (_animalColor != value)
                {
                    _animalColor = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(AnimalBrush));
                }
            }
        }

        public SolidColorBrush PlantBrush => new SolidColorBrush(PlantColor ?? Colors.ForestGreen);
        public SolidColorBrush AnimalBrush => new SolidColorBrush(AnimalColor ?? Colors.DeepPink);

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

            SetZoom(1.0);
            BoardScrollViewer.UpdateLayout();

            double centerOffsetX = (BoardScrollViewer.ExtentWidth - BoardScrollViewer.ViewportWidth) / 2;
            double centerOffsetY = (BoardScrollViewer.ExtentHeight - BoardScrollViewer.ViewportHeight) / 2;

            BoardScrollViewer.ScrollToHorizontalOffset(centerOffsetX);
            BoardScrollViewer.ScrollToVerticalOffset(centerOffsetY);
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

            if (IsRecording == true)
            {
                gifFrames.Add(BoardScreenshot(BoardContainer));
                RecordingFrame++;

                if(RecordingFrame > 50)
                {
                    IsRecording = false;
                    RecordingFrame = 0;

                    CreateGif();
                }
            }
            if (AutoZoom)
            {
                zoomOnBorn();
            }
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
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(docPath, "GameOfLifeSave.txt");

            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                outputFile.WriteLine(CurrentOptions.ToString());
                outputFile.WriteLine(CurrentYear.ToString());
                outputFile.WriteLine(Stats.ToString());
                outputFile.WriteLine(PlantColor.ToString());
                outputFile.WriteLine(AnimalColor.ToString());

                if (Board != null)
                {
                    outputFile.WriteLine(Board.Size.ToString());

                    foreach (Field f in Board.Fields)
                    {
                        outputFile.WriteLine(f.ToString());
                    }
                }
            }

            MessageBox.Show($"Saved game state:\n{filePath}", "Save successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(docPath, "GameOfLifeSave.txt");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Did not find save file", "Error loading", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length < 6)
                {
                    MessageBox.Show("Save file corrupted", "Error loading", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // options (line 0)
                Match match = Regex.Match(lines[0], @"R(.*)W(.*)B(.*)P(.*)Z(.*)M(.*)L(.*)");
                if (match.Success)
                {
                    CurrentOptions.RText = match.Groups[1].Value;
                    CurrentOptions.WText = match.Groups[2].Value;
                    CurrentOptions.BText = match.Groups[3].Value;
                    CurrentOptions.PText = match.Groups[4].Value;
                    CurrentOptions.ZText = match.Groups[5].Value;
                    if (int.TryParse(match.Groups[6].Value, out int m)) CurrentOptions.M = m;
                    if (int.TryParse(match.Groups[7].Value, out int l)) CurrentOptions.L = l;
                }

                // year (line 1)
                if (int.TryParse(lines[1], out int year))
                {
                    CurrentYear = year;
                }

                // stats (line 2)
                Stats = new Statistics();
                match = Regex.Match(lines[2], @"P(.*)G(.*)W(.*)A(.*)B(.*)D(.*)");
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int p)) Stats.TotalPlants = p;
                    if (int.TryParse(match.Groups[2].Value, out int g)) Stats.PlantsGrown = g;
                    if (int.TryParse(match.Groups[3].Value, out int w)) Stats.PlantsWilted = w;
                    if (int.TryParse(match.Groups[4].Value, out int a)) Stats.TotalAnimals = a;
                    if (int.TryParse(match.Groups[5].Value, out int b)) Stats.AnimalsBorn = b;
                    if (int.TryParse(match.Groups[6].Value, out int d)) Stats.AnimalsDied = d;
                }

                // plant color (line 3)
                try
                {
                    PlantColor = (Color)ColorConverter.ConvertFromString(lines[3]);
                }
                catch { }

                // animal color (line 4)
                try
                {
                    AnimalColor = (Color)ColorConverter.ConvertFromString(lines[4]);
                }
                catch { }

                // board size + fields
                if (int.TryParse(lines[5], out int size))
                {
                    Board = new Board(size);

                    int lineIndex = 6;
                    foreach (Field f in Board.Fields)
                    {
                        if (lineIndex < lines.Length)
                        {
                            string fieldData = lines[lineIndex];
                            string[] parts = fieldData.Split(',');

                            if (parts.Length >= 2)
                            {
                                if (bool.TryParse(parts[0], out bool hasPlant)) f.HasPlant = hasPlant;
                                if (bool.TryParse(parts[1], out bool hasAnimal)) f.HasAnimal = hasAnimal;

                                if (parts.Length >= 3 && int.TryParse(parts[2], out int age))
                                {
                                    f.AnimalAge = age;
                                }
                                else
                                {
                                    f.AnimalAge = null;
                                }
                            }

                            lineIndex++;
                        }
                    }
                }

                UpdateStatisticsCounts();

                DataContext = null;
                DataContext = this;

                MessageBox.Show("Game loaded", "Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during loading:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private RenderTargetBitmap BoardScreenshot(UIElement el)
        {
            int width = (int)el.RenderSize.Width;
            int height = (int)el.RenderSize.Height;

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);

            rtb.Render(el);
            return rtb;
        }

        private void RecordClipButton_Click(object sender, RoutedEventArgs e)
        {
            IsRecording = true;
        }

        private void CreateGif()
        {
            try
            {
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = System.IO.Path.Combine(docPath, "GameOfLife_Clip.gif");

                GifBitmapEncoder encoder = new GifBitmapEncoder();

                foreach (var frame in gifFrames)
                {
                    encoder.Frames.Add(BitmapFrame.Create(frame));
                }

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                MessageBox.Show($"Saved gif to:\n{filePath}", "Recording stopped", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving GIF:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                gifFrames.Clear();
            }
        }

        private void ZoomOnAnimalsButton_Click(object sender, RoutedEventArgs e)
        {
            AutoZoom = !AutoZoom;

            if (AutoZoom)
            {
                zoomOnBorn();
            }
            if (!AutoZoom)
            {
                SetZoom(1.0);
                BoardScrollViewer.UpdateLayout();

                double centerOffsetX = (BoardScrollViewer.ExtentWidth - BoardScrollViewer.ViewportWidth) / 2;
                double centerOffsetY = (BoardScrollViewer.ExtentHeight - BoardScrollViewer.ViewportHeight) / 2;

                BoardScrollViewer.ScrollToHorizontalOffset(centerOffsetX);
                BoardScrollViewer.ScrollToVerticalOffset(centerOffsetY);
            }
        }

        private void zoomOnBorn()
        {
            var newbornField = Board.Fields.Cast<Field>().FirstOrDefault(f => f.HasAnimal && f.AnimalAge == 0);

            if (newbornField != null)
            {
                SetZoom(3.0);

                BoardScrollViewer.UpdateLayout();

                var container = BoardControl.ItemContainerGenerator.ContainerFromItem(newbornField) as FrameworkElement;

                if (container != null)
                {
                    GeneralTransform transform = container.TransformToAncestor(BoardScrollViewer);
                    Point centerInViewport = transform.Transform(new Point(container.ActualWidth / 2, container.ActualHeight / 2));

                    double deltaX = centerInViewport.X - (BoardScrollViewer.ViewportWidth / 2);
                    double deltaY = centerInViewport.Y - (BoardScrollViewer.ViewportHeight / 2);

                    BoardScrollViewer.ScrollToHorizontalOffset(BoardScrollViewer.HorizontalOffset + deltaX);
                    BoardScrollViewer.ScrollToVerticalOffset(BoardScrollViewer.VerticalOffset + deltaY);
                }
            }
            else
            {
                SetZoom(1.0);
                BoardScrollViewer.UpdateLayout();

                double centerOffsetX = (BoardScrollViewer.ExtentWidth - BoardScrollViewer.ViewportWidth) / 2;
                double centerOffsetY = (BoardScrollViewer.ExtentHeight - BoardScrollViewer.ViewportHeight) / 2;

                BoardScrollViewer.ScrollToHorizontalOffset(centerOffsetX);
                BoardScrollViewer.ScrollToVerticalOffset(centerOffsetY);
            }
        }
    }
}