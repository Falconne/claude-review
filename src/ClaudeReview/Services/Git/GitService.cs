using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ClaudeReview.Services.Git;

public class GitService
{
    private readonly ILogger<GitService> _logger;

    public GitService(ILogger<GitService> logger)
    {
        _logger = logger;
    }

    public async Task<string> CloneRepository(string sshUrl, string branch)
    {
        var workDir = Path.Combine(Path.GetTempPath(), $"claude-review-{Guid.NewGuid():N}");
        Directory.CreateDirectory(workDir);

        _logger.LogInformation("Cloning {SshUrl} (branch: {Branch}) into {WorkDir}", sshUrl, branch, workDir);

        var startInfo = new ProcessStartInfo("git", ["clone", "--branch", branch, sshUrl, workDir])
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start git clone process");

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        var stderr = await stderrTask;
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"git clone failed with exit code {process.ExitCode}: {stderr}");

        _logger.LogInformation("Repository cloned successfully to {WorkDir}", workDir);
        return workDir;
    }
}
