import {clearFakeSelection, getEditorSelection, restoreEditorSelection, showFakeSelection} from "./selection";
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
    
    addLinkButton.addEventListener("click", () => {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-anchor]");
        previousAnchor?.removeAttribute("data-link-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);
        popover.hidePopover();
        
        const selection = getEditorSelection();
        if (!selection) return;
        
        const targetElement = findNode(selection.focusId || selection.anchorId);
        if (!targetElement) return;
        
        targetElement.style.setProperty("anchor-name", `--${ADD_LINK_POPOVER}`);
        targetElement.setAttribute("data-link-anchor", "");
        popover.showPopover();
    });

    popover.addEventListener("beforetoggle", e => {
        if(e.newState === "closed") {
            restoreEditorSelection();
            clearFakeSelection();
        } else {
            showFakeSelection();
            
            const selection = getSelection();
            displayTextInput!.style.display = selection?.isCollapsed ? "flex" : "none";
        }
    });

    popover.querySelector("button")?.addEventListener("click", closePopover);
    popover.querySelector<HTMLInputElement>("input.link-url")?.addEventListener("keydown", e => {
        if (e.key === "Enter") {
            e.preventDefault();
            closePopover();
        }
    });
    
    function closePopover() {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-anchor]");
        previousAnchor?.removeAttribute("data-link-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);
        popover?.hidePopover();
    }
}

function registerLinkSettings() {
    const popover = document.getElementById(LINK_SETTINGS_POPOVER);
    if (!popover) return;

    document.addEventListener("selectionchange", () => {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-settings-anchor]");
        previousAnchor?.removeAttribute("data-link-settings-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);
        popover.hidePopover();
        
        const selection = getEditorSelection();
        if (!selection || selection.direction !== "none") return;
        
        const targetElement = findNode(selection.anchorId);
        if (!targetElement) return;
        if(!targetElement.closest("a")) return;
        
        targetElement.style.setProperty("anchor-name", `--${LINK_SETTINGS_POPOVER}`);
        targetElement.setAttribute("data-link-settings-anchor", "");
        popover.showPopover();
    });
    
    popover.querySelector<HTMLElement>(".remove-link")?.addEventListener("click", () => {
        const previousAnchor = document.querySelector<HTMLElement>("[data-link-settings-anchor]");
        previousAnchor?.removeAttribute("data-link-settings-anchor");
        previousAnchor?.style.setProperty("anchor-name", null);
        popover.hidePopover();
    });
}