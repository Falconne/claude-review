# ClaudeReview

A .NET 9 console app that automates AI code review of GitLab merge requests using [opencode](https://opencode.ai).

## What It Does

Given a GitLab MR URL, the app:

1. **Fetches MR metadata** from the GitLab API — determines the repo's SSH clone URL, source branch, and target branch.
2. **Clones the repo** (full clone) into a temp directory, checked out to the source (MR) branch.
3. **Writes an opencode config** to `~/.config/opencode/opencode.json` using the model and API token from env vars. Fails if the file already exists.
4. **Prepares a review prompt** from a template, substituting in the target branch name.
5. **Runs `opencode run`** (non-interactive, piping the prompt via stdin), which performs the code review inside the cloned repo.
6. The opencode agent diffs the source branch against the target branch and writes a styled HTML report to `output/review.html` inside the cloned repo.

The app fails hard (non-zero exit, logged error) on any unexpected condition.

## Prerequisites

- `opencode` must be on `PATH`
- An SSH key loaded in the current SSH agent (used by git clone)
- The following environment variables set:

| Variable | Purpose |
|---|---|
| `GITLAB_API_TOKEN` | GitLab personal access token for API calls |
| `MODEL_NAME` | Model in `provider/model` format, e.g. `anthropic/claude-sonnet-4-5` |
| `MODEL_API_TOKEN` | API key for the model provider |

## Usage

```bash
ClaudeReview <gitlab-merge-request-url>
```

Example:
```bash
ClaudeReview https://gitlab.example.com/mygroup/myrepo/-/merge_requests/42
```

## Project Structure

```
ClaudeReview.sln
src/ClaudeReview/
  Program.cs                                  # Entry point: reads args/env, wires DI, orchestrates steps
  Templates/
    opencode.json                             # Opencode config template (copied to bin on build)
    prompt.md                                 # Review prompt template (copied to bin on build)
  Entities/
    MergeRequestInfo.cs                       # Internal model passed between services
    GitLab/
      GitLabProject.cs                        # GitLab API response: ssh_url_to_repo
      GitLabMergeRequest.cs                   # GitLab API response: source_branch, target_branch
  Services/
    GitLab/GitLabService.cs                   # Parses MR URL, calls GitLab REST API
    Git/GitService.cs                         # Runs git clone into a temp directory
    OpenCode/OpenCodeConfigService.cs         # Populates and deploys opencode config file
    OpenCode/OpenCodeRunnerService.cs         # Runs `opencode run`, pipes prompt via stdin
    Prompt/PromptService.cs                   # Loads prompt template, replaces {{TARGET_BRANCH}}
```

## Template Files

**`Templates/opencode.json`** — opencode config skeleton. Placeholders replaced at runtime:
- `{{MODEL_NAME}}` → value of `MODEL_NAME` env var (e.g. `anthropic/claude-sonnet-4-5`)
- `{{PROVIDER_NAME}}` → extracted from model name prefix (e.g. `anthropic`)
- `{{MODEL_API_TOKEN}}` → value of `MODEL_API_TOKEN` env var

Deployed to `~/.config/opencode/opencode.json` (Linux/macOS) or `%APPDATA%/opencode/opencode.json` (Windows).

**`Templates/prompt.md`** — instructs the opencode agent to review the MR diff and write `output/review.html`. Placeholder replaced at runtime:
- `{{TARGET_BRANCH}}` → target branch from the GitLab MR (e.g. `main`)
