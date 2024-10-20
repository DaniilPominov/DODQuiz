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
        textbox.value = "";
        return response;
    }
    catch (error) {
        console.error(error);
    }
};
const sendbtn = document.getElementById("root-code-buttond");
sendbtn.addEventListener("click", changePlayerStatus);
var connect = async function () {
    ws = new WebSocket(`ws://${serverIp}:${port}/ws`);
    ws.onopen = async (event) => {
        console.log("Connection opened");
        const response = await fetch("api/Game/MyQuestion");
        if (response) {
            let questionData = await response.json();
            await fillQuestion(JSON.stringify(questionData));

        }

    };
    ws.onmessage = async function (event) {
        const mes = JSON.parse(event.data);
        let questionData = mes["question"];
        let timerData = mes["timer"];
        if (!(questionData == undefined)) {
            await fillQuestion(questionData);
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
async function fillQuestion(questionData) {
    let questioname = document.getElementById("question-name");
    let questiontext = document.getElementById("question-text");
    let questionimg = document.getElementById("question-image");
    const modal = document.getElementById("myModal");
    modal.style.display = "none";
    let questionJson = JSON.parse(questionData);
    questioname.textContent = questionJson['Name'];
    questiontext.textContent = questionJson['Description'];
    questionimg.src = questionJson['ImageUri'];
    MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
}
const modal = document.getElementById("myModal");

function openModal(message) {
    if (modal.style.display != "block") {
        let path = "/images/key.png";
        if (message == "timer end") {
            path = "/images/close.png";
            message = "К сожалению время вышло и вы не успели получить ключ"
        }
        else {
            message = "Ключ получен!"
        }
        modal.style.display = "block";
        const modalcontent = document.getElementById("modal-content");
        modalcontent.innerHTML = `<span id="modal-close" class="modal">dfghjkhg</span>
      <img src="${path}" alt="Картина" class="modal-image">
      <p class="modal-text">${message}</p>`;
        const span = document.getElementById("modal-close");
        span.style.display = 'block';
        span.onclick = function () {
            modal.style.display = "none";
        }
    }
}
// Функция для закрытия модального окна при нажатии на крестик