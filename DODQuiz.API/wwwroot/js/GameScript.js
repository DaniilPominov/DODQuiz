var ws;
var reconnectInterval = 1000;
var serverIp;
var port = 5072;
getIp();
var MatJax = document.createElement('script');
MatJax.setAttribute('src', 'https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.7/MathJax.js?config=TeX-AMS_HTML');
document.head.appendChild(MatJax);

async function getIp() {
    try {
        const response = await fetch('/api/Game/GetIp');
        if (!response.ok) throw new Error('Ошибка при загрузке пользователей');
        let ipPrepare = response.url.split(":")[1];
        console.log(ipPrepare.replace("//", ""));
        serverIp = ipPrepare.replace("//", "");
    }
    catch (error) {
        console.error(error);
    }
};
function timerHandler(timeRemaining) {
    let time = convertSeconds(timeRemaining);
    if (Number.parseInt(timeRemaining) > 0) {
        
    }
    else {
        openModal("timer end");
    }
    document.getElementById('timer').innerText = `${time["minutes"]}:${time["seconds"]}`;
}
function convertSeconds(totalSeconds) {
    const minutes = Math.floor(totalSeconds / 60); // Получаем полные минуты
    const seconds = totalSeconds % 60; // Получаем оставшиеся секунды

    return {
        minutes: minutes,
        seconds: seconds
    };
}
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
        openModal("You win!");
        return response;
    }
    catch (error) {
        console.error(error);
    }
};
const sendbtn = document.getElementById("root-code-buttond");
sendbtn.addEventListener("click", changePlayerStatus);
var connect = function () {
    ws = new WebSocket(`ws://${serverIp}:${port}/ws`);
    ws.onopen = (event) => {
        console.log("Connection opened");
    };
    ws.onmessage = function (event) {
        const mes = JSON.parse(event.data);
        console.log(mes);
        let questioname = document.getElementById("question-name");
        let questiontext = document.getElementById("question-text");
        let questionimg = document.getElementById("question-image");
        let questionData = mes["question"];
        let timerData = mes["timer"];
        MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        if (!(questionData == undefined)) {
            const modal = document.getElementById("myModal");
            modal.style.display = "none";
            let questionJson = JSON.parse(questionData);
            questioname.textContent = questionJson['Name'];
            questiontext.textContent = questionJson['Description'];
            questionimg.src = questionJson['ImageUri'];
        }    
        if (!(timerData == undefined)) {
            timerHandler(Number.parseInt(timerData));
        }
    };

    ws.onclose = (event) => {
        console.log("Connection closed");
        setTimeout(connect, reconnectInterval);
    };
};
connect();

const modal = document.getElementById("myModal");

function openModal(message) {
    modal.style.display = "block";
    const modalcontent = document.getElementById("modal-content");
    modalcontent.innerHTML = `<span id="modal-close" class="close">&times;</span>
    ${message}
    `;
    const span = document.getElementById("modal-close");
    span.onclick = function () {
        modal.style.display = "none";
    }
}
// Функция для закрытия модального окна при нажатии на крестик