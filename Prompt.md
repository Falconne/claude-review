Create a .Net 9 C# application that does the following actions:
- Takes in the URL for a Gitlab merge request as an argument.
- Based on this URL, use the Gitlab API to figure out the SSH clone URL for the repo it is in. The access token for the API will be in an env var called "GITLAB_API_TOKEN". The required ssh key will already be loaded in the current sshagent, so don't worry about that. Clone this repo into a working directory (it will have to be a full clone for what we need to do). Clone the repo into the "from" branch of the MR.
- The app's source should include a template for an opencode config file. It should have an entry for a single model, with  placeholders for the model name and API token. This template should be copied to the app's bin dir upon build so the app can find it at runtime by looking in its own directory.
- At runtime the app should replace the placehodlers in the config file and copy it to the correct location for an opencode config file on the machine for the current user (the app should fail if a file already exists there). The model name to use will be in a env var called MODEL_NAME and api token in MODEL_API_TOKEN.
- The app's source code should include a prompt.md file that will direct opencode to do a code review of the changes in the current branch that are being merged into the target branch and output the results as a styled html file into an output directory. The target branch name should be a placeholder in the file under source control.
- Like the opencode json template, this prompt file should be distributed along with the app.
- The app should update this prompt at runtime to replace the placeholder with the target branch name determined from the original Merge Request.
- The app should then run "opencode" (assume it's in the path) in a way that will run in Build mode with the contents of the updated prompt file as input for a one-off prompt, and once opencode is done it should exit, leaving us with an output HTML of the codereview that the agent inside opencode ran.

This app should be called "ClaudeReview". Place the code for it in a "/src/" dir, with a sln file at the root and create "ClaudeReview" directory at the root so I can create other sibling project dirs.

Once everything is done and is building, commit and push the changes. There is no need to test this.