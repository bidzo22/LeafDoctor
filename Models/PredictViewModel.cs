namespace LeafDoctor.Models;

public class PredictViewModel
{
    public PredictionResult? Result { get; set; }
    public string? ImageDataUrl { get; set; }

    private static readonly Dictionary<string, string> DisplayNames = new()
    {
        ["healthy"] = "Здоровий лист",
        ["late_blight"] = "Фітофтороз",
        ["leaf_mold"] = "Листова плісень (кладоспоріоз)",
        ["yellow_leaf_curl_virus"] = "Вірус жовтої курчавості листя",
    };

    public static string Display(string label) =>
        DisplayNames.TryGetValue(label, out var name) ? name : label;
}