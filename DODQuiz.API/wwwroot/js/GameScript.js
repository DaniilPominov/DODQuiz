var ws;
var reconnectInterval = 1000;
var timerInterval = 1000;

function starttimer() {
    setInterval(async () => {
        const response = await fetch('/api/Game/Timer');
        if (response.ok) {
            document.getElementById('timer').innerHTML = ":";
            let timeRemaining = await response.json();
            if (timeRemaining <= 0) {
                openModal("Timer end");
                clearInterval();
            }
            let time = convertSeconds(timeRemaining);
            document.getElementById('timer').innerText = `${time["minutes"]}:${time["seconds"]}`;
        }
    }, timerInterval);
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
        starttimer();
        
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