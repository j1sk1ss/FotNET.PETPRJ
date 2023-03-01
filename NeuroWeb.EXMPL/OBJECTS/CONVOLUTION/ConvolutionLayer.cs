using System.Collections.Generic;

using NeuroWeb.EXMPL.SCRIPTS;
using NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.OBJECTS.CONVOLUTION {
    public class ConvolutionLayer {

        public ConvolutionLayer(ConvolutionConfiguration convolutionConfiguration) {
            ConvolutionConfiguration = convolutionConfiguration;

            Filters = new Filter[convolutionConfiguration.FilterCount];
            for (var j = 0; j < convolutionConfiguration.FilterCount; j++) {
                Filters[j] = new Filter(new List<Matrix>());
                for (var i = 0; i < convolutionConfiguration.FilterDepth; i++) {
                    Filters[j].Channels.Add(new Matrix(
                        new double[convolutionConfiguration.FilterColumn, convolutionConfiguration.FilterRow]));
                }
            }

            FilterFillRandom();
        }
        
        public Filter[] Filters { get; set; }
        
        public Tensor Output { get; set; }
        
        private ConvolutionConfiguration ConvolutionConfiguration { get; set; }
        
        public void FilterFillRandom() {
            foreach (var filter in Filters)
                foreach (var matrix in filter.Channels)
                    matrix.FillRandom();
        }

        public Filter[] FlipFilters() {
            var newFilters = new Filter[Filters.Length];
            for (var i = 0; i < Filters.Length; i++)
            {
                newFilters[i] = Filters[i].GetFlipped();
            }
            return newFilters;
        }

        public double GetFilterValue(int filter, int channel, int x, int y) 
            => Filters[filter].Channels[channel].Body[x, y];

        public void SetFilterValue(int filter, int channel, int x, int y, double value) 
            => Filters[filter].Channels[channel].Body[x, y] = value;

        public Tensor GetNextLayer(Tensor layer) {
            var nextLayer = new Tensor(new List<Matrix>());

            nextLayer.Channels.AddRange(Convolution.GetConvolution(layer, Filters, ConvolutionConfiguration.Stride).Channels);
            nextLayer = NeuronActivate.Activation(nextLayer);
            
            Output = GetMaxPooling(nextLayer);
            return Output;
        }

        private Tensor GetMaxPooling(Tensor layer) => Pooling.MaxPool(layer, ConvolutionConfiguration.PoolSize);
    }
}