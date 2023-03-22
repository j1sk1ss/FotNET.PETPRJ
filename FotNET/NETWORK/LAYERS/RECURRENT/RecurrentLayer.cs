using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT;

public class RecurrentLayer : ILayer {

    public RecurrentLayer(Function function, int size) {
        InputWeights  = new Matrix(1, size);
        HiddenWeights = new Matrix(size, size);
        OutputWeights = new Matrix(size, 1);

        HiddenBias = new double[size];
        HiddenBias[0] = .01d;
        
        OutputBias = new double[size];
        for (var i = 0; i < size; i++) {
            HiddenBias[i] = .01d;
        }
        
        Function = function;

        InputWeights.HeInitialization();
        HiddenWeights.HeInitialization();
        OutputWeights.HeInitialization();
    }
    
    private Matrix InputWeights { get; set; }

    private Matrix HiddenWeights { get; set; }
    
    private double[] HiddenBias { get; set; }
    
    private Matrix OutputWeights { get; set; }
    
    private double[] OutputBias { get; set; }

    private Function Function { get; set; }
    
    
    private List<double[]> HiddenNeurons { get; set; }
    private List<double> OutputNeurons { get; set; }
    

    public Tensor GetNextLayer(Tensor tensor) {
        var sequence = tensor.Flatten();
        var output = new List<double>();
        var hidden = new List<double[]>();

        for (var step = 0; step < sequence.Count; step++) {
            var currentElement = sequence[step];
            var input = (InputWeights * currentElement).GetAsList().ToArray();
            
            if (step > 0)
                hidden.Add(new Vector(new Vector(input) 
                                      + new Vector(hidden[step - 1] * HiddenWeights)) + new Vector(HiddenBias));
            else
                hidden.Add(input);

            hidden[^1] = Function.Activate(new Vector(hidden[^1])
                .AsTensor(1, hidden[^1].Length, 0)).Flatten().ToArray();
            output.Add((new Vector(hidden[^1] * OutputWeights) + new Vector(OutputBias))[0]);
        }

        HiddenNeurons = hidden;
        OutputNeurons = output;
        
        return new Vector(output.ToArray()).AsTensor(1, output.Count, 0);
    }
    
    public Tensor BackPropagate(Tensor error, double learningRate) {
        var sequence = error.Flatten();
        
        var hiddenWeightGradient = new double[HiddenNeurons.Count];
        
        var outputGradient = new Matrix(0, 0);
        var hiddenGradient = new Matrix(0, 0);
        var inputGradient  = new Matrix(0, 0);
        
        for (var step = sequence.Count - 1; step >= 0; step--) {
            var currentError = sequence[step];

            outputGradient = OutputWeights.Transpose() * currentError;
            if (step == 0)
                hiddenGradient = outputGradient;
            else
                hiddenGradient = outputGradient + new Vector(HiddenNeurons[step - 1] 
                                                             * HiddenWeights.Transpose()).AsMatrix(1, outputGradient.Columns, 0);

            HiddenNeurons[step - 1] = Function.Derivation(new Tensor(hiddenGradient)).Channels[0].GetAsList().ToArray();

            if (step > 0) {
                hiddenWeightGradient = new Vector(hiddenWeightGradient) + new Vector(HiddenNeurons[step - 1] * hiddenGradient);
            }
            
            inputGradient = hiddenGradient * currentError;

            InputWeights  -= inputGradient * learningRate;
            HiddenWeights -= hiddenGradient * learningRate;
            
            for (var bias = 0; bias < HiddenBias.Length; bias++)
                HiddenBias[bias] -= hiddenGradient.Average() * learningRate;

            OutputWeights -= outputGradient * learningRate;
            
            for (var bias = 0; bias < OutputBias.Length; bias++)
                OutputBias[bias] -= currentError * learningRate;
        }

        return null!;
    }

    public Tensor GetValues()
    {
        throw new NotImplementedException();
    }

    public string GetData()
    {
        throw new NotImplementedException();
    }

    public string LoadData(string data)
    {
        throw new NotImplementedException();
    }
}