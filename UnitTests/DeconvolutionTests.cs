using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace UnitTests;

public class DeconvolutionTests {
    private Matrix _matrix = new Matrix(3, 3);
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

        var convolved = FotNET.NETWORK.LAYERS.DECONVOLUTION.SCRIPTS.Deconvolution.GetDeconvolution(firstMatrix, firstFilter, 2, 2);
        
        Console.WriteLine("Convolved matrix:\n" + convolved.Print());
    }
}