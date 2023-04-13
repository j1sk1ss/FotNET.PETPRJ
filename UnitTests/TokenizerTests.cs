using FotNET.SCRIPTS.TEXT_CLASSIFICATION.SCRIPTS;

namespace UnitTests;

public class TokenizerTests {
    [Test]
    public void TokenTest() {
        var text = "hello .How are u?";
        var tokens = Tokenizer.Tokenize(text, new List<string>());
        foreach (var word in tokens) {
            Console.WriteLine(word);
        }
    }
}