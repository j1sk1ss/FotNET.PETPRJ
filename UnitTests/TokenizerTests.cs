using FotNET.SCRIPTS.TEXT_CLASSIFICATION.SCRIPTS;

namespace UnitTests;

public class TokenizerTests {
    [Test]
    public void TokenTest() {
        var text = "Hello, i`m from Netherlands!" +
                   "\nWhere are u living, dude?";
        var tokens = Tokenizer.Tokenize(text);
        foreach (var word in tokens) {
            Console.WriteLine(word);
        }
    }
}