using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.WINDOWS
{
    public partial class User
    {
        private const string ConfigPath =
            @"C:\Users\j1sk1ss\RiderProjects\NeuroWeb.EXMPL\NeuroWeb.EXMPL\DATA\Config.txt";
        
        public User() {
            InitializeComponent();

            var netConfig = DataWorker.ReadNetworkConfig(ConfigPath);
            Network = new Network(netConfig);
            Network.ReadWeights();
            
            Update = new DispatcherTimer {
                Interval = new TimeSpan(0,0,0,1)
            };
            Update.Tick += AnalyzeUserInput;
            Update.IsEnabled = true;
        }
        private Network Network { get; set; }
        private DispatcherTimer Update { get; set; }
        
        private readonly Brush _userBrush = Brushes.Black;
        private int _pred = 1;
        private void AnalyzeUserInput(object sender, EventArgs eventArgs) {
            var renderTargetBitmap = new RenderTargetBitmap(28,28, 6.5d, 6.5d, 
                PixelFormats.Pbgra32);
            renderTargetBitmap.Render(UserCanvas);

            var writeableBitmap = new WriteableBitmap(renderTargetBitmap);
            
            var matrix      = new double[28,28];
            var temp        = "";
            var numberValue = "";
            
            for (var i = 0; i < 28; i++) {
                for (var j = 0; j < 28; j++) {
                    matrix[i,j] = writeableBitmap.GetPixel(j, i).A / 255d;
                    if (matrix[i, j] > 0) temp += _pred + "  ";
                    else temp += " " + "  ";
                    
                    numberValue += matrix[i, j]+ "  ";
                }

                temp += "\n";
            }

            Answers.Content = "";
            
            for (var i = 0; i < 10; i++)
                Answers.Content += $"\n\n{i} - {Math.Abs(Math.Round(Network.NeuronsValue[2][i] * 100, 1))}%";
            
            Matrix.Content = temp;
            _pred = Prediction.Predict(Network, numberValue);
        }
        
        private Point _currentPoint;
        private void UserMoveMouse(object sender, MouseEventArgs e)
        {
            AnalyzeUserInput(null, null);
            if (e.LeftButton != MouseButtonState.Pressed) return;
            
            var line = new Line {
                Stroke = _userBrush,
                X1     = _currentPoint.X,
                Y1     = _currentPoint.Y,
                X2     = e.GetPosition(this).X,
                Y2     = e.GetPosition(this).Y,
                
                StrokeThickness = 8
            };

            _currentPoint = e.GetPosition(this);

            UserCanvas.Children.Add(line);
        }

        private void UserClick(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed)
                _currentPoint = e.GetPosition(this);
        }

        private void BackPropagation(object sender, RoutedEventArgs e) {
            Update.IsEnabled = false;
            var number = int.Parse(ExpectedAnswer.Text);
            ExpectedAnswer.Text = "";
            
            Network.BackPropagation(number);
            Network.SetWeights(.01);
            Update.IsEnabled = true;
        }

        private void Clear(object sender, RoutedEventArgs e) => UserCanvas.Children.Clear();
        
        private void SaveAdnExit(object sender, RoutedEventArgs e) {
            MessageBox.Show("Сохранение начато...");
            Network.SaveWeights();
            Close();
        }
    }
}