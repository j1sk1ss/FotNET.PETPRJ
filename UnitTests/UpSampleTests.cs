using FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.BICUBIC_INTERPOLATE;
using FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.BILINEAR_INTERPOLATION;
using FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.NEAREST_NEIGHBOR;
using FotNET.NETWORK.MATH.OBJECTS;

namespace UnitTests;

public class UpSampleTests
{
    [Test]
    public void Neighbors() {
        var a = new Matrix(2, 2) {
            Body = {
                [0, 0] = 1,
                [0, 1] = 2,
                [1, 0] = 3,
                [1, 1] = 4
            }
        };
        
        Console.WriteLine(new NearestNeighbor().UpSample(new Tensor(a), 3).Channels[0].Print());
    }

    [Test]
    public void Bilinear() {
        var a = new Matrix(2, 2) {
            Body = {
                [0, 0] = 1,
                [0, 1] = 2,
                [1, 0] = 3,
                [1, 1] = 4
            }
        };
        
        Console.WriteLine(new BilinearInterpolation().UpSample(new Tensor(a), 2).Channels[0].Print());
    }

    [Test]
    public void Bicubic() {
        var a = new Matrix(2, 2) {
            Body = {
                [0, 0] = 1,
                [0, 1] = 2,
                [1, 0] = 3,
                [1, 1] = 4
            }
        };
        
        Console.WriteLine(new BicubicInterpolate().UpSample(new Tensor(a), 2).Channels[0].Print());
    }
}