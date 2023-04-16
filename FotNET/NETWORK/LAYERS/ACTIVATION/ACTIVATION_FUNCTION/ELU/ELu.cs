namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.ELU;

public class ELu : Function {
    public ELu(double alpha) => Alpha = alpha;
    
    private double Alpha { get; }
    
    protected override double Activate(double value) {
        if (value >= 0) 
            return value; 
            
        return Alpha * (Math.Exp(value) - 1);
    }

    protected override double Derivation(double value, double referenceValue) {
        if (referenceValue >= 0) 
            return 1.0d;

        if (referenceValue == 0 && Alpha == 0) return 1.0d;
        
        return value * Alpha * Math.Exp(referenceValue);
    }
}