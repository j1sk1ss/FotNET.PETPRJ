using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.BATCH_NORMALIZATION;

public class BatchNormalization : ILayer {
    /// <summary>
    /// Batch normalization layer
    /// </summary>
    /// <param name="size"> Size of input data </param>
    public BatchNormalization(int size) {
        Input = new Tensor(0,0,0);
        
        Gamma       = new Vector(size);
        Beta        = new Vector(size);
        Mean        = new Vector(size);
        Variance    = new Vector(size);
        XNormalized = new Vector(size);

        for (var i = 0; i < size; i++) {
            Gamma[i]    = 1;
            Variance[i] = 1;
        }
    }
    
    private Vector Gamma { get; set; }
    private Vector Beta { get; }
    private Vector Mean { get; }
    private Vector Variance { get; }
    private Vector XNormalized { get; }
    
    private Tensor Input { get; set; }
    
    public Tensor GetNextLayer(Tensor tensor) {
        var input = tensor.Flatten().ToArray();
        var sum = input.Sum();

        for (var i = 0; i < input.Length; i++) 
            Mean[i] = sum / input.Length;
        
        sum = input.Select((t, i) => Math.Pow(t - Mean[i], 2)).Sum();

        for (var i = 0; i < input.Length; i++) {
            Variance[i] = sum / input.Length;
            XNormalized[i] = (input[i] - Mean[i]) / Math.Sqrt(Variance[i] + double.Epsilon);    
            input[i] = Gamma[i] * XNormalized[i] + Beta[i];            
        }

        Input = tensor;
        
        return new Vector(input).AsTensor(tensor.Channels[0].Rows, tensor.Channels[0].Columns,
            tensor.Channels.Count);
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        var x = Input.Flatten().ToArray();
        var input = error.Flatten().ToArray();
        var gammaGradient = new Vector(input.Length);
        
        var dxNormalized = new Vector(input.Length);
        var dVariance    = new double[input.Length];
        var dMean        = new double[input.Length];
        
        for (var i = 0; i < input.Length; i++) {
            gammaGradient[i] += input[i] * XNormalized[i];
            dxNormalized[i] = input[i] * Gamma[i];
            dVariance[i] = dxNormalized[i] * (x[i] - Mean[i]) * -.5d * Math.Pow(Variance[i] + double.Epsilon, -1.5d);
            dMean[i] = -dxNormalized[i] / Math.Sqrt(Variance[i] + double.Epsilon);
        }
        
        var dVarianceSum = dVariance.Sum();
        for (var i = 0; i < input.Length; i++)
            dMean[i] += dVarianceSum * 2 * (x[i] - Mean[i]) / input.Length;

        var output = new double[input.Length];
        var dMeanSum = dMean.Sum();        
        for (var i = 0; i < input.Length; i++) 
            output[i] = dxNormalized[i] / Math.Sqrt(Variance[i] + double.Epsilon) 
                        + dVariance[i] * 2 * (x[i] - Mean[i]) / input.Length + dMeanSum / input.Length;

        Gamma -= gammaGradient;
        
        return new Vector(input).AsTensor(Input.Channels[0].Rows, Input.Channels[0].Columns,
            Input.Channels.Count);
    }

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}