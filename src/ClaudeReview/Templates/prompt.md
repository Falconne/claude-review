Performing a code review of the current branch. The full diff of the current branch compared to the target branch (`{{TARGET_BRANCH}}`) has already been generated and is available in `diff.txt` at the root of the repository. Read that file to see the changes — do not run git diff yourself.

Do not make nitpicking or trivial comments. Only concentrate on important items, e.g.
- Potential bugs or logic errors
- Possibly missed related fixes
- Performance issues
- Code quality and maintainability
- Missing error handling

Create a directory called `_output` in the current working directory if it doesn't already exist.

Output your complete review as a well-styled HTML file at `_output/review.html`. The HTML should:
- Have a professional, clean design with inline CSS styling
- Use colour coding for severity levels (critical, warning, suggestion)
- Include relevant code snippets where appropriate

Do not ask for confirmation. Proceed immediately with the review.
