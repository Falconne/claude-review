using Microsoft.Extensions.Logging;

namespace ClaudeReview.Services.Prompt;

public class PromptService
{
    private readonly ILogger<PromptService> _logger;

    public PromptService(ILogger<PromptService> logger)
    {
        _logger = logger;
    }

    public string PreparePrompt(string targetBranch, string mrFileBaseUrl)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "prompt.md");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Prompt template not found at: {templatePath}");

        var content = File.ReadAllText(templatePath)
            .Replace("{{TARGET_BRANCH}}", targetBranch)
            .Replace("{{MR_FILE_BASE_URL}}", mrFileBaseUrl);

        _logger.LogInformation("Prompt prepared with target branch: {TargetBranch}", targetBranch);
        return content;
    }
}
