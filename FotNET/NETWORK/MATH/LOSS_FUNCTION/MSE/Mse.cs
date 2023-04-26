using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.MSE;

/// <summary>
/// Mean square error (MSE)
/// </summary>
public class Mse : LossFunction {
    public override double GetLoss(Tensor outputTensor, Tensor expectedTensor) {
        var sum = 0d;
        
        for (var channel = 0; channel < outputTensor.Channels.Count; channel++)
            for (var x = 0; x < outputTensor.Channels[channel].Rows; x++)
                for (var y = 0; y < outputTensor.Channels[channel].Columns; y++)
                    sum += Math.Pow(expectedTensor.Channels[channel].Body[x, y] - outputTensor.Channels[channel].Body[x, y], 2);
            
        return Loss(sum, outputTensor.Flatten().Count);
    }

    public override Tensor GetErrorTensor(Tensor outputTensor, Tensor expectedTensor) {
        var tensor = new Tensor(new List<Matrix>());
        for (var channel = 0; channel < outputTensor.Channels.Count; channel++) {
            tensor.Channels.Add(new Matrix(outputTensor.Channels[channel].Rows, outputTensor.Channels[channel].Columns));
            for (var i = 0; i < outputTensor.Channels[channel].Rows; i++)
                for (var j = 0; j < outputTensor.Channels[channel].Columns; j++)
                    tensor.Channels[^1].Body[i,j] = 2 * (outputTensor.Channels[channel].Body[i, j] - expectedTensor.Channels[channel].Body[i, j]);
        }

        return tensor;
    }
}