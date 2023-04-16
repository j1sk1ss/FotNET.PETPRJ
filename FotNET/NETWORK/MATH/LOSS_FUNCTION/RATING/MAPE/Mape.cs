using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.RATING.MAPE;

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
}