# Contributing to OpenPix

First off, thanks for taking the time to contribute! 🚀

The following is a set of guidelines for contributing to OpenPix. These are mostly guidelines, not rules. Use your best judgment and feel free to propose changes to this document in a pull request.

## How Can I Contribute?

### Reporting Bugs

This section guides you through submitting a bug report for OpenPix. Following these guidelines helps maintainers and the community understand your report, reproduce the behavior, and find related reports.

- **Use a clear and descriptive title** for the issue to identify the problem.
- **Describe the exact steps which reproduce the problem** in as many details as possible.
- **Provide specific examples** to demonstrate the steps.

### Pull Requests

1.  Fork the repo and create your branch from `main`.
2.  If you've added code that should be tested, add tests.
3.  If you've changed APIs, update the documentation.
4.  Ensure the test suite passes.
5.  Make sure your code follows the **Clean Code** and **DDD** principles established in the project.

## Styleguides

### Git Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less

### C# Coding Style

- We follow standard .NET coding conventions.
- Use `var` when the type is obvious.
- Prefer `Span<T>` over string manipulation for parsing logic.
- **Zero Allocation** is a goal for the Parser. Avoid `
ew` inside loops.

### Code of Conduct

Please note that we have a [Code of Conduct](CODE_OF_CONDUCT.md). Please follow it in all your interactions with the project.

### Missing a Feature?

You can request a new feature by submitting an issue to our [GitHub Repository](https://github.com/eduardocp/open-pix/issues). If you would like to implement a new feature, please submit an issue with a proposal for your work first, to be sure that we can use it.

Happy Coding!
