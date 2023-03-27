using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class ConvolutionTests
{
    private Matrix _matrix = new Matrix(3, 3);
    private Matrix _filter = new Matrix(2, 2);

    private Matrix Initialize(int maxValue, Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = new Random().Next() % maxValue;

        return matrix;
    }

    [Test]
    public void Convolution() {
        var firstMatrix = Initialize(10, _matrix);
        var firstFilter = Initialize(5, _filter);
        
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());
        Console.WriteLine("Filter is:\n" + firstFilter.Print());

        var convolved =
            FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.Convolution.GetConvolution(firstMatrix, firstFilter, 1, 0);
        
        Console.WriteLine("Convolved matrix:\n" + convolved.Print());
    }

    [Test]
    public void Padding() {
        var firstMatrix = Initialize(10, _matrix);
        
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());

        firstMatrix = FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.Padding
            .GetPadding(new Tensor(firstMatrix), 2).Channels[0];
        
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());
    }
    
    [Test]
    public void BackConvolution() {
        var firstMatrix = Initialize(10, _matrix);
        var firstFilter = Initialize(5, _filter);
        
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());
        Console.WriteLine("Filter is:\n" + firstFilter.Print());
        
        var backConvolved =
            FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.Convolution.GetExtendedConvolution(new Tensor(firstMatrix),
                new[] { new Filter(new List<Matrix>{firstFilter}) }, 1);
        
        Console.WriteLine("BackConvolved matrix:\n" + backConvolved.Channels[0].Print());
    }
}