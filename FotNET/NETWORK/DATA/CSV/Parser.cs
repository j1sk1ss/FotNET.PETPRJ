using Microsoft.VisualBasic.FileIO;

namespace FotNET.NETWORK.DATA.CSV;

public static class Parser {

    public static void Parse(string path) {
        using (var parser = new TextFieldParser(path)) {
            parser.CommentTokens = new[] { "#" };
            parser.SetDelimiters(new[] { "," });
            parser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            parser.ReadLine();

            while (!parser.EndOfData) {
                // Read current line fields, pointer moves to the next line.
                var fields = parser.ReadFields();
                var Name = fields[0];
                var Address = fields[1];
            }
        }
    }
    
}