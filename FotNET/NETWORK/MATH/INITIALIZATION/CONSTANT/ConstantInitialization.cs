using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.CONSTANT;

public class ConstantInitialization : IWeightsInitialization {
    /// <summary>
    /// Constant initialization is initialization where all weights sets by one value
    /// </summary>
    /// <param name="value"> Start value for all weights </param>
    public ConstantInitialization(double value) => Value = value;
        
    private double Value { get; }
    
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
                matrix.Body[i, j] = Value;

        return matrix;
    }
}