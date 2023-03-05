using System;
using System.Collections.Generic;
using System.Windows;

using NeuroWeb.EXMPL.INTERFACES;
using NeuroWeb.EXMPL.LAYERS.CONVOLUTION;
using NeuroWeb.EXMPL.LAYERS.FLATTEN;
using NeuroWeb.EXMPL.LAYERS.PERCEPTRON;
using NeuroWeb.EXMPL.LAYERS.POOLING;
using NeuroWeb.EXMPL.SCRIPTS.MATH;

namespace NeuroWeb.EXMPL.OBJECTS.NETWORK {
    public class Network {
        public Network(Configuration configuration) {
            Configuration = configuration;
            Layers = new List<ILayer> {
                new ConvolutionLayer(6, 5, 5, 1, 1, .005),
                new PoolingLayer(2),
                new ConvolutionLayer(16, 5, 5, 6, 1, .005),
                new PoolingLayer(2),
                new FlattenLayer(),
                new PerceptronLayer(256, 128, .005),
                new PerceptronLayer(128, 10, .005),
                new PerceptronLayer(10, .005)
            };
        }
        public Configuration Configuration { get; }
        private Tensor ImageTensor { get; set; }
        public List<ILayer> Layers { get; }

        public void InsertInformation(Number number) {
            ImageTensor = new Tensor(number.GetAsMatrix());
        }
        
        public void InsertInformation(Tensor tensor) {
            ImageTensor = new Tensor(tensor.Channels);
        }

        public int ForwardFeed() {
            try {
                foreach (var layer in Layers) 
                    ImageTensor = layer.GetNextLayer(ImageTensor);
                return GetMaxIndex(ImageTensor.Flatten());
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }

        private static int GetMaxIndex(IReadOnlyList<double> values) {
            var max   = values[0];
            var index = 0;
            
            for (var i = 0; i < values.Count; i++)
                if (max < values[i]) {
                    max   = values[i];
                    index = i;
                }

            return index;
        }
        
        public void BackPropagation(double expectedAnswer) {
            try {
                var errorTensor = LossFunction.GetLoss(Layers[^1].GetValues(), (int)expectedAnswer);
                for (var i = Layers.Count - 2; i >= 0; i--) {
                    errorTensor = Layers[i].BackPropagate(errorTensor);
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
        /*
        private static string _weights;
        private static string GetWeights() {
            var defaultWeights = Properties.Resources.defaultWeights;

            var file = new OpenFileDialog {
                Filter = "TXT files | *.txt"
            };
            var message = MessageBox.Show("Использовать стандартные веса вместо " +
                                          "других", "Укажите файл весов!", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes) return defaultWeights;
            _weights = file.FileName;
            return file.ShowDialog() != true ? "" : File.ReadAllText(file.FileName);
        }
        
        public void SaveWeights() {
            try {
                MessageBox.Show("Начата запись весов!");
                var temp = "";

                for (var layer = 0; layer < ConvolutionLayers.Length; layer++)
                    for (var filter = 0; filter < ConvolutionLayers[layer].Filters.Length; filter++)
                        for (var channel = 0; channel < ConvolutionLayers[layer].Filters[filter].Channels.Count; channel++)
                            temp += ConvolutionLayers[layer].Filters[filter].Channels[channel].GetValues();

                for (var layer = 0; layer < ConvolutionLayers.Length; layer++)
                    for (var filter = 0; filter < ConvolutionLayers[layer].Filters.Length; filter++)
                        for (var bias = 0; bias < ConvolutionLayers[layer].Filters[filter].Bias.Count; bias++)
                            temp += ConvolutionLayers[layer].Filters[filter].Bias[bias] + " ";

                for (var layer = 0; layer < PerceptronLayers.Length - 1; layer++)
                    temp += PerceptronLayers[layer].Weights.GetValues();

                for (var layer = 0; layer < PerceptronLayers.Length - 1; layer++)
                    for (var bias = 0; bias < PerceptronLayers[layer].Bias.Length; bias++)
                        temp += PerceptronLayers[layer].Bias[bias] + " ";


                if (File.Exists(_weights)) File.WriteAllText(_weights, temp);
                else {
                    var file = new SaveFileDialog {
                        Filter = "TXT files | *.txt"
                    };
                    MessageBox.Show("Укажите место для сохранения весов!");
                    if (file.ShowDialog() == true) File.WriteAllText(file.FileName, temp);
                    return;
                }
                MessageBox.Show("Веса обновлены!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при записи весов!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }

        public void ReadWeights() {
            try {
                var data = GetWeights().Split(" ", 
                    StringSplitOptions.RemoveEmptyEntries);
                
                if (data.Length < 1) {
                    MessageBox.Show("Веса не загружены!", "Внимание!", MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return;
                }
                
                var position = 0;

                for (var layer = 0; layer < ConvolutionLayers.Length; layer++)
                    for (var filter = 0; filter < ConvolutionLayers[layer].Filters.Length; filter++)
                        for (var channel = 0; channel < ConvolutionLayers[layer].Filters[filter].Channels.Count; channel++)
                            for (var x = 0; x < ConvolutionLayers[layer].Filters[filter].Channels[channel].Body.GetLength(0); x++)
                                for (var y = 0; y < ConvolutionLayers[layer].Filters[filter].Channels[channel].Body.GetLength(1); y++)
                                    ConvolutionLayers[layer].Filters[filter].Channels[channel].Body[x, y] = double.Parse(data[position++]);

                for (var layer = 0; layer < ConvolutionLayers.Length; layer++)
                    for (var filter = 0; filter < ConvolutionLayers[layer].Filters.Length; filter++)
                        for (var bias = 0; bias < ConvolutionLayers[layer].Filters[filter].Bias.Count; bias++)
                        ConvolutionLayers[layer].Filters[filter].Bias[bias] = double.Parse(data[position++]);

                for (var layer = 0; layer < PerceptronLayers.Length - 1; layer++)
                    for (var x = 0; x < PerceptronLayers[layer].Weights.Body.GetLength(0); x++)
                        for (var y = 0; y < PerceptronLayers[layer].Weights.Body.GetLength(1); y++)
                            PerceptronLayers[layer].Weights.Body[x, y] = double.Parse(data[position++]);

                for (var layer = 0; layer < PerceptronLayers.Length - 1; layer++)
                    for (var bias = 0; bias < PerceptronLayers[layer].Bias.Length; bias++)
                        PerceptronLayers[layer].Bias[bias] = double.Parse(data[position++]);

                if (position < data.Length) MessageBox.Show("Веса считанны некорректно или не считанны",
                    "Предупреждение!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при чтении весов!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }
        */
    }
    
    public struct Configuration {
        public int Weight;
        public int Height;

        public int ConvolutionLayouts;
        public int ForwardLayout;

        public ConvolutionConfiguration[] ConvolutionConfigurations;

        public int[] NeuronsLayer;
    }

    public struct ConvolutionConfiguration {
        public int FilterColumn;
        public int FilterRow;
        public int FilterDepth;

        public int FilterCount;
        public int PoolSize;
        public int Stride;
    }
    
}