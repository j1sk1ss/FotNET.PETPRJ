using FotNET.NETWORK.LAYERS.CONVOLUTION.ADAM;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION {
    public class ConvolutionLayer : ILayer {
        /// <summary> Layer that perform tensor convolution by filters and biases. </summary>
        /// <param name="filters"> Count of filters on layer. </param>
        /// <param name="filterShape"> Shape of filters </param>
        /// <param name="weightsInitialization"> Type of weights initialization of filters on layer. </param>
        /// <param name="stride"> Stride of convolution. </param>
        /// <param name="padding"> Padding type. </param>
        /// <param name="convolutionOptimization"> Optimization type. </param>
        public ConvolutionLayer(int filters, (int Weight, int Height, int Depth) filterShape, 
            IWeightsInitialization weightsInitialization, int stride, Padding padding,
            ConvolutionOptimization convolutionOptimization) {
            ConvolutionOptimization = convolutionOptimization;
            
            _backPropagate = true;
            _padding       = padding;
            Filters        = new Filter[filters];
            
            for (var j = 0; j < filters; j++) {
                Filters[j] = new Filter(new List<Matrix>()) {
                    Bias = .001d
                };

                for (var i = 0; i < filterShape.Depth; i++)
                    Filters[j].Channels.Add(new Matrix(
                        new double[filterShape.Weight, filterShape.Height]));
            }

            foreach (var filter in Filters)
                for (var i = 0; i < filter.Channels.Count; i++)
                    filter.Channels[i] = weightsInitialization.Initialize(filter.Channels[i]);

            _stride = stride;
            Input   = new Tensor(new Matrix(0, 0));
        }

        /// <summary> Layer that perform tensor convolution by filters and biases. </summary>
        /// <param name="filtersPath"> Path to .txt file of filters. </param>
        /// <param name="filterDepth"> Depth of filters on layer. </param>
        /// <param name="stride"> Stride of convolution. </param>
        /// <param name="padding"> Padding type. </param>
        /// <param name="convolutionOptimization"> Optimization type. </param>
        public ConvolutionLayer(string filtersPath, int filterDepth, int stride, Padding padding,
            ConvolutionOptimization convolutionOptimization) {
            ConvolutionOptimization = convolutionOptimization;
            var filters = File.ReadAllText(filtersPath).Split("/", StringSplitOptions.RemoveEmptyEntries);
            
            _backPropagate = false;
            _padding       = padding;
            Filters        = new Filter[filters.Length];
            
            for (var j = 0; j < filters.Length; j++) {
                Filters[j] = new Filter(new List<Matrix>()) {
                    Bias = .001d
                };

                for (var i = 0; i < filterDepth; i++)
                    Filters[j].Channels.Add(new Matrix(filters[j]));
            }

            _stride = stride;
            Input   = new Tensor(new Matrix(0, 0));
        }
        
        private readonly int _stride;
        private readonly bool _backPropagate;
        private readonly Padding _padding;

        private Filter[] Filters { get; }
        private Tensor Input { get; set; }
        private ConvolutionOptimization ConvolutionOptimization { get; }

        public Tensor GetValues() => Input;

        public Tensor GetNextLayer(Tensor tensor) {
            Input = _padding.GetPadding(tensor.Copy());
            return Convolution.GetConvolution(_padding.GetPadding(tensor.Copy()), Filters, _stride);
        }

        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
            ConvolutionOptimization.BackPropagate(error, learningRate, backPropagate, Input, Filters, _backPropagate,
                _stride);

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
            var dataNumbers = data.Split(" ",  StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in Filters) {
                foreach (var channel in filter.Channels)
                    for (var x = 0; x < channel.Rows; x++)
                        for (var y = 0; y < channel.Columns; y++)
                            channel.Body[x, y] = double.Parse(dataNumbers[position++]);

                filter.Bias = double.Parse(dataNumbers[position++]);
            }

            return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
        }
    }
}