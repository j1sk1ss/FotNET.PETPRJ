using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SIGMOID;

namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.SWISH;

public class Swish : Function {
    /// <summary>
    /// Swish learnable activation function
    /// </summary>
    /// <param name="startValue"> Start weight parameter </param>
    /// <param name="learningRate"> Learning Rate </param>
    public Swish(double startValue, double learningRate) {
        LearningRate = learningRate;
        Beta         = startValue;
    }

    private double Beta { get; set; }
    private double LearningRate { get; }
    
    protected override double Activate(double value) => value / (1 + Math.Exp(-(value * Beta)));
    
    protected override double Derivation(double value, double activatedValue) {
        var deactivatedValue =
            activatedValue + new Sigmoid().Activate(new[] { activatedValue })[0] * (Beta - activatedValue);
        Beta -= value * LearningRate;
        return value * deactivatedValue;
    }
}