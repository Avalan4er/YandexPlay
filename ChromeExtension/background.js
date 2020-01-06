// function loadSynchronously(url) {
//     $.ajax({
//         url : "url",
//         type : "get",
//         async: false,
//         success : function(response) {
//            return response;
//         },
//         error: function() {
//             return null;
//         }
//      });
// var request = new XMLHttpRequest();
// request.open('GET', '/bar/foo.txt', false); // `false` makes the request synchronous
// request.send(null);

// if (request.status === 200) {
//     console.log(request.responseText);
// }

// }

function loadSynchronously(url) {
    //JSON data
    var dataType = 'application/json';
    var data = {
        Link: url
    }

    console.log(JSON.stringify(data));
    var request = $.ajax({
        type: 'POST',
        url: 'http://localhost:6481/api/yandex',
        contentType: dataType,
        data: JSON.stringify(data),
        success: function (result) {
            console.log('Data received: ');
            console.log(result);
        }
    });
    console.log(request);
}

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