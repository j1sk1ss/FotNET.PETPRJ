using Microsoft.VisualBasic.FileIO;
using Array = FotNET.NETWORK.OBJECTS.DATA_OBJECTS.ARRAY.Array;

namespace FotNET.NETWORK.DATA.CSV;

public static class Parser {
    public static IEnumerable<Array> ParseArray(string path, DataConfig dataConfig) {
        var data = Parse(path, dataConfig.StartRow, dataConfig.Delimiters);
        return data.Select(t => ParseArray(t, dataConfig.InputColumnStart, dataConfig.InputColumnEnd,
            dataConfig.OutputColumnStart, dataConfig.OutputColumnEnd)).ToList();
    }

    private static Array ParseArray(IReadOnlyList<string> data, int inputColumnStart, int inputColumnEnd, 
        int outputColumnStart, int outputColumnEnd) {
        
        var inputData  = new double[inputColumnEnd - inputColumnStart];
        var outputData = new double[outputColumnEnd - outputColumnStart];

        for (var i = inputColumnStart; i < inputColumnEnd; i++) 
            inputData[i - inputColumnStart] = double.Parse(data[i]);
        
        for (var i = outputColumnStart; i < outputColumnEnd; i++) 
            outputData[i - outputColumnStart] = double.Parse(data[i]);
        
        return new Array(inputData, outputData);
    }

    private static IEnumerable<string[]> Parse(string path, int startRow, string[] delimiters) {
        using var parser = new TextFieldParser(path) {
            HasFieldsEnclosedInQuotes = true
        };
        parser.SetDelimiters(delimiters);

        for (var i = 0; i < startRow; i++) parser.ReadLine();

        var data = new List<string[]>();
        while (!parser.EndOfData) 
            data.Add(parser.ReadFields()!);
        
        return data;
    }
}