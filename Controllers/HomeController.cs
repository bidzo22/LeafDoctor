using System.Diagnostics;
using LeafDoctor.Models;
using LeafDoctor.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeafDoctor.Controllers;

public class HomeController : Controller
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; 

    private readonly ILeafClassifier _classifier;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILeafClassifier classifier, ILogger<HomeController> logger)
    {
        _classifier = classifier;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View(new PredictViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormFile? photo)
    {
        var vm = new PredictViewModel();

        if (photo is null || photo.Length == 0)
            ModelState.AddModelError("photo", "Будь ласка, оберіть фото.");
        else if (photo.Length > MaxFileSize)
            ModelState.AddModelError("photo", "Файл завеликий (максимум 5 МБ).");
        else if (!AllowedExtensions.Contains(Path.GetExtension(photo.FileName).ToLowerInvariant()))
            ModelState.AddModelError("photo", "Підтримуються лише файли JPG та PNG.");

        if (!ModelState.IsValid)
            return View(vm);

        using var ms = new MemoryStream();
        await photo!.CopyToAsync(ms);
        var bytes = ms.ToArray();

        try
        {
            vm.Result = _classifier.Predict(bytes);
            var contentType = string.IsNullOrEmpty(photo.ContentType) ? "image/jpeg" : photo.ContentType;
            vm.ImageDataUrl = $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка під час розпізнавання");
            ModelState.AddModelError("photo", "Не вдалося обробити зображення. Спробуйте інше фото.");
        }

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}