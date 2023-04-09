using FotNET.DATA.DATA_OBJECTS;
using Microsoft.VisualBasic.FileIO;
using Array = FotNET.DATA.DATA_OBJECTS.ARRAY.Array;

namespace FotNET.DATA.CSV;

public static class Parser {
    public static List<IData> CsvToArrays(string path, Config config) {
        var data = Parse(path, config.StartRow, config.Delimiters);
        return new List<IData>(data.Select(datum => new Array(datum, config)).ToList());
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