using Microsoft.Extensions.Logging;

namespace ClaudeReview.Services.OpenCode;

public class OpenCodeConfigService
{
    private readonly ILogger<OpenCodeConfigService> _logger;

    public OpenCodeConfigService(ILogger<OpenCodeConfigService> logger)
    {
        _logger = logger;
    }

    public void SetupConfig(string modelName, string apiToken)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "opencode.json");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"OpenCode config template not found at: {templatePath}");

        var providerName = ExtractProviderName(modelName);

        var configContent = File.ReadAllText(templatePath)
            .Replace("{{MODEL_NAME}}", modelName)
            .Replace("{{MODEL_API_TOKEN}}", apiToken)
            .Replace("{{PROVIDER_NAME}}", providerName);

        var configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "opencode");
        var configPath = Path.Combine(configDir, "opencode.json");

        if (File.Exists(configPath))
            throw new InvalidOperationException(
                $"OpenCode config file already exists at: {configPath}. " +
                "Please remove it before running this tool.");

        Directory.CreateDirectory(configDir);
        File.WriteAllText(configPath, configContent);

        _logger.LogInformation("OpenCode config written to {ConfigPath}", configPath);
    }

    private static string ExtractProviderName(string modelName)
    {
        var slashIndex = modelName.IndexOf('/');
        if (slashIndex <= 0)
            throw new ArgumentException(
                $"Model name '{modelName}' must be in 'provider/model' format (e.g. 'anthropic/claude-sonnet-4-5')");

        return modelName[..slashIndex];
    }
}
