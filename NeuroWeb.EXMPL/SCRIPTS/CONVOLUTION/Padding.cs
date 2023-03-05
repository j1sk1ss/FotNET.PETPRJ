using NeuroWeb.EXMPL.OBJECTS;
using System;
using System.Collections.Generic;
using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION {
    internal static class Padding {

        public static Matrix GetPadding(Matrix matrix, int paddingSize) {
            var newMatrix = new Matrix(matrix.Body.GetLength(0) + paddingSize * 2, matrix.Body.GetLength(0) + paddingSize * 2);

            for (var i = Math.Abs(paddingSize); i < newMatrix.Body.GetLength(0) - Math.Abs(paddingSize); i++)
                for (var j = Math.Abs(paddingSize); j < newMatrix.Body.GetLength(1) - Math.Abs(paddingSize); j++)
                    newMatrix.Body[i, j] = matrix.Body[i - paddingSize, j - paddingSize];

            return newMatrix;
        }

        public static Tensor GetPadding(Tensor tensor, int paddingSize) {
            var newTensor = new Tensor(new List<Matrix>());
            for (var i = 0; i < tensor.Channels.Count; i++) {
                newTensor.Channels.Add(GetPadding(tensor.Channels[i], paddingSize));
            }
            return newTensor;
        }

    }
}
