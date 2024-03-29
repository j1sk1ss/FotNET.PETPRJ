﻿namespace FotNET.NETWORK.LAYERS.SOFT_MAX.SCRIPTS {
    public static class SoftMax {
        public static List<double> Softmax(List<double> input) {
            var result = input.Select(t => Math.Exp(t - input.Max())).ToList();
            var sum = result.Sum();
            
            for (var i = 0; i < input.Count; i++)
                result[i] /= sum;

            return result;
        }
    }
}