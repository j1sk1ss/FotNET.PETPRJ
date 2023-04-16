using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;

namespace FotNET.NETWORK.MATH.LOSS_FUNCTION.ERROR.DERIVATE;

public class Derivate : ErrorFunction {
    public Derivate(Function function) => Function = function;
    
    private Function Function { get; }
    
    protected override double Derivation(double prediction, double expected) =>
         Math.Abs(prediction - expected) < .01d ? Function.Derivation(new[] { 1d - prediction }, 
             new[] { prediction })[0] : Function.Derivation(new[] { - prediction }, 
             new[] { prediction })[0];
}