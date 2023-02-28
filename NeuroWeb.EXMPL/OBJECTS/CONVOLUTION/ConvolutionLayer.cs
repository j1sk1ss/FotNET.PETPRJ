using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.SCRIPTS;
using NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.OBJECTS.CONVOLUTION {
    public class ConvolutionLayer {

        public ConvolutionLayer(ConvolutionConfiguration convolutionConfiguration) {
            ConvolutionConfiguration = convolutionConfiguration;
            
            Filters = new Filter(new List<Matrix>());
            for (var j = 0; j < convolutionConfiguration.FilterCount; j++)
                Filters.Channels.Add(new Matrix(
                    new double[convolutionConfiguration.FilterColumn, convolutionConfiguration.FilterRow]));
            for (var j = 0; j < convolutionConfiguration.FilterCount; j++)
                Filters.Bias.Add(0);
            
            
            FilterFillRandom();
        }
        
        public Filter Filters { get; set; }
        
        public Tensor Output { get; set; }
        
        private ConvolutionConfiguration ConvolutionConfiguration { get; set; }
        
        public void FilterFillRandom() {
            foreach (var matrix in Filters.Channels)
                matrix.FillRandom();
        }

        public double GetFilterValue(int filter, int channel, int x, int y) 
            => Filters.Channels[channel].Body[x, y];

        public void SetFilterValue(int filter, int channel, int x, int y, double value) 
            => Filters.Channels[channel].Body[x, y] = value;

        public Tensor GetNextLayer(Tensor layer) {
            var nextLayer = new Tensor(new List<Matrix>());
            
            foreach (var matrix in layer.Channels) 
                nextLayer.Channels.AddRange(Convolution.GetConvolution(matrix, Filters, ConvolutionConfiguration.Stride).Channels);
            
            Output = nextLayer;
            return GetMaxPooling(nextLayer);
        }

        private Tensor GetMaxPooling(Tensor layer) => Pooling.MaxPool(layer, ConvolutionConfiguration.PoolSize);
    }
}