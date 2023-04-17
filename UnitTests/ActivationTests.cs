using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.BINARY_STEP;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.DOUBLE_LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.ELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.GAUSSIAN;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.GELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.HYPERBOLIC_TANGENT;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.IDENTITY;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.PRELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SiLu;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SOFT_PLUS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace UnitTests;

public class ActivationTests {
    [Test]
    public void DoubleLeakyReLu() {
        Console.WriteLine($"{new DoubleLeakyReLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }

    [Test]
    public void Sigmoid() {
        Console.WriteLine($"{new Sigmoid().Derivation(new Tensor(new Matrix(new []{1d,1d})), new Tensor(new Matrix(new []{0.4613, 1000.7}))).Channels[0].Print()}");
    }

    [Test]
    public void ReLu() {
        Console.WriteLine($"{new ReLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }

    [Test]
    public void Tangensoid() {
        Console.WriteLine($"{new HyperbolicTangent().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void Binary() {
        Console.WriteLine($"{new BinaryStep().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void LeakyReLu() {
        Console.WriteLine($"{new LeakyReLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void Elu() {
        Console.WriteLine($"{new ELu(.01d).Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void Gaussian() {
        Console.WriteLine($"{new Gaussian().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void GeLu() {
        Console.WriteLine($"{new GeLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void Identity() {
        Console.WriteLine($"{new Identity().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void PReLU() {
        Console.WriteLine($"{new PReLu(.1).Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void SeLU() {
        Console.WriteLine($"{new SeLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
        
    [Test]
    public void SiLU() {
        Console.WriteLine($"{new SiLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
    
    [Test]
    public void Softplus() {
        Console.WriteLine($"{new SoftPlus().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
}