using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.GROUP_NORMALIZATION;

public class GroupNormalization : ILayer {
    public GroupNormalization(Tensor inputTensorShape, int numGroups, double epsilon) {
        NumGroups   = numGroups;
        NumChannels = inputTensorShape.Channels.Count;
        Epsilon     = epsilon;
        
        Gamma       = inputTensorShape;
        Beta        = inputTensorShape;
    }
    
    private int NumGroups { get; }
    private int NumChannels { get; }
    private double Epsilon { get; }
    private Tensor Gamma { get; set; }
    private Tensor Beta { get; set; }

    public Tensor GetNextLayer(Tensor tensor) {
        var groupSize = NumChannels / NumGroups;
        var groups = new Tensor[NumGroups];
        for (var i = 0; i < NumGroups; i++) {
            var tempTensor = new Tensor(new List<Matrix>());
            for (var j = i * groupSize; j < i * groupSize + groupSize; j++) 
                tempTensor.Channels.Add(tensor.Channels[j]);
            
            groups[i] = tempTensor;
        }

        var normalizedGroups = new Tensor[NumGroups];
        for (var i = 0; i < NumGroups; i++) 
            normalizedGroups[i] = (groups[i] - TensorMean(groups[i])) / Math.Sqrt(TensorVar(groups[i]) + Epsilon);
        
        var normalized = new Tensor(new List<Matrix>());
        foreach (var currentTensor in normalizedGroups)
            normalized.Channels.AddRange(currentTensor.Channels);

        normalized *= Gamma;
        normalized += Beta;

        return normalized;
    }

    private static double TensorMean(Tensor tensor) =>
        tensor.Channels.Sum(channel => channel.Average()) / tensor.Channels.Count;
    
    private static double TensorVar(Tensor x, bool unbiased = true) {
        var diff = x - TensorMean(x);
        var n = unbiased ? x.Channels[0].Rows - 1 : x.Channels[0].Rows;
        
        return TensorMean(diff * diff) * n / (n - 1);
    }
    
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        var gammaGrad = new Tensor(new List<Matrix>());
        var betaGrad = new Tensor(new List<Matrix>());
        var groupSize = NumChannels / NumGroups;

        for (var i = 0; i < NumGroups; i++) {
            var tempTensor = new Tensor(new List<Matrix>());
            for (var j = i * groupSize; j < i * groupSize + groupSize; j++) 
                tempTensor.Channels.Add(error.Channels[j]);
                
            var mean = TensorMean(tempTensor);
            var var = TensorVar(tempTensor, false);
            var stdInv = 1.0 / Math.Sqrt(var + Epsilon);

            for (var j = i * groupSize; j < i * groupSize + groupSize; j++) {
                var x = tempTensor.Channels[j - i * groupSize];
                var xHat = (x - mean) * stdInv;
                gammaGrad.Channels.Add(xHat);
                betaGrad.Channels.Add(new Matrix(x.Rows, x.Columns));
            }
        }

        var totalGammaGrad = new Tensor(1, error.Flatten().Count, 1);
        var totalBetaGrad = new Tensor(1, error.Flatten().Count, 1);
        
        for (var i = 0; i < NumGroups * groupSize; i++) {
            var gamma = Gamma.Channels[i % groupSize];
            totalGammaGrad.Channels[0].Body[0, i] = (gamma * gammaGrad.Channels[i % groupSize]).Sum();
            totalBetaGrad.Channels[0].Body[0, i] = betaGrad.Channels[i % groupSize].Sum();
        }

        Gamma -= totalGammaGrad * learningRate;
        Beta -= totalBetaGrad * learningRate;

        if (backPropagate) {
            var nextError = new Tensor(new List<Matrix>());
            for (var i = 0; i < NumGroups; i++) {
                var tempTensor = new Tensor(new List<Matrix>());
                for (var j = i * groupSize; j < i * groupSize + groupSize; j++) 
                    tempTensor.Channels.Add(error.Channels[j]);
                
                for (var j = i * groupSize; j < i * groupSize + groupSize; j++) {
                    var matrix = tempTensor.Channels[j - i * groupSize];
                    var gammaGradChannel = gammaGrad.Channels[j - i * groupSize];
                    var gamma = Gamma.Channels[j % groupSize];        
                    
                    var errorChannel = new Matrix(matrix.Rows, matrix.Columns);
                    for (var r = 0; r < matrix.Rows; r++)
                        for (var c = 0; c < matrix.Columns; c++)
                            errorChannel.Body[r, c] += gammaGradChannel.Body[r, c] * gamma.Body[r, c];
                            
                    nextError.Channels.Add(errorChannel);
                }
            }

            return nextError;
        }

        return error;
    }

    public Tensor GetValues() => null!;

    public string GetData() => "";

    public string LoadData(string data) => data;
}