using System.Text.Json.Serialization;

namespace ClaudeReview.Entities.GitLab;

public class GitLabProject
{
    [JsonPropertyName("ssh_url_to_repo")]
    public required string SshUrlToRepo { get; init; }
}
