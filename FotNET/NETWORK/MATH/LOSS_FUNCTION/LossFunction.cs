using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION {
    public abstract class LossFunction {
        public Tensor GetErrorTensor(Tensor outputTensor, int expectedClass, double expectedValue) {
            var prediction = outputTensor.Channels[0].GetAsList().ToArray();
            var error = new List<double>();

            for (var i = 0; i < prediction.Length; i++)
                if (i != expectedClass) error.Add(-Derivation(prediction[i], 0));
                else error.Add(-Derivation(prediction[i], expectedValue));
            
            return new Vector(error.ToArray()).AsTensor(1, error.Count, 1);
        }

        public Tensor GetErrorTensor(Tensor outputTensor, Tensor expectedTensor) {
            var outputTensorError = new Tensor(new List<Matrix>(outputTensor.Channels));
            
            for (var channel = 0; channel < outputTensorError.Channels.Count; channel++)
                for (var x = 0; x < outputTensorError.Channels[channel].Rows; x++)
                    for (var y = 0; y < outputTensorError.Channels[channel].Columns; y++)
                        outputTensorError.Channels[channel].Body[x, y] = -Derivation(
                            outputTensor.Channels[channel].Body[x, y], expectedTensor.Channels[channel].Body[x, y]);
            
            return outputTensorError;
        }
        
        protected abstract double Derivation(double prediction, double expected);
    }
}