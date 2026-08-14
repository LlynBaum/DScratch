export function registerLinks() {
    window.editor.node?.addEventListener("keydown", e => {
        if (e.key === "Control") {
            window.editor.node?.setAttribute("data-link-click-active", "");
        }
    });

    window.editor.node?.addEventListener("keyup", e => {
        if (e.key === "Control") {
            window.editor.node?.removeAttribute("data-link-click-active");
        }
    });
    
    window.editor.node?.addEventListener("click", e => {
        const link = (e.target as HTMLElement).closest("a");
       if (!link) return;
        e.preventDefault();
        
        const shouldNavigate = e.ctrlKey || e.metaKey;
        if (!shouldNavigate) return;

        const url = link.href;
        if (!url) return;
        const target = link.getAttribute('target') || '_self';

        if (target === '_blank') {
            // Opens in a new tab; noopener/noreferrer helps security & performance
            window.open(url, '_blank', 'noopener,noreferrer');
        } else {
            window.open(url, target);
        }
    });
}