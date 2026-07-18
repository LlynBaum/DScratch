---
name: dotnet-workflow
description: >-
  Use this skill when you need to build the DScratch project, run unit tests, or run Playwright E2E tests.
---

# .NET Development Workflow for DScratch

This skill guides you through building and testing the DScratch editor project.

## Workflows

### 1. Build the Solution
Run the standard build command to compile all projects in the solution (Host, Client, Core library, Tests, and Tools):
`dotnet build`

### 2. Run All Tests
To run both the unit tests and the Playwright E2E tests:
`dotnet test`

> [!NOTE]
> The E2E test project (`DScratch.E2E`) automatically spins up the `DScratch.Host` server on port 5001 during setup and terminates it on teardown.

### 3. Run Unit Tests Only
For quick validation of code logic without launching browser tests:
`dotnet test tests/DScratch.Tests`

### 4. Run E2E Tests Only
To execute only the Playwright E2E tests:
`dotnet test tests/DScratch.E2E`
