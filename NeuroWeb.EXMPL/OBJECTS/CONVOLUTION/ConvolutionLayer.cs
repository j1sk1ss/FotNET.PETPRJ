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
                    Filters[j].Bias.Add(0);
                }
            }

            FilterFillRandom();
        }
        
        public Filter[] Filters { get; }
        
        public Tensor Input { get; private set; }
        
        public Tensor NotPooled { get; private set; }
        
        public Tensor Output { get; private set; }

        private ConvolutionConfiguration ConvolutionConfiguration { get; set; }

        private void FilterFillRandom() {
            foreach (var filter in Filters)
                foreach (var matrix in filter.Channels)
                    matrix.FillRandom();
        }

        public static Filter[] FlipFilters(Filter[] filters) {
            var newFilters = new Filter[filters.Length];
            for (var i = 0; i < filters.Length; i++) {
                newFilters[i] = filters[i].GetFlipped();
            }
            return newFilters;
        }

        public static Filter[] GetFiltersWithoutBiases(Filter[] filters) { 
            for (var i = 0; i < filters.Length; i++) {
                filters[i] = new Filter(filters[i].Channels);
            }
            return filters;
        }

        public Tensor GetNextLayer(Tensor layer) {
            Input = layer;
            var nextLayer = new Tensor(new List<Matrix>());

            nextLayer.Channels.AddRange(Convolution.GetConvolution(layer, Filters, ConvolutionConfiguration.Stride).Channels);
            nextLayer = NeuronActivate.Activation(nextLayer);

            NotPooled = nextLayer;
            Output    = GetMaxPooling(nextLayer);
            
            return Output;
        }

        private Tensor GetMaxPooling(Tensor layer) => Pooling.MaxPool(layer, ConvolutionConfiguration.PoolSize);
    }
}