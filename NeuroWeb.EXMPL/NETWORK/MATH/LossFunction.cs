using System.Collections.Generic;
using NeuroWeb.EXMPL.NETWORK.ACTIVATION.INTERFACES;
using NeuroWeb.EXMPL.NETWORK.OBJECTS;

namespace NeuroWeb.EXMPL.NETWORK.MATH
{
    public static class LossFunction
    {
        public static Tensor GetLoss(Tensor tensor, int expectedClass, IFunction function)
        {
            var prediction = tensor.Channels[0].GetAsList().ToArray();
            var error = new List<double>();

            for (var i = 0; i < prediction.Length; i++)
                if (i != expectedClass) error.Add(-function.Derivation(prediction[i]));
                else error.Add(1.0 - function.Derivation(prediction[i]));

            return new Vector(error.ToArray()).AsTensor(1, error.Count, 1);
        }
    }
}