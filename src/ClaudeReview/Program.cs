using ClaudeReview.Services.Git;
using ClaudeReview.Services.GitLab;
using ClaudeReview.Services.OpenCode;
using ClaudeReview.Services.Prompt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: ClaudeReview <gitlab-merge-request-url>");
    return 1;
}

var mrUrl = args[0];

var gitLabApiToken = Environment.GetEnvironmentVariable("GITLAB_API_TOKEN")
    ?? throw new InvalidOperationException("GITLAB_API_TOKEN environment variable is not set");

var modelName = Environment.GetEnvironmentVariable("MODEL_NAME")
    ?? throw new InvalidOperationException("MODEL_NAME environment variable is not set");

var modelApiToken = Environment.GetEnvironmentVariable("MODEL_API_TOKEN")
    ?? throw new InvalidOperationException("MODEL_API_TOKEN environment variable is not set");

var services = new ServiceCollection();
services.AddHttpClient();
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
services.AddSingleton<GitLabService>();
services.AddSingleton<GitService>();
services.AddSingleton<OpenCodeConfigService>();
services.AddSingleton<PromptService>();
services.AddSingleton<OpenCodeRunnerService>();

using var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    var gitLabService = serviceProvider.GetRequiredService<GitLabService>();
    var mrInfo = await gitLabService.GetMergeRequestInfo(mrUrl, gitLabApiToken);

    var gitService = serviceProvider.GetRequiredService<GitService>();
    var repoPath = await gitService.CloneRepository(mrInfo.SshCloneUrl, mrInfo.SourceBranch);

    var configService = serviceProvider.GetRequiredService<OpenCodeConfigService>();
    configService.SetupConfig(modelName, modelApiToken);

    var promptService = serviceProvider.GetRequiredService<PromptService>();
    var promptContent = promptService.PreparePrompt(mrInfo.TargetBranch);

    var runner = serviceProvider.GetRequiredService<OpenCodeRunnerService>();
    await runner.Run(repoPath, promptContent);

    return 0;
}
catch (Exception ex)
{
    logger.LogError(ex, "ClaudeReview failed");
    return 1;
}
