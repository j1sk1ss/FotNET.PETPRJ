using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.AVERAGE;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.BILINEAR;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MAX;
using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.MIN;
using FotNET.NETWORK.MATH.OBJECTS;
using NUnit.Framework;

namespace UnitTests;

public class PoolingTests {
    [Test]
    public void MaxPooling() {
        var maxPool = new MaxPooling();
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d, 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d, 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d, 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(4, 4, 2);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());

        tensor = maxPool.Pool(tensor, 4);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }

    [Test]
    public void BackMaxPooling() {
        var maxPool = new MaxPooling();
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d, 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d, 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d, 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(4, 4, 2);
        var newTensor = new Tensor(new List<Matrix>(tensor.Channels));

        maxPool.Pool(newTensor, 4);
        
        Console.WriteLine("Tensor is: " + newTensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + newTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + newTensor.Channels[1].Print());

        newTensor = maxPool.BackPool(newTensor, tensor, 4);
        
        Console.WriteLine("Tensor is: " + newTensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + newTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + newTensor.Channels[1].Print());
    }

    [Test]
    public void MinPooling() {
        var maxPool = new MinPooling();
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(2, 2, 2);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());

        tensor = maxPool.Pool(tensor, 2);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }

    [Test]
    public void BackMinPooling() {
        var maxPool = new MinPooling();
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(2, 2, 2);
        var newTensor = new Tensor(new List<Matrix>(tensor.Channels));
        maxPool.Pool(newTensor, 2);
        
        Console.WriteLine("Tensor is: " + newTensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + newTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + newTensor.Channels[1].Print());

        newTensor = maxPool.BackPool(newTensor, tensor, 2);
        
        Console.WriteLine("Tensor is: " + newTensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + newTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + newTensor.Channels[1].Print());
    }
    
    [Test]
    public void AveragePooling() {
        var maxPool = new AveragePooling();
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(2, 2, 2);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());

        tensor = maxPool.Pool(tensor, 2);
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }

    [Test]
    public void BackAveragePooling() {
        var maxPool = new AveragePooling();
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        
        var tensor = vector.AsTensor(2, 2, 2);
        var newTensor = new Tensor(new List<Matrix>(tensor.Channels));
        maxPool.Pool(newTensor, 2);
        
        Console.WriteLine("Tensor is: " + newTensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + newTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + newTensor.Channels[1].Print());

        newTensor = maxPool.BackPool(newTensor, tensor, 2);
        
        Console.WriteLine("Tensor is: " + newTensor.GetInfo() + "\n");
        Console.WriteLine("First channel:\n" + newTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + newTensor.Channels[1].Print());
    }

    [Test]
    public void BilinearPooling() {
        var maxPool = new BilinearPooling();
        var vector = new Vector(new[] { 2d, 3d, 1d, 4d, 6d, 2d, 4d, 3d,  5d, 4d, 2d, 1d, 1d, 5d, 6d, 4d });
        
        var tensor = vector.AsTensor(4, 4, 1);
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        tensor = maxPool.Pool(tensor, 2);
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("First channel:\n" + maxPool.BackPool(tensor, tensor, 2).Channels[0].Print());
    }
}