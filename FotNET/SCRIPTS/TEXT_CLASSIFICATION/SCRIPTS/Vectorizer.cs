namespace FotNET.SCRIPTS.TEXT_CLASSIFICATION.SCRIPTS;

public static class Vectorizer {
    public static List<List<double>> Vectorize(IEnumerable<int> tokens, List<List<double>> embeddings) =>
         tokens.Select(token => embeddings[token]).ToList();
}