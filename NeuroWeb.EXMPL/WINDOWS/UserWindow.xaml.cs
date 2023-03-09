using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

using Matrix = NeuroWeb.EXMPL.NETWORK.OBJECTS.Matrix;
using NeuroWeb.EXMPL.SCRIPTS.MNIST;
using NeuroWeb.EXMPL.SCRIPTS.DATA;
using NeuroWeb.EXMPL.NETWORK.LAYERS.INTERFACES;
using NeuroWeb.EXMPL.NETWORK;
using NeuroWeb.EXMPL.NETWORK.OBJECTS;
using NeuroWeb.EXMPL.NETWORK.MATH;
using NeuroWeb.EXMPL.NETWORK.LAYERS.CONVOLUTION;
using NeuroWeb.EXMPL.NETWORK.LAYERS.ACTIVATION;
using NeuroWeb.EXMPL.NETWORK.LAYERS.POOLING;
using NeuroWeb.EXMPL.NETWORK.ACTIVATION.LEAKY_RELU;
using NeuroWeb.EXMPL.NETWORK.LAYERS.FLATTEN;
using NeuroWeb.EXMPL.NETWORK.LAYERS.PERCEPTRON;

namespace NeuroWeb.EXMPL.WINDOWS
{
    public partial class User {
        public User() {
            try {
                var layers = new List<ILayer> {
                    new ConvolutionLayer(6, 5, 5, 1, 1, .0002),
                    new ActivationLayer(new LeakyReLu()),
                    new PoolingLayer(2),
                    new ConvolutionLayer(16, 5, 5, 6, 1, .0002),
                    new ActivationLayer(new LeakyReLu()),
                    new PoolingLayer(2),
                    new FlattenLayer(),
                    new PerceptronLayer(256, 128, .0002),
                    new ActivationLayer(new LeakyReLu()),
                    new PerceptronLayer(128, 10, .0002),
                    new ActivationLayer(new LeakyReLu()),
                    new PerceptronLayer(10, .0002)
                };
                
                Network = new Network(layers, new LeakyReLu());
                InitializeComponent();
                
                Answers = new List<Label> {
                     Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine
                };

                WeightsWorker.LoadData(Network);
                
                Update = new DispatcherTimer {
                    Interval = new TimeSpan(0,0,0,1)
                };
                Update.Tick += AnalyzeUserInput;
                Update.IsEnabled = true;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        private Network Network { get; }
        private DispatcherTimer Update { get; }
        private double[,] Number { get; set; }
        private List<Label> Answers { get; }

        private readonly Brush _userBrush = Brushes.Black;
        
        private int _pred = 1;
        
        private void AnalyzeUserInput(object sender, EventArgs eventArgs) {
            try {
                var renderTargetBitmap = new RenderTargetBitmap(28,28, 6.5d, 6.5d, 
                    PixelFormats.Pbgra32);
                renderTargetBitmap.Render(UserCanvas);

                var writeableBitmap = new WriteableBitmap(renderTargetBitmap);
            
                var matrix = new double[28,28];
                var temp   = "";

                for (var i = 0; i < 28; i++) {
                    for (var j = 0; j < 28; j++) {
                        matrix[i,j] = writeableBitmap.GetPixel(j, i).A / 255d;
                        if (matrix[i, j] > 0) temp += _pred + "  ";
                        else temp += "  " + "  ";
                    }
                
                    temp += "\n";
                } 
                
                for (var i = 0; i < Answers.Count; i++) Answers[i].Content = 
                    $"{Math.Abs(Math.Round(SoftMax.Softmax(Network.Layers[^1].GetValues().Flatten().ToArray())[i] * 100, 1))}%";   // add softmax as layer         
            
                Matrix.Content = temp;
                Number         = matrix;
                
                _pred = Network.ForwardFeed(new Tensor(new Matrix(matrix)));
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
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
                Teaching.LightStudying(Network, new Tensor(new Matrix(Number)), number);
            }
            else MessageBox.Show("Введённое число не корректно!");
            
            Update.IsEnabled = true;
        }

        private void Clear(object sender, RoutedEventArgs e) => UserCanvas.Children.Clear();
        
        private void Exit(object sender, RoutedEventArgs e) => Close();
        
        private void SaveWeights(object sender, RoutedEventArgs e) {
            MessageBox.Show("Сохранение начато...");
            WeightsWorker.ExportData(Network); 
        }

        private void LoadWeights(object sender, RoutedEventArgs e) {
            WeightsWorker.LoadData(Network); 
        }
        
        private void DragWindow(object sender, MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e); 
            DragMove();
        }
    }
}