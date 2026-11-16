// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

// A pasted file is handed to the file input as if it had been chosen, so InputFile reads it like any other file.
export function attachPaste(input, accept) {
    const patterns = (accept ?? "").split(",").map(pattern => pattern.trim().toLowerCase()).filter(Boolean);

    const matches = file => patterns.length === 0 || patterns.some(pattern => {
        if (pattern.startsWith(".")) {
            return file.name.toLowerCase().endsWith(pattern);
        }

        if (pattern.endsWith("/*")) {
            return file.type.toLowerCase().startsWith(pattern.slice(0, -1));
        }

        return file.type.toLowerCase() === pattern;
    });

    const handler = event => {
        const file = [...(event.clipboardData?.files ?? [])].find(matches);

        if (!file || input.disabled) {
            return;
        }

        event.preventDefault();
        const transfer = new DataTransfer();
        transfer.items.add(file);
        input.files = transfer.files;
        input.dispatchEvent(new Event("change", { bubbles: true }));
    };

    document.addEventListener("paste", handler);

    return { dispose: () => document.removeEventListener("paste", handler) };
}
