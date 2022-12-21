using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32;

using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.WINDOWS {
    public partial class User {
        public User() {
            var defaultConfig = Properties.Resources.defaultConfig;

            if (MessageBox.Show("Использовать стандартную конфигарацию вместо другой?", 
                    "Укажите конфигурацию!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Network = new Network(DataWorker.ReadNetworkConfig(defaultConfig));
            else {
                  var file = new OpenFileDialog();
                  if (file.ShowDialog() == true)
                      Network = new Network(DataWorker.ReadNetworkConfig(File.ReadAllText(file.FileName)));
            }
            
            InitializeComponent();
            
            Answers = new List<Label> {
                 Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine
            };  
            
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
        private List<Label> Answers { get; }

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
            
            for (var i = 0; i < Answers.Count; i++) Answers[i].Content = 
                $"{Math.Abs(Math.Round(Network.NeuronsValue[2][i] * 100, 1))}%";            
            
            Matrix.Content = temp;
            Number         = numberValue;
            _pred          = Prediction.Predict(Network, numberValue);
        }
        
        private Point _currentPoint;
        
        private void UserMoveMouse(object sender, MouseEventArgs e) {
            AnalyzeUserInput(null, null);
            if (e.LeftButton != MouseButtonState.Pressed) return;
            
            var line = new Line {
                Stroke = _userBrush,
                X1     = _currentPoint.X,
                Y1     = _currentPoint.Y,
                X2     = e.GetPosition(UserCanvas).X,
                Y2     = e.GetPosition(UserCanvas).Y,
                
                StrokeThickness = 8
            };

            _currentPoint = e.GetPosition(UserCanvas);
            UserCanvas.Children.Add(line);
        }

        private void UserClick(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed)
                _currentPoint = e.GetPosition(UserCanvas);
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
        
        private void Exit(object sender, RoutedEventArgs e) => Close();
        
        private void SaveWeights(object sender, RoutedEventArgs e) {
            MessageBox.Show("Сохранение начато...");
            Network.SaveWeights();
        }
        
        private void LoadWeights(object sender, RoutedEventArgs e) => Network.ReadWeights();
        
        private void DragWindow(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e); 
            DragMove();
        }
    }
}