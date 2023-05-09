using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS {
    /// <summary>
    /// ILayer interface that include methods for layers
    /// </summary>
    public interface ILayer {
        /// <summary>
        /// Put tensor into layer  for calculations
        /// </summary>
        /// <param name="tensor"> Data tensor </param>
        /// <returns> Data tensor for next layer </returns>
        public Tensor GetNextLayer(Tensor tensor);
        
        /// <summary>
        /// Put error tensor into layer for changing weights and calculate next error
        /// </summary>
        /// <param name="error"> Error tensor </param>
        /// <param name="learningRate"> Rate of learning power </param>
        /// <param name="backPropagate"> Should are weights be updated </param>
        /// <returns> Error tensor for next layer </returns>
        public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate);
        
        /// <summary>
        /// Returns any tensor from any layer
        /// </summary>
        /// <returns> Tensor </returns>
        public Tensor GetValues();
        
        /// <summary>
        /// Returns weights of layer
        /// </summary>
        /// <returns> String with weights values </returns>
        public string GetData();
        
        /// <summary>
        /// Put values into weight matrices 
        /// </summary>
        /// <param name="data"> String with weight data </param>
        /// <returns> Mod. string with data for another layers  </returns>
        public string LoadData(string data);
    }
}