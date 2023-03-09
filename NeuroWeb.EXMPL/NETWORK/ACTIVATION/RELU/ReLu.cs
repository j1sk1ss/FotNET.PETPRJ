using NeuroWeb.EXMPL.NETWORK.ACTIVATION.INTERFACES;
using NeuroWeb.EXMPL.NETWORK.OBJECTS;

namespace NeuroWeb.EXMPL.NETWORK.ACTIVATION.RELU
{
    public class ReLu : IFunction
    {
        private double Activate(double value) => value switch
        {
            < 0 => value * .01d,
            _ => value
        };

        public double[] Activate(double[] value)
        {
            for (var i = 0; i < value.Length; i++)
                value[i] = Activate(value[i]);

            return value;
        }

        private Matrix Activate(Matrix matrix)
        {
            for (var i = 0; i < matrix.Body.GetLength(0); i++)
                for (var j = 0; j < matrix.Body.GetLength(1); j++)
                    matrix.Body[i, j] = Activate(matrix.Body[i, j]);
            return matrix;
        }

        public Tensor Activate(Tensor tensor)
        {
            for (var i = 0; i < tensor.Channels.Count; i++)
                tensor.Channels[i] = Activate(tensor.Channels[i]);

            return tensor;
        }

        public double Derivation(double value) => value * value < 0 ? .01d : 1;

        public double[] Derivation(double[] values)
        {
            for (var i = 0; i < values.Length; i++)
                values[i] = Derivation(values[i]);
            return values;
        }

        public Tensor Derivation(Tensor tensor)
        {
            foreach (var channel in tensor.Channels)
                for (var x = 0; x < channel.Body.GetLength(0); x++)
                    for (var y = 0; y < channel.Body.GetLength(1); y++)
                        channel.Body[x, y] = Derivation(channel.Body[x, y]);

            return tensor;
        }
    }
}
