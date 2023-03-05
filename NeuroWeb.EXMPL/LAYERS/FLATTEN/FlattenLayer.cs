using System.Collections.Generic;

using NeuroWeb.EXMPL.INTERFACES;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.MATH;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;

namespace NeuroWeb.EXMPL.LAYERS.FLATTEN {
    public class FlattenLayer : ILayer {
        private Tensor _inputTensor;
        
        public Tensor GetValues() {
            return _inputTensor;
        }
        
        public Tensor GetNextLayer(Tensor tensor) {
            _inputTensor = tensor;
            
            var temp = new List<double>();
            foreach (var matrix in tensor.Channels) temp.AddRange(matrix.GetAsList());            
            
            var newTensor = new Tensor(new Matrix(1, temp.Count));
            for (var i = 0; i < newTensor.Channels[0].Body.GetLength(1); i++)
                newTensor.Channels[0].Body[0, i] = temp[i];

            return newTensor;
        }

        public Tensor BackPropagate(Tensor error) {
            var temp = new List<double>();
            foreach (var matrix in error.Channels) temp.AddRange(matrix.GetAsList());

            return new Vector(temp.ToArray()).AsTensor(_inputTensor.Channels[0].Body.GetLength(0),
                _inputTensor.Channels[0].Body.GetLength(1), _inputTensor.Channels.Count);
        }
    }
}