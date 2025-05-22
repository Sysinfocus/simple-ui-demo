let started = false;
let eventSource;

export const init = (url, dno) => {
    started = true;
    eventSource = new EventSource(url);
    if (!started) return;
    eventSource.onopen = e => dno?.invokeMethodAsync('HandleOnOpen', "The connection has been established.");
    eventSource.onmessage = e => dno?.invokeMethodAsync('HandleOnMessage', e.data, e.lastEventId, e.origin, e.ports, e.source);
    eventSource.onerror = e => dno?.invokeMethodAsync('HandleOnError', "An error occurred while attempting to connect.");    
}

export const stop = () => {
    started = false;
    eventSource.close();
}