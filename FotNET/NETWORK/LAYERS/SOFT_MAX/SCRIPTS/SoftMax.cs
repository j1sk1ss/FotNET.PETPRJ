namespace FotNET.NETWORK.LAYERS.SOFT_MAX.SCRIPTS {
    public static class SoftMax {
        public static List<double> Softmax(List<double> input) {
            var result = new List<double>();
            var maxInput = input.Max();
            var sum = 0.0;

            for (var i = 0; i < input.Count; i++) {
                result.Add(Math.Exp(input[i] - maxInput));
                sum += result[i];
            }

            for (var i = 0; i < input.Count; i++)
                result[i] /= sum;

            return result;
        }
    }
}