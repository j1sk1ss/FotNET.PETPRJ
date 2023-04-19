using FotNET.NETWORK.MATH.LOSS_FUNCTION.RATING;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.MAPE;

/// <summary>
/// Mean absolute percent error (MAPE)
/// </summary>
public class Mape : LossFunction {
    public override double GetLoss(Tensor outputTensor, Tensor expectedTensor) {
        var sum = 0d;
        
        for (var channel = 0; channel < outputTensor.Channels.Count; channel++)
            for (var x = 0; x < outputTensor.Channels[channel].Rows; x++)
                for (var y = 0; y < outputTensor.Channels[channel].Columns; y++)
                    sum += Math.Abs((expectedTensor.Channels[channel].Body[x, y] - outputTensor.Channels[channel].Body[x, y]) 
                                    / outputTensor.Channels[channel].Body[x, y]);
            
        return Loss(sum, outputTensor.Flatten().Count);
    }

    public override Tensor GetErrorTensor(Tensor outputTensor, Tensor expectedTensor) {
        var tensor = new Tensor(new List<Matrix>());
        for (var channel = 0; channel < outputTensor.Channels.Count; channel++) {
            tensor.Channels.Add(new Matrix(outputTensor.Channels[channel].Rows, outputTensor.Channels[channel].Columns));
            for (var i = 0; i < outputTensor.Channels[channel].Rows; i++)
            for (var j = 0; j < outputTensor.Channels[channel].Columns; j++)
                tensor.Channels[^1].Body[i,j] = (outputTensor.Channels[channel].Body[i, j] 
                                                 - expectedTensor.Channels[channel].Body[i, j]) / expectedTensor.Channels[channel].Body[i, j];
        }

        return tensor;
    }
}