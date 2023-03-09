namespace FotNET.NETWORK.MATH {
    public static class SoftMax {
        public static double[] Softmax(double[] input) {
            var result = new double[input.Length];
            var maxInput = input.Max();
            var sum = 0.0;

            for (var i = 0; i < input.Length; i++) {
                result[i] = Math.Exp(input[i] - maxInput);
                sum += result[i];
            }

            for (var i = 0; i < input.Length; i++)
                result[i] /= sum;

            return result;
        }
    }
}