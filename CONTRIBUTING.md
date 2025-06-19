# Contributing to OsmoDoc
Welcome to the osmodoc project! We appreciate your interest in contributing to the project and making it even better. As a contributor, 
please follow the guidelines outlined below:

## Table of contents
- [Got a question or problem?](#got-a-question-or-problem)
- [Issues and bugs](#found-any-issues-and-bugs)
- [Submitting an issue](#submission-guidelines)
- [Feature requests](#feature-requests)
- [Coding rules](#coding-rules)
- [Commit message guidelines](#commit-message-guidelines)

## Got a question or problem?

**If you have questions or encounter problems, please refrain from opening issues for general support questions**. GitHub issues are primarily for bug 
reports and feature requests. For general questions and support, consider using [Stack Overflow](https://stackoverflow.com/questions/tagged/osmodoc) 
and tag your questions with the `osmodoc` tag. Here's why Stack Overflow is a preferred platform:

	- Questions and answers are publicly available, helping others.
	- The voting system on Stack Overflow highlights the best answers.

To save time for both you and us, we will close issues related to general support questions and direct users to Stack Overflow.

## Found any issues and bugs

If you find a bug in the source code, you can help us by [submitting an issue](https://github.com/OsmosysSoftware/osmodoc/issues/new) 
to our GitHub Repository. Even better, you can submit a [pull request](https://github.com/OsmosysSoftware/osmodoc/pulls) with a fix.

## Submission guidelines

### Submitting an issue

Before you submit an issue, please check the issue tracker to see if a similar issue already exists. The discussions there may provide workarounds.

For us to address and fix a bug, we need to reproduce it. Thus when submitting a bug report, we will ask for a minimal reproduction scenario using a repository or [Gist](https://gist.github.com/). Providing a live, reproducible scenario helps us understand the issue better. Information to include:

- The version of the osmodoc you are using.
- Any third-party libraries and their versions.
- A use-case that demonstrates the issue.

Without a minimal reproduction, we may need to close the issue due to insufficient information.

You can file new issues using our [new issue form](https://github.com/OsmosysSoftware/osmodoc/issues/new).

### Submitting a pull request (PR)

Before submitting a Pull Request (PR), please follow these guidelines:

1. Search GitHub [pull requests](https://github.com/OsmosysSoftware/osmodoc/pulls) to ensure there is no open or closed PR 
   related to your submission.
2. Fork this repository.
3. Make your changes in a new Git branch.
   ```shell
   git checkout -b my-fix-branch main
   ```
4. Follow our coding rules, which is mentioned below.
5. Commit your changes using a descriptive commit message that adheres to our commit message conventions.
   ```shell
   git commit -a
   ```
   Note: the optional commit -a command line option will automatically "add" and "rm" edited files.
6. Push your branch to your GitHub repository.
   ```shell
   git push origin my-fix-branch
   ```
7. Send a pull request to the `osmodoc:main`.

- <strong style="color:black">�</strong> **If we suggest changes, then:**
  - � Make the required updates.
  - � Ensure that your changes do not break existing functionality or introduce new issues.
  - � Rebase your branch and force push to your GitHub repository. This will update your Pull Request.

  That's it! Thank you for your contribution!

## Feature requests

You can request new features by submitting an issue to our GitHub repository. If you intend to implement a new feature, start by proposing it through 
an issue. Major features require discussion, so please prefix your proposal with [discussion], for example, "[discussion]: your feature idea.
" Smaller features can be crafted and directly submitted as a Pull Request.

## Coding rules

To ensure consistency throughout the source code, follow these rules as you work on the project:

- All features or bug fixes must be tested by one or more specs (unit-tests).

- We follow the [development coding standards](https://github.com/OsmosysSoftware/dev-standards/blob/main/coding-standards/dotnet.md),
  but wrap all code at 100 characters.
 
## Commit message guidelines

In this project, we have specific rules for formatting our Git commit messages. These guidelines result in more readable messages that are easy 
to follow when reviewing the project's history. Additionally, we use these commit messages to **generate the osmodoc change log**.

### Commit message format

Each commit message consists of a **header**, a **body**, and a **footer**. The header follows a specific format that includes 
a **type**, a **scope**, and a **subject**:
```
<type>: <subject>
<BLANK LINE>
<body>
<BLANK LINE>
<footer>
```
The **header** is mandatory.

Any line of the commit message cannot be longer than 100 characters! This allows the message to be easier to read 
on GitHub as well as in various git tools.

Footer should contain a [closing reference to an issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue) if any.

Samples: (even more [samples](https://github.com/OsmosysSoftware/osmodoc/commits/main))
`docs: update change log to beta.5`
`fix: need to depend on latest rxjs and zone.js`

### Revert

If the commit reverts a previous commit, it should begin with `revert:`, followed by the header of the reverted commit. 
In the body it should say: `This reverts commit <hash>`., where the hash is the SHA of the commit being reverted.

### Type
Must be one of the following:

- build: Changes that affect the build system or external dependencies (example scopes: gulp, broccoli, npm)
- chore: Updating tasks, etc.; no production code change
- ci: Changes to our CI configuration files and scripts (example scopes: Travis, Circle, BrowserStack, SauceLabs)
- docs: Documentation-only changes
- feat: A new feature
- fix: A bug fix
- perf: A code change that improves performance
- refactor: A code change that neither fixes a bug nor adds a feature
- style: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
- test: Adding missing tests or correcting existing tests
- sample: A change to the samples

### Subject
The subject contains succinct description of the change:
- use the imperative, present tense: "change" not "changed" nor "changes"
- don't capitalize first letter
- no dot (.) at the end

### Body

Just as in the **subject**, use the imperative, present tense: "change" not "changed" nor "changes". The body should include 
the motivation for the change and contrast this with previous behavior.

### Footer

The footer should contain any information about **Breaking Changes** and is also the place to reference GitHub 
issues that this commit **Closes**.

**Breaking Changes** should start with the word `BREAKING CHANGE:` with a space or two newlines. The rest of the commit message 
is then used for this.
