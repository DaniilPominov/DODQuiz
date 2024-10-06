var ws;
var reconnectInterval = 1000;
let btn = document.getElementById("check-wss");

async function changePlayerStatus() {
    let textbox = document.getElementById("root-code");
    let code = textbox.value;
    try {
        const response = await fetch(`api/Game/ChangeUserStatus?code=${code}`, {
            method: "PUT"
        });
        if (!response.ok) {
            throw new Error('Ошибка при изменении статуса');
        }
        return response;
    }
    catch (error) {
        console.error(error);
    }
};
const sendbtn = document.getElementById("root-code-buttond");
sendbtn.addEventListener("click", changePlayerStatus);
var connect = function () {
    ws = new WebSocket('ws://192.168.31.225:5072/ws');
    ws.onopen = (event) => {
        console.log("Connection opened");
    };
    ws.onmessage = function (event) {
        const mes = JSON.parse(event.data);
        let questioname = document.getElementById("question-name");
        let questiontext = document.getElementById("question-text");
        let questionimg = document.getElementById("question-image");
        questioname.textContent = mes.Name;
        questiontext.textContent = mes.Description;
        questionimg.src = mes.ImageUri;
        console.log(mes);
    };

    ws.onclose = (event) => {
        console.log("Connection closed");
        setTimeout(connect, reconnectInterval);
    };
};
connect();