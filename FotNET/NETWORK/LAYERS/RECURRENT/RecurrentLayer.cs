using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT;

public class RecurrentLayer : ILayer {

    public RecurrentLayer(Function function, int size) {
        InputWeights  = new Matrix(1, size);
        
        HiddenWeights = new Matrix(size, size);
        HiddenBias = new double[size];
        
        OutputWeights = new Matrix(size, 1);
        OutputBias = new double[1];
        
        Function = function;

        HiddenNeurons = new double[size];

        for (var i = 0; i < size; i++) {
            HiddenNeurons[i] = .01;
            HiddenBias[i] = .01;
        }
        
        OutputNeurons = new double[size];
        
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
    
    public double[] HiddenNeurons { get; }
    public double[] OutputNeurons { get; set; }

    public Tensor GetNextLayer(Tensor tensor) =>
        new Vector(new RecurrentLayer(Function, 1).ForwardFeed(tensor, HiddenNeurons).OutputNeurons).AsTensor(1, OutputNeurons.Length, 0);

    private RecurrentLayer ForwardFeed(Tensor tensor, double[] hiddenNeurons) {
        var sequence = tensor.Flatten();
        var tensorSequence = new List<double>();
        var currentElement = 0d;
        
        for (var i = 0; i < sequence.Count; i++) {
            if (i == 0) {
                currentElement = sequence[i];
                continue;
            }

            tensorSequence.Add(sequence[i]);
        }

        var nextStepTensor = new Vector(tensorSequence.ToArray()).AsTensor(1, tensorSequence.Count, 0);
        
        var neurons = (InputWeights * currentElement).GetAsList().ToArray();
        var currentHiddenNeurons = Function.Activate(new Vector(new Vector(hiddenNeurons * HiddenWeights) + new Vector(neurons))
            .AsTensor(1, neurons.Length, 0)).Flatten().ToArray();
        
        OutputNeurons = currentHiddenNeurons * OutputWeights;
        
        return new RecurrentLayer(Function, 1).ForwardFeed(nextStepTensor, HiddenNeurons);
    }
    
    public Tensor BackPropagate(Tensor error, double learningRate)
    {
        throw new NotImplementedException();
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