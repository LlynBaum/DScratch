# DScratch C# & Blazor Coding Guidelines

These rules apply to all code modifications and additions within this workspace.

## 1. Project Architecture & Stack
Refer to the [Architecture.md](../../docs/Architecture.md) file for details. Code should be split cleanly according to the following roles:
*   **Core Library (`src/DScratch`)**: The "brain", domain model, and absolute source of truth. It manages the document tree using CRDTs, handles text modifications, performs document calculations, and generates transactions/diffs. It has no dependencies on Blazor or browser-specific APIs.
*   **Client UI (`src/DScratch.Client`)**: Acts as a rendering pipe and input forwarder. It handles Blazor components, UI rendering, and events that require browser-specific interactions.
*   **TypeScript Layer & Render Engine (`src/DScratch.Client/BrowserInteractions/Scripts`)**: Directly accesses Browser APIs (e.g. intercepting input events via `beforeinput` with `preventDefault`), executes StepDiff mutations, handles selections/cursors, and **manages physical document layout & multi-page splitting (`paging.ts`)**. Note: Always edit TS source files in this directory; they are compiled/bundled to the `dist` directory. Do not edit compiled bundles directly.
*   **ASP.NET Host (`src/DScratch.Host`)**: Handles routing, serving WASM/static assets, and Server-Side Prerendering (SSR).
*   **Tests**: Contains C# unit tests (`tests/DScratch.Tests`), Vitest in-browser TypeScript tests (`src/DScratch.Client/.../Scripts`), and Playwright E2E tests (`tests/DScratch.E2E`).


## 2. Coding Standards
*   Use modern C# features (C# 12+), such as file-scoped namespaces, primary constructors, collection expressions, and pattern matching where applicable.
*   **Private Fields**: Use `camelCase` for private fields (do NOT prefix with underscores). Example: `private int someCount;`.
*   Maintain docstring and comment integrity. Never delete or modify existing comments unless directly requested or refactoring the exact lines they describe.
*   Ensure proper asynchronous programming: always use `await` where appropriate and append `Async` to asynchronous method signatures.
*   Use standard .NET capitalization and naming conventions:
    *   `PascalCase` for classes, methods, properties, and namespaces.
    *   `camelCase` for local variables, method parameters, and private fields.

## 3. Testing & Verification
*   **TypeScript Unit Tests**: When modifying DOM operations, selection mapping, or pagination/layouting logic, run `npm test` from `src/DScratch.Client/BrowserInteractions/Scripts`.
*   **Core .NET Unit Tests**: Whenever modifying core editor logic, CRDT trees, or client states, run `dotnet test tests/DScratch.Tests` to verify correctness.
*   **E2E Tests**: Make sure new features or elements are covered by E2E tests when applicable (`dotnet test tests/DScratch.E2E`).
*   Never assume changes work without verifying that the unit tests build and pass successfully.
