using FotNET.NETWORK.MATH.Initialization.HE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class MathTests {
    [Test]
    public void MatrixAddMatrix() {
        var firstMatrix = new HeInitialization().Initialize(new Matrix(5, 5));
        var secondMatrix = new HeInitialization().Initialize(new Matrix(5,5));
        
        Console.WriteLine(firstMatrix.Print() + "+ \n" + secondMatrix.Print() + "=>\n");
        firstMatrix += secondMatrix;
        Console.WriteLine(firstMatrix.Print());
    }

    [Test]
    public void MatrixMultiplyNumber() {
        var matrix = new HeInitialization().Initialize(new Matrix(5, 5));
        var number = 4d;
        
        Console.WriteLine(matrix.Print() + "* " + number + "=>\n");
        matrix *= number;
        Console.WriteLine(matrix.Print());
    }

    [Test]
    public void MatrixMultiplyMatrix() {
        var firstMatrix = new HeInitialization().Initialize(new Matrix(5, 5));
        var secondMatrix = new HeInitialization().Initialize(new Matrix(5,5));
        
        Console.WriteLine(firstMatrix.Print() + "* \n" + secondMatrix.Print() + "=>\n");
        firstMatrix *= secondMatrix;
        Console.WriteLine(firstMatrix.Print());
    }

    [Test]
    public void FullMatrixMultiplyMatrix() {
        var firstMatrix = new HeInitialization().Initialize(new Matrix(5, 5));
        var secondMatrix = new HeInitialization().Initialize(new Matrix(5,5));
        
        Console.WriteLine(firstMatrix.Print() + "* \n" + secondMatrix.Print() + "=>\n");
        firstMatrix = Matrix.Multiply(firstMatrix, secondMatrix);
        Console.WriteLine(firstMatrix.Print());
    }

    [Test]
    public void FlipMatrix() {
        var firstMatrix = new HeInitialization().Initialize(new Matrix(5, 5));
        
        Console.WriteLine(firstMatrix.Print() + "=>\n");
        Console.WriteLine(firstMatrix.Flip().Print());
    }
    
    [Test]
    public void VectorAddVector() {
        var firstVector  = new Vector(new[] { 0d, 1d, 2d, 3d, 4d, 5d });
        var secondVector = new Vector(new[] { 0d, 1d, 2d, 3d, 4d, 5d });
        
        Console.WriteLine("First vector is: " + firstVector.Print());
        Console.WriteLine("Second vector is: " + secondVector.Print());

        firstVector = new Vector(firstVector + secondVector);
        
        Console.WriteLine("Addition is: " + firstVector.Print());
    }

    [Test]
    public void VectorAddNumber() {
        var firstVector  = new Vector(new[] { 0d, 1d, 2d, 3d, 4d, 5d });
        var number = 1d;
        
        Console.WriteLine("First vector is: " + firstVector.Print());
        Console.WriteLine("Number is: " + number);

        firstVector += number;
        
        Console.WriteLine("Answer is: " + firstVector.Print());
    }

    [Test]
    public void VectorMultiplyVector() {
        var firstVector  = new Vector(new[] { 0d, 1d, 2d, 3d, 4d, 5d });
        var secondVector = new Vector(new[] { 0d, 1d, 2d, 3d, 4d, 5d });
        
        Console.WriteLine("First vector is: " + firstVector.Print());
        Console.WriteLine("Second vector is: " + secondVector.Print());

        firstVector *= secondVector;
        
        Console.WriteLine("Multiplication is: " + firstVector.Print());
    }

    [Test]
    public void VectorMultiplyNumber() {
        var firstVector  = new Vector(new[] { 0d, 1d, 2d, 3d, 4d, 5d });
        var number = 1d;
        
        Console.WriteLine("First vector is: " + firstVector.Print());
        Console.WriteLine("Number is: " + number);

        firstVector *= number;
        
        Console.WriteLine("Multiplication is: " + firstVector.Print());
    }

    [Test]
    public void VectorToTensor() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        Console.WriteLine("Vector is: " + vector.Print() + "\n");

        var tensor = vector.AsTensor(2, 2, 2);
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }

    [Test]
    public void TensorToVector() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });

        var tensor = vector.AsTensor(2, 2, 2);
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
        
        Console.WriteLine("Vector is: " + new Vector(tensor.Flatten().ToArray()).Print() + "\n");
    }

    [Test]
    public void TensorMinusNumber() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(2, 2, 2);

        var number = 2d;
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
        
        Console.WriteLine("- " + number + "\n");
        
        tensor -= number;
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }

    [Test]
    public void TensorMultiplyNumber() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var tensor = vector.AsTensor(2, 2, 2);

        var number = 2d;
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
        
        Console.WriteLine("* " + number + "\n");
        
        tensor *= number;
        
        Console.WriteLine("Tensor is: " + tensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + tensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + tensor.Channels[1].Print());
    }

    [Test]
    public void TensorAddTensor() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var firstTensor = vector.AsTensor(2, 2, 2);
        var secondTensor = vector.AsTensor(2, 2, 2);
        
        Console.WriteLine("Tensor is: " + firstTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + firstTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + firstTensor.Channels[1].Print());
        
        Console.WriteLine("+\n");
        
        Console.WriteLine("Tensor is: " + secondTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + secondTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + secondTensor.Channels[1].Print());
        
        firstTensor += secondTensor;
        
        Console.WriteLine("Tensor is: " + firstTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + firstTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + firstTensor.Channels[1].Print());
    }

    [Test]
    public void TensorMinusTensor() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var firstTensor = vector.AsTensor(2, 2, 2);
        var secondTensor = vector.AsTensor(2, 2, 2);
        
        Console.WriteLine("Tensor is: " + firstTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + firstTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + firstTensor.Channels[1].Print());
        
        Console.WriteLine("-\n");
        
        Console.WriteLine("Tensor is: " + secondTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + secondTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + secondTensor.Channels[1].Print());
        
        firstTensor -= secondTensor;
        
        Console.WriteLine("Tensor is: " + firstTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + firstTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + firstTensor.Channels[1].Print());
    }
    
    [Test]
    public void TensorMultiplyTensor() {
        var vector = new Vector(new[] { 0d, 1d, 1d, 2d, 3d, 3d, 2d, 1d });
        var firstTensor = vector.AsTensor(2, 2, 2);
        var secondTensor = vector.AsTensor(2, 2, 2);
        
        Console.WriteLine("Tensor is: " + firstTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + firstTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + firstTensor.Channels[1].Print());
        
        Console.WriteLine("*\n");
        
        Console.WriteLine("Tensor is: " + secondTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + secondTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + secondTensor.Channels[1].Print());
        
        firstTensor *= secondTensor;
        
        Console.WriteLine("Tensor is: " + firstTensor.GetInfo() + "\n");
        
        Console.WriteLine("First channel:\n" + firstTensor.Channels[0].Print());
        Console.WriteLine("Second channel:\n" + firstTensor.Channels[1].Print());
    }
}