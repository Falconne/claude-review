using System.Net.Http.Json;
using ClaudeReview.Entities;
using ClaudeReview.Entities.GitLab;
using Microsoft.Extensions.Logging;

namespace ClaudeReview.Services.GitLab;

public class GitLabService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GitLabService> _logger;

    public GitLabService(IHttpClientFactory httpClientFactory, ILogger<GitLabService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<MergeRequestInfo> GetMergeRequestInfo(string mrUrl, string apiToken)
    {
        var (baseUrl, projectPath, mrIid) = ParseMergeRequestUrl(mrUrl);
        var encodedProjectPath = Uri.EscapeDataString(projectPath);

        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", apiToken);

        _logger.LogInformation("Fetching project info for {ProjectPath}", projectPath);
        var project = await client.GetFromJsonAsync<GitLabProject>(
            $"{baseUrl}/api/v4/projects/{encodedProjectPath}")
            ?? throw new InvalidOperationException($"Failed to retrieve project info for '{projectPath}'");

        _logger.LogInformation("Fetching merge request !{MrIid}", mrIid);
        var mergeRequest = await client.GetFromJsonAsync<GitLabMergeRequest>(
            $"{baseUrl}/api/v4/projects/{encodedProjectPath}/merge_requests/{mrIid}")
            ?? throw new InvalidOperationException($"Failed to retrieve merge request !{mrIid}");

        _logger.LogInformation(
            "MR !{MrIid}: {SourceBranch} -> {TargetBranch}, SSH URL: {SshUrl}",
            mrIid, mergeRequest.SourceBranch, mergeRequest.TargetBranch, project.SshUrlToRepo);

        var fileBaseUrl = $"{baseUrl}/{projectPath}/-/blob/{mergeRequest.SourceBranch}/";

        return new MergeRequestInfo
        {
            SshCloneUrl = project.SshUrlToRepo,
            SourceBranch = mergeRequest.SourceBranch,
            TargetBranch = mergeRequest.TargetBranch,
            FileBaseUrl = fileBaseUrl
        };
    }

    private static (string BaseUrl, string ProjectPath, int MergeRequestIid) ParseMergeRequestUrl(string mrUrl)
    {
        var uri = new Uri(mrUrl);
        var path = uri.AbsolutePath;

        const string mrSegment = "/-/merge_requests/";
        var mrIndex = path.IndexOf(mrSegment, StringComparison.OrdinalIgnoreCase);
        if (mrIndex < 0)
            throw new ArgumentException($"Invalid GitLab merge request URL: {mrUrl}");

        var projectPath = path[1..mrIndex];
        var mrIidString = path[(mrIndex + mrSegment.Length)..].TrimEnd('/');

        if (!int.TryParse(mrIidString, out var mrIid))
            throw new ArgumentException($"Invalid merge request IID in URL: {mrIidString}");

        var baseUrl = $"{uri.Scheme}://{uri.Host}";
        if (!uri.IsDefaultPort)
            baseUrl += $":{uri.Port}";

        return (baseUrl, projectPath, mrIid);
    }
}
