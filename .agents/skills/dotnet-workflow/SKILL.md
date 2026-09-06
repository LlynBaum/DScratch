---
name: dotnet-workflow
description: >-
  Use this skill when you need to build the DScratch project, run .NET unit tests, run Vitest in-browser TypeScript tests, or run Playwright E2E tests.
---

# Development & Testing Workflow for DScratch

This skill guides you through building and testing the DScratch editor project across both .NET and TypeScript layers.

## Workflows

### 1. Build the Solution
Run the standard build command to compile all projects in the solution (Host, Client, Core library, Tests, and Tools). This automatically bundles the TypeScript client scripts via `esbuild`:
`dotnet build`

### 2. Run TypeScript Unit Tests (Vitest Browser Mode)
The client-side DOM logic (paging, transactions, selections, node helpers) is tested natively in headless Chromium using Vitest Browser Mode (`@vitest/browser-playwright`).

From the scripts directory (`src/DScratch.Client/BrowserInteractions/Scripts`):
* **Run all TypeScript tests (headless)**:
  `npm test`
* **Run TypeScript tests in interactive watch mode**:
  `npx vitest` (or `npm run test:watch`)
* **Run Vitest with Visual UI & DevTools Inspector**:
  `npm run test:ui` (opens the browser runner UI where you can inspect rendered DOM, stylesheets, and debug step-by-step)
* **Run a single test file**:
  `npx vitest run tests/paging/overflow-new-page.test.ts`

> [!NOTE]
> Vitest browser tests execute inside a real Chromium browser (`/usr/bin/chromium-browser`) with real CSS tokens and layout loaded. Use `page.getByTestId(...)` (mapped to `data-dnode-id`) or custom locators (`page.getByCSS(...)`, `page.getByPageNumber(...)`, `page.DPage()`).

### 3. Run .NET Unit Tests Only
For quick validation of C# core logic (CRDT, event handlers, commands) without launching browser tests:
`dotnet test tests/DScratch.Tests`

### 4. Run Playwright E2E Tests Only
To execute full-stack Playwright E2E integration tests:
`dotnet test tests/DScratch.E2E`

> [!NOTE]
> The E2E test project (`DScratch.E2E`) automatically spins up the `DScratch.Host` server on port 5001 during setup and terminates it on teardown.

### 5. Run All .NET Tests
To run both the .NET unit tests and the Playwright E2E tests:
`dotnet test`
