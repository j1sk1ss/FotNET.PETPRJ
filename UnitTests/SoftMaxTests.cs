using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class SoftMaxTests {
    [Test]
    public void SoftMax() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(2, 2, 2);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());

        tensor = new SoftMaxLayer().GetNextLayer(tensor);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }
}