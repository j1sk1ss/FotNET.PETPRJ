using NeuroWeb.EXMPL.SCRIPTS;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class ConvolutionLayer {

        public ConvolutionLayer(Configuration configuration, ConvolutionConfiguration convolutionConfiguration) {
            Configuration = configuration;

            Filter.Body = new List<Matrix>();
            for (var i = 0; i < convolutionConfiguration.FilterCount; i++){
                Filter.Body.Add(new Matrix(new double[convolutionConfiguration.FilterColumn, convolutionConfiguration.FilterRow]));
            }
            FilterFillRandom();
        }
        
        public Filter Filter { get; set; }
        public Configuration Configuration { get; set; }

        public void FilterFillRandom() {
            foreach (var matrix in Filter.Body) {
                matrix.FillRandom();
            }
        }

        public double GetFilterValue(int channel, int x, int y) => Filter.Body[channel].Body[x, y];

        public void SetFilterValue(int channel, int x, int y, double value) => Filter.Body[channel].Body[x, y] = value;

        public Tensor GetNextLayer(Tensor layer) {
            var nextLayer = new Tensor(new List<Matrix>());

            foreach (var matrix in layer.Body) {
                nextLayer.Body.Concat(Convolution.GetConvolution(matrix, Filter, 1).Body);
            }

            for (var i = 0; i < nextLayer.Body.Count; i++) {
                nextLayer.Body[i] = NeuronActivate.Activation(nextLayer.Body[i]);
            }

            return GetMaxPooling(nextLayer);
        }

        public Tensor GetMaxPooling(Tensor layer) => Pooling.MaxPool(layer, 2);
    }
}