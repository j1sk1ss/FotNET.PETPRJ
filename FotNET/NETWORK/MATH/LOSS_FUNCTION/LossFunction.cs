using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

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
            var output = outputTensor.Flatten();
            var expected = expectedTensor.Flatten();
            var error = output.Select((t, i) => -Derivation(t, expected[i])).ToList();

            return new Vector(error.ToArray()).AsTensor(1, error.Count, 1);
        }
        
        protected abstract double Derivation(double prediction, double expected);
    }
}