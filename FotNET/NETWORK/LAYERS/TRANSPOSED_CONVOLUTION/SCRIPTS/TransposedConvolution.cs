using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.DECONVOLUTION.SCRIPTS;

public static class TransposedConvolution {
    public static Matrix GetTransposedConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
        var outputHeight = (matrix.Rows - 1) * stride + filter.Rows; 
        var outputWidth = (matrix.Columns - 1) * stride + filter.Columns; 
        
        var output = new double[outputHeight, outputWidth];
        var tempMatrix = new Matrix(output);

        Parallel.For(0, matrix.Rows, j => {
            for (var k = 0; k < matrix.Columns; k++) {
                var multiply = filter * matrix.Body[j, k];
                var secondTempMatrix = new Matrix(outputHeight, outputWidth);

                for (var x = j * stride; x < j * stride + multiply.Rows; x++)
                    for (var y = k * stride; y < k * stride + multiply.Columns; y++)
                        secondTempMatrix.Body[x, y] = multiply.Body[x - j * stride, y - k * stride];

                tempMatrix += secondTempMatrix;
            }
        });
        
        return tempMatrix + bias;
    }
    
    public static Tensor GetTransposedConvolution(Tensor tensor, Filter[] filters, int stride) {
        var xSize = (tensor.Channels[0].Rows - 1) * stride + filters[0].Channels[0].Rows; 
        var ySize = (tensor.Channels[0].Columns - 1) * stride + filters[0].Channels[0].Columns; 
        
        var newTensor = new Tensor(new List<Matrix>());
        for (var i = 0; i < filters.Length; i++)
            newTensor.Channels.Add(new Matrix(0,0));
        
        Parallel.For(0, filters.Length, filter => {
                var tempMatrix = new Matrix(xSize, ySize);
                
                for (var j = 0; j < tensor.Channels.Count; j++) 
                    tempMatrix += GetTransposedConvolution(tensor.Channels[j], filters[filter].Channels[j], stride,
                        filters[filter].Bias);
                
                newTensor.Channels[filter] = tempMatrix;
        });
        
        return newTensor;
    }
}