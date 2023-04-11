using System.Collections.Concurrent;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.DECONVOLUTION.SCRIPTS;

public static class Deconvolution {
    public static Matrix GetDeconvolution(Matrix matrix, Matrix filter, int stride, double bias) {
        var outputHeight = (matrix.Rows - 1) * stride + filter.Rows; 
        var outputWidth = (matrix.Columns - 1) * stride + filter.Columns; 
        
        var output = new double[outputHeight, outputWidth]; 
        
        for (var i = 0; i < matrix.Rows; i++) 
            for (var j = 0; j < matrix.Columns; j++) 
                for (var k = 0; k < filter.Rows; k++) 
                    for (var l = 0; l < filter.Columns; l++) {
                        var row = i * stride + k;
                        var col = j * stride + l;
                        output[row, col] += matrix.Body[i, j] * filter.Body[k, l] + bias;
                    }
        
        return new Matrix(output);
    }
    
    public static Tensor GetDeconvolution(Tensor tensor, Filter[] filters, int stride) {
        var xSize = (tensor.Channels[0].Rows - 1) * stride + filters[0].Channels[0].Rows; 
        var ySize = (tensor.Channels[0].Columns - 1) * stride + filters[0].Channels[0].Columns; 
        
        var tempMatrices = new ConcurrentBag<Matrix>();
        Parallel.For(0, filters.Length, filter => {
            var tempMatrix = new Matrix(xSize, ySize);
            
            for (var j = 0; j < tensor.Channels.Count; j++) {
                tempMatrix += GetDeconvolution(tensor.Channels[j], filters[filter].Channels[j], stride,
                    filters[filter].Bias);
            }
            
            tempMatrices.Add(tempMatrix);
        });
        
        var newTensor = new Tensor(new List<Matrix>());
        for (var i = 0; i < filters.Length; i++)
            newTensor.Channels.Add(tempMatrices.ElementAt(i));
            
        return newTensor;
    }
}