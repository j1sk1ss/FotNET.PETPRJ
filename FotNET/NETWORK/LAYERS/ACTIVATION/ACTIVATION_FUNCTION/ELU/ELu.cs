namespace FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.ELU;

public class ELu : Function {

    public ELu(double alpha) => Alpha = alpha;
    
    private double Alpha { get; }
    
    protected override double Activate(double value) {
        if (value >= 0) 
            return value; 
            
        return Alpha * (Math.Exp(value) - 1);
    }

    protected override double Derivation(double value) {
        if (value >= 0) 
            return 1.0; 
        
        return value * Alpha * Math.Exp(value);
    }
}