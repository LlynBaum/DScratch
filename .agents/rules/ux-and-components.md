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
*   **Reference Baseline**: Align styles with the clean, minimalist look of the document editor (refer to [DocumentEditor.razor.css](file:///home/darki/Developement/DScratch/src/DScratch.Client/Pages/Editor/Components/DocumentEditor.razor.css) and [EditorMenu.razor.css](file:///home/darki/Developement/DScratch/src/DScratch.Client/Pages/Editor/Components/EditorMenu.razor.css)).
*   **CSS Isolation**: Prefer CSS isolation via `.razor.css` files instead of global stylesheets or inline styles.
*   **Color Palette**: Use the neutral slate and cool gray palette:
    *   Backgrounds: `#ffffff` (white), `#f8fafc` (slate-50), `#f1f5f9` (slate-100), `#e2e8f0` (slate-200).
    *   Borders: `#cbd5e1` (slate-300), `#94a3b8` (slate-400).
    *   Text: `#0f172a` (slate-900 / primary), `#334155` (slate-700 / secondary), `#64748b` (slate-500 / muted).
    *   Accents: `#2563eb` (blue-600 / primary active), `#10b981` (emerald-500 / success state).
*   **Typography**:
    ```css
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
    ```
*   **Interactive States**: Add smooth transitions and subtle hover/active offsets:
    ```css
    transition: all 0.12s ease-in-out;
    ```
    *   Hover: Shift background slightly darker (e.g. white to `#f1f5f9`), borders to `#94a3b8`, and text to `#0f172a`.
    *   Active/Pressed: Shift background to `#e2e8f0` and remove shadow offset.
