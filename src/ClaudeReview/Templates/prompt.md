Performing a code review of the current branch. The full diff of the current branch compared to the target branch (`{{TARGET_BRANCH}}`) has already been generated and is available in `diff.txt` at the root of the repository. Read that file to see the changes — do not run git diff yourself.

Do not make nitpicking or trivial comments. Only concentrate on important items, e.g.
- Potential bugs or logic errors
- Possibly missed related fixes
- Performance issues
- Code quality and maintainability
- Missing error handling

When mentioning files or specific lines of code in your review, make them clickable links to the GitLab merge request. The base URL for file links is:

`{{MR_FILE_BASE_URL}}`

To construct a link, append the file path (relative to the repo root, no leading slash) and optionally a line anchor. Examples:
- File only: `{{MR_FILE_BASE_URL}}src/Services/MyService.cs`
- File at line 42: `{{MR_FILE_BASE_URL}}src/Services/MyService.cs#L42`

All file/line links in the HTML must use `target="_blank"` so they open in a new tab.

Output your complete review as a well-styled HTML file in `__output/review.html`. The HTML must:
- Read `review.css` from the root of the repository and embed its full contents verbatim inside a `<style>` block in the `<head>` — do NOT link to it externally; the HTML file must be self-contained
- Use the HTML structure and CSS class names defined in `review.css` (see the comments at the top of that file for the expected markup)
- Use colour coding for severity levels via the badge and finding classes: `finding-critical`, `finding-warning`, `finding-suggestion`
- Include relevant code snippets where appropriate inside `<pre><code>` blocks

Do not ask for confirmation. Proceed immediately with the review.
