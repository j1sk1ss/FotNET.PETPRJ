using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization;

public interface IWeightsInitialization {
    /// <summary>
    /// Initialization method for creating start values
    /// </summary>
    /// <param name="matrix"> Matrix for initialization </param>
    /// <returns> Matrix after initialization </returns>
    public Matrix Initialize(Matrix matrix);
}