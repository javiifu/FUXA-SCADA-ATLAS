let onFuxaSelectionHandler;

function parseSelectionId(data) {
    if (!data) {
        return null;
    }

    if (typeof data === 'string') {
        const trimmed = data.trim();
        if (!trimmed) {
            return null;
        }

        try {
            const asJson = JSON.parse(trimmed);
            return parseSelectionId(asJson);
        } catch {
            return trimmed;
        }
    }

    if (typeof data === 'object') {
        if (data.type === 'fuxa-object-selected' || data.type === 'fuxa-selection') {
            return data.id ?? data.objectId ?? data.fuxaObjectId ?? data.tag ?? null;
        }

        return data.id ?? data.objectId ?? data.fuxaObjectId ?? data.tag ?? null;
    }

    return null;
}

export function startFuxaSelectionBridge(dotNetRef) {
    stopFuxaSelectionBridge();

    onFuxaSelectionHandler = (event) => {
        const selectionId = parseSelectionId(event.data);
        if (!selectionId) {
            return;
        }

        dotNetRef.invokeMethodAsync('OnFuxaObjectSelected', `${selectionId}`);
    };

    window.addEventListener('message', onFuxaSelectionHandler);
}

export function stopFuxaSelectionBridge() {
    if (onFuxaSelectionHandler) {
        window.removeEventListener('message', onFuxaSelectionHandler);
        onFuxaSelectionHandler = null;
    }
}
