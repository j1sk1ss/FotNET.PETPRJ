using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.LEAKY_RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.RELU;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.TANGENSOID;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class ActivationTests {
    [Test]
    public void LeakyReLu() {
        Console.WriteLine($"{new LeakyReLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }

    [Test]
    public void Sigmoid() {
        Console.WriteLine($"{new Sigmoid().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }

    [Test]
    public void ReLu() {
        Console.WriteLine($"{new ReLu().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }

    [Test]
    public void Tangensoid() {
        Console.WriteLine($"{new Tangensoid().Activate(new Tensor(new Matrix(new []{-2d, -.5d, 0d, .1d, .6d, 1d, 2d}))).Channels[0].Print()}");
    }
}