using System;
using System.Collections.Generic;
using System.Linq;

using NeuroWeb.EXMPL.INTERFACES;
using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;
using NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION;
using NeuroWeb.EXMPL.SCRIPTS.MATH; 

namespace NeuroWeb.EXMPL.LAYERS.CONVOLUTION {
    public class ConvolutionLayer : ILayer {
        public ConvolutionLayer(int filters, int filterWeight, int filterHeight, int filterDepth, int stride, double learningRate) {
            Filters = new Filter[filters];
            for (var j = 0; j < filters; j++) {
                Filters[j] = new Filter(new List<Matrix>());
                Filters[j].Bias = new Random().Next() % 50 * .06 / (j * 10 + 15);

                for (var i = 0; i < filterDepth; i++) 
                    Filters[j].Channels.Add(new Matrix(
                        new double[filterWeight, filterHeight]));                
            }

            foreach (var filter in Filters)
                foreach (var matrix in filter.Channels)
                    matrix.FillRandom();
            
            _learningRate = learningRate;
            _stride       = stride;
        }

        private readonly double _learningRate;
        private readonly int _stride;

        private Filter[] Filters { get; }
        private Tensor Input { get; set; }
        
        private static Filter[] FlipFilters(IReadOnlyList<Filter> filters) {
            var newFilters = new Filter[filters.Count];
            for (var i = 0; i < filters.Count; i++) {
                newFilters[i] = filters[i].GetFlip();
            }
            return newFilters;
        }

        private static Filter[] GetFiltersWithoutBiases(Filter[] filters) { 
            for (var i = 0; i < filters.Length; i++) {
                filters[i] = new Filter(filters[i].Channels);
            }
            return filters;
        }

        public Tensor GetValues() => Input;
        
        public Tensor GetNextLayer(Tensor layer) {
            Input = layer;
            var nextLayer = NeuronActivate.LeakyReLu(Convolution.GetConvolution(layer, Filters, _stride));
            return nextLayer;
        }

        public Tensor BackPropagate(Tensor error) {
            var inputTensor = Input;
            
            var extendedInput   = inputTensor.GetSameChannels(error);
            var originalFilters = Filters;
            for (var i = 0; i < originalFilters.Length; i++) 
                originalFilters[i] = originalFilters[i].GetSameChannels(error).AsFilter();
                              
            for (var f = 0; f < Filters.Length; f++) {
                for (var channel = 0; channel < Filters[f].Channels.Count; channel++) {
                    var channelGradient = Convolution.GetConvolution(extendedInput.Channels[f],
                        error.Channels[f], _stride, Filters[f].Bias);

                    Filters[f].Channels[channel] -= channelGradient * _learningRate;
                    Filters[f].Bias -= error.Channels[f].GetSum() * _learningRate;
                }
            }

            var nextError = Convolution.GetExtendedConvolution(error,
                FlipFilters(GetFiltersWithoutBiases(originalFilters)), 1);
            
            return NeuronActivate.GetDerivative(nextError);  
        }

        public string GetData() {
            var temp = "";
            foreach (var filter in Filters) {
                temp = filter.Channels.Aggregate(temp, (current, channel) => current + channel.GetValues());
                temp += filter.Bias + " ";
            }
            return temp;
        }

        public string LoadData(string data) {
            var position = 0;
            var dataNumbers = data.Split(" ");

            foreach (var filter in Filters) {
                foreach (var channel in filter.Channels) 
                    for (var x = 0; x < channel.Body.GetLength(0); x++) 
                        for (var y = 0; y < channel.Body.GetLength(1); y++) 
                            channel.Body[x, y] = double.Parse(dataNumbers[position++]);
                                                                          
                filter.Bias = double.Parse(dataNumbers[position++]);
            }

            return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
        }
    }
}