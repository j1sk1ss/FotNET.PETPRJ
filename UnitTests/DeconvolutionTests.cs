using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class DeconvolutionTests {
    private Matrix _matrix = new Matrix(3, 3);
    private Matrix _matrix1 = new Matrix(3, 3);
    private Matrix _filter = new Matrix(2, 2);

    private Matrix Initialize(int maxValue, Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
        for (var j = 0; j < matrix.Columns; j++)
            matrix.Body[i, j] = new Random().Next() % maxValue + 1;

        return matrix;
    }
    
    [Test]
    public void Deconvolution() {
        var firstMatrix = Initialize(10, _matrix);
        var firstFilter = Initialize(5, _filter);
        
        Console.WriteLine("Matrix is:\n" + firstMatrix.Print());
        Console.WriteLine("Filter is:\n" + firstFilter.Print());

        var convolved = FotNET.NETWORK.LAYERS.DECONVOLUTION.SCRIPTS.Deconvolution.GetDeconvolution(firstMatrix, firstFilter, 1, 2);
        
        Console.WriteLine("Convolved matrix:\n" + convolved.Print());
    }

    [Test]
    public void TensorDeconvolution() {
        var firstMatrix = Initialize(10, _matrix);
        var second = Initialize(10, _matrix1);
        var firstFilter = Initialize(5, _filter);

        var firstTensor = new Tensor(new List<Matrix> { second, firstMatrix, firstMatrix, firstMatrix });
        var secondTensor = new Filter(new List<Matrix> { firstFilter, firstFilter, firstFilter, firstFilter });
        
        var answer = FotNET.NETWORK.LAYERS.DECONVOLUTION.SCRIPTS.Deconvolution.GetDeconvolution(firstTensor, 
            new[]{secondTensor}, 1);
        
        Console.WriteLine(answer.Channels[0].Print());
    }
}