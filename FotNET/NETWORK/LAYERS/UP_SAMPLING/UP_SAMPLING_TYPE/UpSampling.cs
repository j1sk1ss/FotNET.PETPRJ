using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE;

public abstract class UpSampling {
    protected abstract Matrix UpSample(Matrix matrix, int scale);

    public Tensor UpSample(Tensor tensor, int scale) {
        var newTensor = new Tensor(new List<Matrix>());

        foreach (var channel in tensor.Channels) 
            newTensor.Channels.Add(UpSample(channel, scale));
        
        return newTensor;
    }

    protected abstract Matrix DownSample(Matrix matrix, int scale);
    
    public Tensor DownSample(Tensor tensor, int scale) {
        var newTensor = new Tensor(new List<Matrix>());

        foreach (var channel in tensor.Channels) 
            newTensor.Channels.Add(DownSample(channel, scale));
        
        return newTensor;
    }
}