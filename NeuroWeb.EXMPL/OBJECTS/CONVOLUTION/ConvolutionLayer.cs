using System.Collections.Generic;
using System.Linq;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS.CONVOLUTION {
    public class ConvolutionLayer {

        public ConvolutionLayer(int filterCount, int channelSize, ConvolutionConfiguration convolutionConfiguration) {
            Filter = new Filter[filterCount];
            for (var i = 0; i < filterCount; i++) {
                Filter[i].Channels = new List<Matrix>();
                    for (var j = 0; j < channelSize; j++)
                        Filter[j].Channels.Add(new Matrix(
                            new double[convolutionConfiguration.FilterColumn, convolutionConfiguration.FilterRow]));
            }
            
            FilterFillRandom();
        }
        
        public Filter[] Filter { get; set; }
        
        public void FilterFillRandom() {
            for (var i = 0; i < Filter.Length; i++) {
                foreach (var matrix in Filter[i].Channels)
                    matrix.FillRandom();
            }
        }

        public double GetFilterValue(int filter, int channel, int x, int y) 
            => Filter[filter].Channels[channel].Body[x, y];

        public void SetFilterValue(int filter, int channel, int x, int y, double value) 
            => Filter[filter].Channels[channel].Body[x, y] = value;

        public Tensor GetNextLayer(Tensor layer) {
            var nextLayer = new Tensor(new List<Matrix>());

            for (var i = 0; i < Filter.Length; i++) 
                foreach (var matrix in layer.Channels) 
                    nextLayer.Channels.Concat(Convolution.GetConvolution(matrix, Filter[i], 1).Channels);
            
            return GetMaxPooling(nextLayer);
        }

        private static Tensor GetMaxPooling(Tensor layer) => Pooling.MaxPool(layer, 2);
    }
}