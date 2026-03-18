using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ClaudeReview.Services.OpenCode;

public class OpenCodeRunnerService
{
    private readonly ILogger<OpenCodeRunnerService> _logger;

    public OpenCodeRunnerService(ILogger<OpenCodeRunnerService> logger)
    {
        _logger = logger;
    }

    public async Task Run(string repoPath, string promptContent)
    {
        _logger.LogInformation("Starting opencode in {RepoPath}", repoPath);

        var startInfo = new ProcessStartInfo("opencode", ["run"])
        {
            WorkingDirectory = repoPath,
            RedirectStandardInput = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException("Failed to start opencode process");

        await process.StandardInput.WriteAsync(promptContent);
        process.StandardInput.Close();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"opencode exited with code {process.ExitCode}");
        }

        var outputPath = Path.Combine(repoPath, "__output", "review.html");
        if (File.Exists(outputPath))
        {
            _logger.LogInformation("Review output written to {OutputPath}", outputPath);
        }
        else
        {
            _logger.LogWarning(
                "opencode completed but review.html was not found at {OutputPath}",
                outputPath);
        }
    }
}