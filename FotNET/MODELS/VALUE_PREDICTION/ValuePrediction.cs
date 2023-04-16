using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.HYPERBOLIC_TANGENT;
using FotNET.NETWORK.LAYERS.RECURRENT;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.MANY_TO_ONE;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.Xavier;

namespace FotNET.MODELS.VALUE_PREDICTION;

public static class ValuePrediction {
    public static Network SimpleValuePrediction = new Network(new List<ILayer> {
        new RecurrentLayer(new HyperbolicTangent(), new ManyToOne(), 10, new XavierInitialization()),
        new SoftMaxLayer()
    });
    
    public static Network DeepValuePrediction = new Network(new List<ILayer> {
        new RecurrentLayer(new HyperbolicTangent(), new ManyToOne(), 100, new XavierInitialization()),
        new SoftMaxLayer()
    });
}