using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION;

/// <summary>
/// Loss function class
/// </summary>
public abstract class LossFunction {
    protected double GetLoss(Tensor outputTensor, Tensor expectedTensor) {
        var sum = 0d;
        
        for (var channel = 0; channel < outputTensor.Channels.Count; channel++)
            for (var x = 0; x < outputTensor.Channels[channel].Rows; x++)
                for (var y = 0; y < outputTensor.Channels[channel].Columns; y++)
                    sum += Calculate(expectedTensor, outputTensor, channel, x, y);

        return sum / outputTensor.Flatten().Count;
    }

    protected abstract double Calculate(Tensor expected, Tensor predicted, int channel, int x, int y);

    public Tensor GetErrorTensor(Tensor outputTensor, Tensor expectedTensor) {
        var tensor = new Tensor(new List<Matrix>());
        for (var i = 0; i < outputTensor.Channels.Count; i++)
            tensor.Channels.Add(new Matrix(outputTensor.Channels[0].Rows, outputTensor.Channels[0].Columns));

        Parallel.For(0, outputTensor.Channels.Count, channel => {
            for (var i = 0; i < outputTensor.Channels[channel].Rows; i++)
                for (var j = 0; j < outputTensor.Channels[channel].Columns; j++)
                    tensor.Channels[channel].Body[i,j] = Derivation(expectedTensor, outputTensor, channel, i, j);
        });

        return tensor;
    }

    protected abstract double Derivation(Tensor expected, Tensor predicted, int channel, int x, int y);
}