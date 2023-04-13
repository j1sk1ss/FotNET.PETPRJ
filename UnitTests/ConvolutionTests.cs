using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.SAME;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.VALID;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class ConvolutionTests {
    private Matrix _matrix = new Matrix(3, 3);
    private Matrix _matrix1 = new Matrix(3, 3);
    private Matrix _filter = new Matrix(2, 2);

    private Matrix Initialize(int maxValue, Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = new Random().Next() % maxValue;

        return matrix;
    }

    [Test]
    public void MatrixConvolution() {
        var firstMatrix = Initialize(10, _matrix);
        var firstFilter = Initialize(5, _filter);
        
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());
        Console.WriteLine("Filter is:\n" + firstFilter.Print());

        var convolved = Convolution.GetConvolution(firstMatrix, firstFilter, 1, 0);
        
        Console.WriteLine("Convolved matrix:\n" + convolved.Print());
    }

    [Test]
    public void TensorConvolution() {
        var firstMatrix = Initialize(10, _matrix);
        var second = Initialize(10, _matrix1);
        var firstFilter = Initialize(5, _filter);

        var firstTensor = new Tensor(new List<Matrix> { second, firstMatrix, firstMatrix, firstMatrix });
        var secondTensor = new Filter(new List<Matrix> { firstFilter, firstFilter, firstFilter, firstFilter });

        Console.WriteLine("Matrix is:\n" + second.Print());
        Console.WriteLine("Matrix is:\n" + firstFilter.Print());
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());
        
        var convolved = Convolution.GetConvolution(firstTensor, new[] { secondTensor }, 1);
        
        Console.WriteLine(convolved.GetInfo());
    }

    [Test]
    public void Padding() {
        var firstMatrix = Initialize(10, _matrix);
        var firstFilter = Initialize(5, _filter);
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());

        var thirdMatrix = new SamePadding().GetPadding(new Tensor(firstMatrix), firstFilter.Rows - 1).Channels[0];
        var ans = Convolution.GetConvolution(firstMatrix, firstFilter, 1, 0);
        var ans1 = Convolution.GetConvolution(thirdMatrix, firstFilter, 1, 0);
        
        Console.WriteLine("Matrix is:\n" + ans.Print());
        Console.WriteLine("Matrix is:\n" + ans1.Print());
    }
}