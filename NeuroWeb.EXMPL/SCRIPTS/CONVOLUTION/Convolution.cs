using System;
using System.Collections.Generic;
using System.Windows;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION {
    public static class Convolution {
        private static Matrix GetConvolution(Matrix matrix, Matrix filter, int stride, double bias) {
            var xFilterSize = filter.Body.GetLength(0);
            var yFilterSize = filter.Body.GetLength(1);
            
            var matrixSize = matrix.Body.GetLength(0);
            var conMat = new Matrix(matrixSize - xFilterSize + 1, matrixSize - yFilterSize + 1);
            
            for (var i = 0; i < conMat.Body.GetLength(0); i += stride) {
                for (var j = 0; j < conMat.Body.GetLength(1); j += stride) {
                    var subMatrix = matrix.GetSubMatrix(i, j, i + xFilterSize, j + yFilterSize);
                    conMat.Body[i,j] += (filter * subMatrix).GetSum() + bias;
                }
            }
            
            return conMat;
        }
        
        public static Tensor GetConvolution(Tensor tensor, Filter[] filters, int stride) {
            var newTensor = new Tensor(new List<Matrix>());

            for (var i = 0; i < filters.Length; i++)
                for (var j = 0; j < tensor.Channels.Count; j++) 
                    newTensor.Channels.Add(GetConvolution(tensor.Channels[j], filters[i].Channels[j], stride, filters[i].Bias));
              
            return newTensor;
        }
    }
}