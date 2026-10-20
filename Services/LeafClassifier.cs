using LeafDoctor.MLModels;
using LeafDoctor.Models;

namespace LeafDoctor.Services;

public class LeafClassifier : ILeafClassifier
{
    
    private static readonly object Sync = new();

    public PredictionResult Predict(byte[] imageBytes)
    {
        var input = new LeafModel.ModelInput { ImageSource = imageBytes };

        List<KeyValuePair<string, float>> scores;
        lock (Sync)
        {
            scores = LeafModel.PredictAllLabels(input).ToList(); 
        }

        var top = scores[0];
        return new PredictionResult(top.Key, top.Value, scores);
    }
}