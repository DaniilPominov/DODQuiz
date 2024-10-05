var webSocket = new WebSocket('ws://192.168.31.225:5072/ws');
let btn = document.getElementById("check-wss");
function detector() {
    console.log()
};

webSocket.onopen = (event) => {
    console.log("Connection opened");
};
webSocket.onmessage = function (event) {
    const mes = JSON.parse(event.data);
    let questioname = document.getElementById("question-name");
    let questiontext = document.getElementById("question-text");
    let questionimg = document.getElementById("question-image");
    questioname.textContent = mes.Name;
    questiontext.textContent = mes.Description;
    questionimg.src = mes.ImageUri;
    console.log(mes);
};

webSocket.onclose = (event) => {
    console.log("Connection closed");
};