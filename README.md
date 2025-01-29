# Code Repository for Crafting an Error Handling Strategy (.NET)

This repository provides code used for exercises and demonstrations
included in the .NET version of the [Crafting an Error Handling Course](https://learn.temporal.io/courses/errstrat/) training course.

It's important to remember that the example code used in this course was designed to support learning a specific aspect of Temporal, not to serve as a ready-to-use template for implementing a production system.

For the exercises, make sure to run `temporal server start-dev --ui-port 8080 --db-filename clusterdata.db` in one terminal to start the Temporal server. For more details on this command, please refer to the `Setting up a Local Development Environment` chapter in the course. Note: If you're using the Codespaces environment to run this exercise, you can skip this step.

## Hands-On Exercises

| Directory Name                        | Exercise                                                    |
| :------------------------------------ | :---------------------------------------------------------- |
| `exercises/handling-errors`           | [Exercise 1](exercises/handling-errors/README.md)           |
| `exercises/non-retryable-error-types` | [Exercise 2](exercises/non-retryable-error-types/README.md) |
| `exercises/rollback-with-saga`        | [Exercise 3](exercises/rollback-with-saga/README.md)        |

## Reference

The following links provide additional information that you may find helpful as you work through this course.

- [General Temporal Documentation](https://docs.temporal.io/)
- [Temporal .NET SDK API Reference](https://dotnet.temporal.io/)
- [.NET SDK Reference](https://github.com/temporalio/sdk-dotnet/)
- [C# Language Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)

## Exercise Environment for this Course

You can launch an exercise environment for this course using GitHub Codespaces by 
following [this](codespaces.md) walkthrough.

Alternatively, you can perform these exercises directly on your computer. Refer to the instructions about setting up a local development environment, which you'll find in the "About this Course" chapter.