using System.Text.Json.Serialization;

namespace ClaudeReview.Entities.GitLab;

public class GitLabMergeRequest
{
    [JsonPropertyName("source_branch")]
    public required string SourceBranch { get; init; }

    [JsonPropertyName("target_branch")]
    public required string TargetBranch { get; init; }
}
