using FotNET.NETWORK;
using FotNET.NETWORK.LAYERS;
using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION.TANGENSOID;
using FotNET.NETWORK.LAYERS.RECURRENT;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ManyToOne;
using FotNET.NETWORK.LAYERS.SOFT_MAX;
using FotNET.NETWORK.MATH.Initialization.Xavier;

namespace FotNET.MODELS.TEXT_CLASSIFICATION;

public static class TextClassification {
    public static Network SimpleTextClassification = new Network(new List<ILayer> {
        new RecurrentLayer(new Tangensoid(), new ManyToOne(), 10, new XavierInitialization()),
        new SoftMaxLayer()
    });
    
    public static Network DeepTextClassification = new Network(new List<ILayer> {
        new RecurrentLayer(new Tangensoid(), new ManyToOne(), 100, new XavierInitialization()),
        new SoftMaxLayer()
    });
}