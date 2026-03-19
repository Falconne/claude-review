namespace ClaudeReview.Entities;

public class MergeRequestInfo
{
    public required string SshCloneUrl { get; init; }
    public required string SourceBranch { get; init; }
    public required string TargetBranch { get; init; }
    /// <summary>
    /// Base URL for linking to files in the MR source branch, e.g.
    /// "https://gitlab.example.com/mygroup/myrepo/-/blob/feature-branch/"
    /// Append "{file_path}#L{line}" to form a full file link.
    /// </summary>
    public required string FileBaseUrl { get; init; }
}
