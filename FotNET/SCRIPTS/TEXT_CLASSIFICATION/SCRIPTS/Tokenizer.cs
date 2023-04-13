namespace FotNET.SCRIPTS.TEXT_CLASSIFICATION.SCRIPTS;

public static class Tokenizer {
    
    public static List<string> Tokenize(string text, List<string> stopWords) {
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
                            if (!stopWords.Contains(word)) tokens.Add(word);
                            word = "";
                            tokens.Add(element[i] + "");
                            break;
                    }
            
            if (word == "") continue;
            if (!stopWords.Contains(word)) tokens.Add(word);
        }
        
        return tokens;
    }

    public static List<int> Tokenize(IEnumerable<string> tokens, List<string> dictionary) =>
         tokens.Select(token => dictionary.Contains(token) ? dictionary.IndexOf(token) : 0).ToList();
}