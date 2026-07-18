# DScratch UI/UX & Component Creation Guidelines

These guidelines apply when creating new debugging panels or styling UI elements within the editor application.

## 1. Creating Debugging Panels
When asked to create a new debugging panel or visualizer:
*   **Location**: Create the files in `src/DScratch.Client/Pages/Editor/Components/Debugging/`.
*   **Structure**: Always split the component into three files:
    *   `Debug[Name].razor`: Markup (HTML/Razor).
    *   `Debug[Name].razor.cs`: Code-behind class (logic and parameters), using the partial class pattern within the `DScratch.Client.Pages.Editor.Components.Debugging` namespace.
    *   `Debug[Name].razor.css`: Isolated styling (CSS).
*   **Integration**:
    *   Open `DebugPanel.razor`. Add a `<button>` inside the `.debug-tabs` container:
        ```razor
        <button class="tab-btn @(currentTab == nameof(Debug[Name]) ? "active" : "")" @onclick="@(() => SwitchTab(nameof(Debug[Name])))">Tab Label</button>
        ```
    *   Add a case inside the `@switch (currentTab)` statement:
        ```razor
        case nameof(Debug[Name]):
            <Debug[Name] />
            break;
        ```

## 2. UI Styling & Theme Guidelines
When designing or styling any component:
*   **Strict Separation of Concerns**: 
    *   **Document Content styling** is scoped via [document-styles.css](file:///home/darki/Developement/DScratch/src/DScratch.Client/wwwroot/document-styles.css) and [document-tokens.css](file:///home/darki/Developement/DScratch/src/DScratch.Client/wwwroot/document-tokens.css).
    *   **Editor UI styling** must ONLY use design tokens from [editor-tokens.css](file:///home/darki/Developement/DScratch/src/DScratch.Client/wwwroot/editor-tokens.css).
*   **CSS Isolation**: Use component CSS isolation via `.razor.css` files instead of global stylesheets or inline styles.
*   **Use Design Tokens (CSS Variables)**: Hardcoded hex colors, padding/margin dimensions, transitions, or typography definitions are forbidden in component `.razor.css` files. Always reference the variables from `editor-tokens.css`:
    *   Backgrounds: `var(--editor-bg-app)`, `var(--editor-bg-surface)`, `var(--editor-bg-hover)`, `var(--editor-bg-active)`.
    *   Borders: `var(--editor-border-color)`, `var(--editor-border-muted)`, `var(--editor-border-focus)`.
    *   Text: `var(--editor-text-primary)`, `var(--editor-text-secondary)`, `var(--editor-text-muted)`.
    *   Accents/States: `var(--editor-color-primary)`, `var(--editor-color-success)`, `var(--editor-color-danger)`.
    *   Typography: `var(--editor-font-family)`, font sizes like `var(--editor-font-size-sm)`.
    *   Spacing: `var(--editor-spacing-sm)`, `var(--editor-spacing-md)`, etc.
*   **Interactive States**: Add smooth transitions and subtle hover/active states utilizing variables:
    *   Transitions: `transition: var(--editor-transition-fast);`.
    *   Hover: Use `var(--editor-bg-hover)`, border `var(--editor-border-focus)`, text `var(--editor-text-primary)`.
    *   Active: Use `var(--editor-bg-active)`.
