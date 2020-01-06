var websocket;
function createWebSocketConnection(link) {
    if('WebSocket' in window){
        websocket = new WebSocket('ws://localhost:6481');
        console.log("======== websocket ===========", websocket);

        websocket.onopen = function() {
            websocket.send(link);
        };

        websocket.onclose = function() { console.log("==== websocket closed ======"); };
    }
}

chrome.webRequest.onBeforeRequest.addListener(
    function (details) {
        if (details.type == "script")
            return;

        if (details.url.includes("get-mp3")) {
            createWebSocketConnection(details.url);
        }
    }, {
        urls: ["https://*.storage.yandex.net/get-mp3/*"]
    },
    ["blocking"]);