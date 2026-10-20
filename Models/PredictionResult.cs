namespace LeafDoctor.Models;

public record PredictionResult(
    string Label,
    float Confidence,
    List<KeyValuePair<string, float>> AllScores);