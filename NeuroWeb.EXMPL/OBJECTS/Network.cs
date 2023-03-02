using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;
using NeuroWeb.EXMPL.OBJECTS.FORWARD;
using NeuroWeb.EXMPL.SCRIPTS;
using NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Configuration configuration) {
            Configuration = configuration;

            ConvolutionLayers = new ConvolutionLayer[Configuration.ConvolutionLayouts];
            for (var i = 0; i < ConvolutionLayers.Length; i++)
                ConvolutionLayers[i] =
                    new ConvolutionLayer(Configuration.ConvolutionConfigurations[i]);

            PerceptronLayers = new PerceptronLayer[Configuration.ForwardLayout];
            for (var i = 0; i < Configuration.ForwardLayout - 1; i++)
                PerceptronLayers[i] =
                    new PerceptronLayer(Configuration.NeuronsLayer[i], Configuration.NeuronsLayer[i + 1]);
            PerceptronLayers[^1] = new PerceptronLayer(Configuration.NeuronsLayer[^1]);
        }

        public Configuration Configuration { get; }        
        private Tensor ImageTensor { get; set; }
        private ConvolutionLayer[] ConvolutionLayers { get; }
        public PerceptronLayer[] PerceptronLayers { get; }        
        
        public void InsertInformation(Number number) {
            ImageTensor = new Tensor(number.GetAsMatrix());
        }
        
        public void InsertInformation(Tensor tensor) {
            ImageTensor = new Tensor(tensor.Channels);
        }

        public int ForwardFeed() {
            try {
                foreach (var layer in ConvolutionLayers)
                    ImageTensor = layer.GetNextLayer(ImageTensor);
                
                var perceptronInput = ImageTensor.GetValues().ToArray();
                for (var i = 0; i < PerceptronLayers.Length - 1; i++) {
                    PerceptronLayers[i].Neurons = perceptronInput;
                    perceptronInput = PerceptronLayers[i].GetNextLayer();
                }
                
                PerceptronLayers[^1].Neurons = perceptronInput;
                return GetMaxIndex(perceptronInput);
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
        
        public void BackPropagation(double expectedAnswer, double learningRange) {
            try {
                for (var i = 0; i < PerceptronLayers[^1].Neurons.Length - 1; i++) 
                    if (i != (int)expectedAnswer) 
                        PerceptronLayers[^1].NeuronsError[i] = -PerceptronLayers[^1].Neurons[i] * 
                                                                  NeuronActivate.GetDerivative(PerceptronLayers[^1].Neurons[i]);
                    else PerceptronLayers[^1].NeuronsError[i] = (1.0 - PerceptronLayers[^1].Neurons[i]) * 
                                                                NeuronActivate.GetDerivative(PerceptronLayers[^1].Neurons[i]);

                for (var i = PerceptronLayers.Length - 2; i >= 0; i--) {
                    PerceptronLayers[i].NeuronsError = PerceptronLayers[i].Weights.GetTranspose() * PerceptronLayers[i + 1].NeuronsError;
                    for (var j = 0; j < PerceptronLayers[i].Neurons.Length; j++)
                        PerceptronLayers[i].NeuronsError[j] *= NeuronActivate.GetDerivative(PerceptronLayers[i].NeuronsError[j]);
                }
                
                for (var i = 0; i < PerceptronLayers.Length - 1; ++i)
                    PerceptronLayers[i].SetWeights(learningRange, PerceptronLayers[i + 1]);
                
                var outputTensor = ConvolutionLayers.Last().Output;
                var errorTensor = new Vector(PerceptronLayers[0].NeuronsError).AsTensor(
                    outputTensor.Channels[0].Body.GetLength(0),
                    outputTensor.Channels[0].Body.GetLength(1), outputTensor.Channels.Count);

                for (var i = ConvolutionLayers.Length - 1; i >= 0; i--) {
                    var inputTensor     = ConvolutionLayers[i].Output;
                    var prevErrorTensor = errorTensor;

                    if (prevErrorTensor.Channels.Count != inputTensor.Channels.Count) {
                        prevErrorTensor = prevErrorTensor.Channels.Count < inputTensor.Channels.Count 
                            ? prevErrorTensor.IncreaseChannels(inputTensor.Channels.Count - prevErrorTensor.Channels.Count) 
                            : prevErrorTensor.CropChannels(inputTensor.Channels.Count);
                    }
                    
                    var filterGradientTensor = Convolution.GetConvolution(Padding.GetPadding(inputTensor, 
                        (ConvolutionLayers[i].Filters[0].Channels[0].Body.GetLength(0) - 1)/2), new[] {prevErrorTensor.AsFilter()}, 1);

                    for (var f = 0; f < ConvolutionLayers[i].Filters.Length; f++) {
                        ConvolutionLayers[i].Filters[f]      -= filterGradientTensor * learningRange;
                        ConvolutionLayers[i].Filters[f].Bias -= errorTensor.TensorSum() * learningRange;
                    }
                   
                    errorTensor = prevErrorTensor;
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }

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
                        temp += ConvolutionLayers[layer].Filters[filter].Bias + " ";

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
                        ConvolutionLayers[layer].Filters[filter].Bias = double.Parse(data[position++]);

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
    }
    
    public struct Configuration {
        int Weight;
        int Height;

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