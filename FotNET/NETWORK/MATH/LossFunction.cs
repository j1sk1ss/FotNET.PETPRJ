using FotNET.NETWORK.OBJECTS;

namespace FotNET.NETWORK.MATH {
    public static class LossFunction {
        public static Tensor GetErrorTensor(Tensor outputTensor, int expectedClass) {
            var prediction = outputTensor.Channels[0].GetAsList().ToArray();
            var error = new List<double>();

            for (var i = 0; i < prediction.Length; i++)
                if (i != expectedClass) error.Add(-Derivation(prediction[i], 0));
                else error.Add(-Derivation(prediction[i], 1));
            
            return new Vector(error.ToArray()).AsTensor(1, error.Count, 1);
        }

        private static double Derivation(double prediction, double expected) =>
             prediction * (1 - prediction) * (expected - prediction);
    }
}