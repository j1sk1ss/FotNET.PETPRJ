namespace FotNET.SCRIPTS.TEXT_CLASSIFICATION.SCRIPTS.MATH;

public static class Similarity {
    public static double GetSimilarity(List<double> firstWord, List<double> secondWord) =>
        firstWord.Select((t, i) => Math.Abs(t - secondWord[i])).Sum() / firstWord.Count;
}