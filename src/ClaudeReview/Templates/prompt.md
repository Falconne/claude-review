You are performing a code review of a GitLab merge request.

Review all the code changes in the current branch compared to the `{{TARGET_BRANCH}}` branch.

To see the changes, run:
```
git diff {{TARGET_BRANCH}}...HEAD
```

Analyse each changed file and provide a thorough code review covering:
- Potential bugs or logic errors
- Security concerns
- Performance issues
- Code quality and maintainability
- Best practices violations
- Missing error handling

Create a directory called `output` in the current working directory if it doesn't already exist.

Output your complete review as a well-styled HTML file at `output/review.html`. The HTML should:
- Have a professional, clean design with inline CSS styling
- Organise findings by file
- Use colour coding for severity levels (critical, warning, suggestion)
- Include relevant code snippets where appropriate
- Have a summary section at the top with an overall assessment

Do not ask for confirmation. Proceed immediately with the review.
