using System.Collections.Generic;

using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.SCRIPTS.MATH {
    public static class LossFunction {
        public static Tensor GetLoss(Tensor tensor, int expectedClass) {
            var prediction = tensor.Channels[0].GetAsList().ToArray();
            var error = new List<double>();
                
            for (var i = 0; i < prediction.Length; i++) 
                if (i != expectedClass) error.Add(-NeuronActivate.GetDerivative(prediction[i]));
                else error.Add(1.0 - NeuronActivate.GetDerivative(prediction[i]));
            
            return new Vector(error.ToArray()).AsTensor(1, error.Count, 1);
        }
    }
}