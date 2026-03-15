namespace ClaudeReview.Entities;

public class MergeRequestInfo
{
    public required string SshCloneUrl { get; init; }
    public required string SourceBranch { get; init; }
    public required string TargetBranch { get; init; }
}
