﻿using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.FLATTEN {
    /// <summary> Layer that convert multi-dimension data tensor to 1D vector-data tensor. </summary>
    public class FlattenLayer : ILayer {
        private Tensor _inputTensor = new (new Matrix(0,0));

        public Tensor GetValues() => _inputTensor;

        public Tensor GetNextLayer(Tensor tensor) {
            _inputTensor = tensor.Copy();
            return new Vector(tensor.Flatten().ToArray()).AsTensor(1, tensor.Flatten().Count, 1);
        }

        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
             new Vector(error.Flatten().ToArray()).AsTensor(_inputTensor.Channels[0].Rows,
                _inputTensor.Channels[0].Columns, _inputTensor.Channels.Count);

        public string GetData() => "";
        
        public string LoadData(string data) => data;
    }
}