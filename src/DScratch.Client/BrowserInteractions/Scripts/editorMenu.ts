import {
    clearFakeSelection,
    getEditorSelection,
    restoreEditorSelection,
    showFakeSelection
} from "./selection";
import {findNode} from "./nodeHelper";

const ADD_LINK_POPOVER = "add-link-popover";
const LINK_SETTINGS_POPOVER = "link-settings-popover";

export function registerMenu() {
    registerAddLink();
    registerLinkSettings();
}

function registerAddLink() {
    const addLinkButton = document.getElementById("add-link");
    const popover = document.getElementById(ADD_LINK_POPOVER);
    if (!addLinkButton || !popover) return;
    
    const displayTextInput = popover.querySelector<HTMLElement>(".display-text");

    popover.addEventListener("beforetoggle", e => {
        if (e.newState === "closed") {
            restoreEditorSelection();
            clearFakeSelection();
        } else {
            showFakeSelection();
            
            const selection = getSelection();
            displayTextInput!.style.display = selection?.isCollapsed ? "flex" : "none";
            positionPopover();
        }
    });

    popover.querySelector<HTMLInputElement>("input.link-url")?.addEventListener("keydown", e => {
        if (e.key === "Enter") {
            e.preventDefault();
            if (!popover.querySelector("button")?.disabled) {
                closePopover();
            }
        }
    });
    
    function closePopover() {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-anchor]");
        previousAnchor?.removeAttribute("data-link-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);
        popover?.hidePopover();
    }
    
    function positionPopover() {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-anchor]");
        previousAnchor?.removeAttribute("data-link-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);

        const selection = getEditorSelection();
        if (!selection) return;

        const targetElement = findNode(selection.focusId || selection.anchorId);
        if (!targetElement) return;

        targetElement.style.setProperty("anchor-name", `--${ADD_LINK_POPOVER}`);
        targetElement.setAttribute("data-link-anchor", "");
    }
}

function registerLinkSettings() {
    const popover = document.getElementById(LINK_SETTINGS_POPOVER);
    if (!popover) return;

    document.addEventListener("selectionchange", () => {
        
        const selection = getEditorSelection();
        if (!selection || selection.direction !== "none") {
            hideLinkSettings();
            return;
        }
        
        const targetElement = findNode(selection.anchorId);
        if (!targetElement) {
            hideLinkSettings();
            return;
        }
        
        const link = targetElement.closest("a");
        if(!link) {
            hideLinkSettings();
            return;
        }

        if (!targetElement.hasAttribute("data-link-settings-anchor")) {
            const previousAnchor = document.querySelector<HTMLElement>("[data-link-settings-anchor]");
            previousAnchor?.removeAttribute("data-link-settings-anchor");
            previousAnchor?.style.setProperty("anchor-name", null);

            popover.querySelector<HTMLInputElement>("input.link-url")!.value = link.href;
        }
        
        targetElement.style.setProperty("anchor-name", `--${LINK_SETTINGS_POPOVER}`);
        targetElement.setAttribute("data-link-settings-anchor", "");
        popover.showPopover();
    });
    
    popover.addEventListener("beforetoggle", e => {
        if (e.newState !== "open") return;

        const selection = getEditorSelection();
        if (!selection || selection.direction !== "none") return;
        const targetElement = findNode(selection.anchorId);
        const link = targetElement?.closest("a");
        if(!link) return;
        popover.querySelector<HTMLInputElement>("input.link-url")!.value = link.href;
    });
    
    popover.querySelector<HTMLElement>(".remove-link")?.addEventListener("click", hideLinkSettings);

    function hideLinkSettings() {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-settings-anchor]");
        previousAnchor?.removeAttribute("data-link-settings-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);
        popover?.hidePopover();
    }
}