using FotNET.DATA;
using FotNET.DATA.DATA_OBJECTS;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.MATH.LOSS_FUNCTION;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK {
    public class Network {
        /// <summary> The main Network class. Uses for creating ur own models. </summary>
        /// <param name="layers"> Layers of model that will be uses for forward feed. </param>
        public Network(List<ILayer> layers) => Layers = layers;
        
        private List<ILayer> Layers { get; set; }

        /// <summary>
        /// Method for getting acces to layers in model
        /// </summary>
        /// <returns> List of layers </returns>
        public List<ILayer> GetLayers() => Layers;

        /// <summary> Forward feed method. Takes data tensor and prefer answer type. </summary>
        /// <param name="data"> Data tensor. Use in-build methods to convert ur data to data tensor. </param>
        /// <param name="answerType"> Answer type that includes value type and index of class type. </param>
        /// <returns> Returns value of predicted class or index of predicted class. </returns>
        public double ForwardFeed(Tensor data, AnswerType answerType) {
            var tensor = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
            return answerType switch {
                AnswerType.Class => tensor.GetMaxIndex(),
                AnswerType.Value => tensor.Flatten()[tensor.GetMaxIndex()],
                _                => 0
            };
        }

        /// <summary> Forward feed method. Takes data tensor. </summary>
        /// <param name="data"> Data tensor. Use in-build methods to convert ur data to data tensor. </param>
        /// <returns> Returns tensor after all layers of model. </returns>
        public Tensor ForwardFeed(Tensor data) =>
            Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
        
        /// <summary> Back propagation method. </summary>
        /// <param name="expectedAnswer"> Index of class, that was expected. </param>
        /// <param name="expectedValue"> Value of class, that was expected. </param>
        /// <param name="errorFunction"> Type of loss function calculation. </param>
        /// <param name="learningRate"> Value of learning rate. </param>
        /// <param name="backPropagate"> Type of back propagation (without or with grad). </param>
        /// <returns> Returns error after all layers of model. </returns>
        public Tensor BackPropagation(double expectedAnswer, double expectedValue, LossFunction errorFunction, double learningRate, bool backPropagate) {
            var expectedTensor = new Tensor(new Matrix(1, Layers[^1].GetValues().Flatten().Count));
            for (var j = 0; j < expectedTensor.Channels[0].Columns; j++)
                if (j == (int)expectedAnswer)
                    expectedTensor.Channels[0].Body[0, j] = expectedValue;
            
            var errorTensor = errorFunction.GetErrorTensor(Layers[^1].GetValues(), expectedTensor);
            for (var i = Layers.Count - 1; i >= 0; i--)
                errorTensor = Layers[i].BackPropagate(errorTensor, learningRate, backPropagate);
            return errorTensor;
        }

        /// <summary> Back propagation method. </summary>
        /// <param name="expectedAnswer"> Tensor of classes, that was expected. </param>
        /// <param name="errorFunction"> Type of loss function calculation. </param>
        /// <param name="learningRate"> Value of learning rate. </param>
        /// <param name="backPropagate"> Type of back propagation (without or with grad). </param>
        /// <returns> Returns error after all layers of model. </returns>
        public Tensor BackPropagation(Tensor expectedAnswer, LossFunction errorFunction, double learningRate, bool backPropagate) {
            var errorTensor = errorFunction.GetErrorTensor(Layers[^1].GetValues(), expectedAnswer);
            for (var i = Layers.Count - 1; i >= 0; i--)
                errorTensor = Layers[i].BackPropagate(errorTensor, learningRate, backPropagate);
            return errorTensor;
        }
        
        /// <summary> Back propagation method. </summary>
        /// <param name="errorTensor"> Tensor of error from another neural networks. </param>
        /// <param name="learningRate"> Value of learning rate. </param>
        /// <param name="backPropagate"> Type of back propagation (without or with grad). </param>
        /// <returns> Returns error after all layers of model. </returns>
        public Tensor BackPropagation(Tensor errorTensor, double learningRate, bool backPropagate) {
            for (var i = Layers.Count - 1; i >= 0; i--)
                errorTensor = Layers[i].BackPropagate(errorTensor, learningRate, backPropagate);
            return errorTensor;
        }
        
        /// <returns> Returns all weights of model like string with whitespace delimiter. </returns>
        public string GetWeights() =>
            Layers.Aggregate(" ", (current, layer) => current + layer.GetData());

        /// <summary> Load weights of model with whitespace delimiter. </summary>
        /// <param name="weights"> String of weights with whitespace delimiter. </param>
        public void LoadWeights(string weights) =>
            Layers.Aggregate(weights, (current, layer) => layer.LoadData(current));

        /// <summary> Method for fitting model. </summary>
        /// <param name="type"> Type of data for fitting. </param>
        /// <param name="path"> Path to CSV file with data. </param>
        /// <param name="config"> Configuration of one piece of CSV data. </param>
        /// <param name="epochs"> Count of epochs. </param>
        /// <param name="lossFunction"> Type of loss function calculation. </param>
        /// <param name="baseLearningRate"> Base learning rate for back propagation. </param>
        public void Fit(IData.Type type, string path, Config config, int epochs, LossFunction lossFunction, double baseLearningRate) =>
             Layers = MODEL.Fit.FitModel(this, Parse(type, path, config), epochs, lossFunction, baseLearningRate).Layers;
        
        /// <summary> Method for testing model. </summary>
        /// <param name="type"> Type of data for fitting. </param>
        /// <param name="path"> Path to CSV file with data. </param>
        /// <param name="config"> Configuration of one piece of CSV data. </param>
        public double Test(IData.Type type, string path, Config config) =>
             MODEL.Test.TestModel(this, Parse(type, path, config));

        private static List<IData> Parse(IData.Type type, string path, Config config) => type switch {
            IData.Type.Array => DATA.CSV.Parser.CsvToArrays(path, config),
            IData.Type.Image => DATA.IMAGE.Parser.FilesToImages(path, config.LabelPath),
            _                => null!
        };
    }
}