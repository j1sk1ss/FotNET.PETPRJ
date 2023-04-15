using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.DATA;

public class DataLayer : ILayer {

    public DataLayer(DataType dataType) {
        DataType = dataType;
    }
    
    private DataType DataType { get; }
    private Tensor Data { get; set; }

    public Tensor GetNextLayer(Tensor tensor) {
        if (DataType == DataType.InputTensor) Data = tensor;
        return tensor;
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        if (DataType == DataType.ErrorTensor) Data = error;
        return error;
    }

    public Tensor GetValues() => Data;

    public string GetData() => "";

    public string LoadData(string data) => data;
}

public enum DataType {
    InputTensor,
    ErrorTensor
}