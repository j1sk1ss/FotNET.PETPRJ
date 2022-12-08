using System;
using System.Diagnostics.CodeAnalysis;
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
        private Network Network { get; }
        private DispatcherTimer Update { get; }
        private string Number { get; set; }
        
        private readonly Brush _userBrush = Brushes.Black;
        private int _pred = 1;
        [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
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
                    else temp += "  " + "  ";
                    
                    numberValue += matrix[i, j]+ "  ";
                }

                temp += "\n";
            } 
            
            One.Content   = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][1] * 100, 1))}%";
            Two.Content   = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][2] * 100, 1))}%";
            Three.Content = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][3] * 100, 1))}%";
            Four.Content  = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][4] * 100, 1))}%";
            Five.Content  = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][5] * 100, 1))}%";
            Six.Content   = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][6] * 100, 1))}%";
            Seven.Content = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][7] * 100, 1))}%";
            Eight.Content = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][8] * 100, 1))}%";
            Nine.Content  = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][9] * 100, 1))}%";
            Zero.Content  = $"{Math.Abs(Math.Round(Network.NeuronsValue[2][0] * 100, 1))}%";
            
            Matrix.Content = temp;
            Number         = numberValue;
            _pred          = Prediction.Predict(Network, numberValue);
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
            if (int.TryParse(ExpectedAnswer.Text, out var number)) {
                ExpectedAnswer.Text = "";
                Teaching.LightStudying(Network, Number, number);
            }
            else MessageBox.Show("Введённое число не корректно!");
            
            Update.IsEnabled = true;
        }

        private void Clear(object sender, RoutedEventArgs e) => UserCanvas.Children.Clear();
        
        private void SaveAndExit(object sender, RoutedEventArgs e) {
            MessageBox.Show("Сохранение начато...");
            Network.SaveWeights();
            Close();
        }
    }
}