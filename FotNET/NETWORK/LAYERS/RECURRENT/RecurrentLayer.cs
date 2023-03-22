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
    
    private Matrix InputWeights { get; }
    
    private Matrix HiddenWeights { get; }
    
    private double[] HiddenBias { get; }
    
    private Matrix OutputWeights { get; }
    
    private double[] OutputBias { get; }

    private Function Function { get; }
    
    
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
        
        for (var i = sequence.Count - 1; i >= 0; i--) {
            var currentError = sequence[i];
            var outputGradient = HiddenNeurons[i]
        }
        /*
         * l1_grad = loss_grad[1].reshape(1,1)

o_weight_grad += hiddens[1][:,np.newaxis] @ l1_grad
o_bias_grad += np.mean(l1_grad)
         */
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