namespace FotNET.SCRIPTS.TEXT_CLASSIFICATION.SCRIPTS;

public static class Tokenizer {
    public static List<string> Tokenize(string text) {
        var whiteSpaces = text.Split(new []{' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        var punctuation = new List<string> { ".", ",", "/", "'", "!", "?", "`", "-", ":"};

        var tokens = new List<string>();
        foreach (var element in whiteSpaces) {
            if (punctuation.Contains(element)) continue;
            
            var word = "";
            for (var i = 0; i < element.Length; i++) 
                if (!punctuation.Contains(element[i] + "")) word += element[i] + "";
                else if (punctuation.Contains(element[i] + "") && (i == element.Length - 1 || i == 0))
                    switch (i == 0) {
                        case true:
                            tokens.Add(element[i] + "");
                            break;
                        default:
                            tokens.Add(word);
                            word = "";
                            tokens.Add(element[i] + "");
                            break;
                    }
            
            if (word == "") continue;
            tokens.Add(word);
        }
        
        return tokens;
    }
}