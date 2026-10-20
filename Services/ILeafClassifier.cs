using LeafDoctor.Models;

namespace LeafDoctor.Services;

public interface ILeafClassifier
{
    PredictionResult Predict(byte[] imageBytes);
}