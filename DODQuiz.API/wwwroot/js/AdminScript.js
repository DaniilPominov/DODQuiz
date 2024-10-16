var playercount = 3;
var serverIp;
var port = 5072;
getIp();

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

async function timerHandler(timeRemaining) {
    let time = await convertSeconds(timeRemaining);
    document.getElementById('timer').innerText = `${time["minutes"]}:${time["seconds"]}`;
}
async function convertSeconds(totalSeconds) {
    const minutes = Math.floor(totalSeconds / 60); // Получаем полные минуты
    const seconds = totalSeconds % 60; // Получаем оставшиеся секунды

    return {
        minutes: minutes,
        seconds: seconds
    };
}
function generateborders(k) {
    const protectedDiv = document.getElementById("protectedContent");
    const contentdiv = document.createElement("div");
    contentdiv.className = "pagecontent";
    protectedDiv.innerHTML = '';
    protectedDiv.appendChild(contentdiv);
    for (let i = 1; i <= k; i++) {
        let cell = document.createElement("div");
        cell.id = `user-container${i}`;
        cell.className = `image-container${i}`;
        cell.innerHTML = `<div class="image-frame">
        <img id="user-image${i}" src="/images/200.png" alt="Изображение ${i}">
            <div id="category-container${i}">
                <label for="userSelect${i}">Выберите пользователя:</label>
                <select id="userSelect${i}">
                    <option value="">Загрузка...</option>
                </select>

                <br><br>

                    <label for="categorySelect${i}">Выберите категорию:</label>
                    <select id="categorySelect${i}">
                        <option value="">Загрузка...</option>
                    </select>

                    <br><br>

                        <button id="submitUserCategory${i}" onclick="submitSelection(${i})">Подтвердить выбор</button>
                        <div id="result${i}"></div>
    </div>`;

        contentdiv.appendChild(cell);
    }
    let buttonDiv = document.createElement("div");
    buttonDiv.className = "button-container";
    buttonDiv.innerHTML = "<br>";

    let buttonstart = document.createElement("button");
    buttonstart.id = "start-round-button";
    buttonstart.textContent = "Старт";
    buttonstart.className = "button";
    buttonstart.addEventListener("click", startround);

    let buttonend = document.createElement("button");
    buttonend.id = "stop-round-button";
    buttonend.textContent = "Завершить";
    buttonend.className = "button";
    buttonend.addEventListener("click", stopround);

    let timerbox = document.createElement("input");
    timerbox.id = "round-length";
    timerbox.placeholder = "Длина раунда (секунд)";
    buttonDiv.appendChild(timerbox);
    buttonDiv.appendChild(buttonend);
    buttonDiv.appendChild(buttonstart);
    

    protectedDiv.appendChild(buttonDiv);
}
async function loadUsers() {
    try {
        const response = await fetch('/api/Admin/GetUsers');
        if (!response.ok) throw new Error('Ошибка при загрузке пользователей');
        const users = await response.json();
      
        console.log("onload");
        console.log(users);
        return users;
    }
    catch (error) {
        console.error(error);
    }
}
async function loadUsersToSelect(i, usersList) {
    try {
        //const response = await fetch('/api/Admin/GetUsers');
        //if (!response.ok) throw new Error('Ошибка при загрузке пользователей');
        const users = usersList;

        const userSelect = document.getElementById(`userSelect${i}`);
        userSelect.innerHTML = ''; // Очищаем текущие опции
        let cnt = 0;
        users.forEach(user => {
            const option = document.createElement('option');
            option.value = user.id;
            if (cnt == i) {
                option.selected = true;
            }
            cnt++;
            option.textContent = user.name; // Предполагается, что объект пользователя имеет поля id и name
            userSelect.appendChild(option);
        });
    } catch (error) {
        console.error(error);
        document.getElementById(`result${i}`).textContent = 'Не удалось загрузить пользователей.';
    }
}

// Функция для получения категорий
async function loadCategories() {
    try {
        const response = await fetch('/api/Game/GetCategories');
        if (!response.ok) throw new Error('Ошибка при загрузке категорий');
        const categories = await response.json();
        console.log(categories);
        return categories;
    } catch (error) {
        console.error(error);
    }
}
async function loadCategoriesToSelect(i, categoriesList) {

    let categories = categoriesList;
    try {
        
        const categorySelect = document.getElementById(`categorySelect${i}`);
        categorySelect.innerHTML = ''; // Очищаем текущие опции
        let cnt = 1;
        categories.forEach(category => {
            const option = document.createElement('option');
            option.value = category;
            if (cnt == i) {
                option.selected = true;
            }
            cnt++;
            option.textContent = category; // Предполагается, что объект категории имеет поля id и name
            categorySelect.appendChild(option);
        });
        } catch (error) {
            console.error(error);
        }
}

// Функция для отправки выбранных данных
async function submitSelection(i) {
    const userId = document.getElementById(`userSelect${i}`).value;
    const categoryName = document.getElementById(`categorySelect${i}`).value;

    if (!userId || !categoryName) {
        document.getElementById(`result${i}`).textContent = 'Пожалуйста, выберите пользователя и категорию.';
        return;
    }

    try {
        const response = await fetch(`/api/Admin/SetUserCategory?userId=${userId}&categoryName=${categoryName}`, {
            method: 'POST'//,
        });

        if (!response.ok) throw new Error('Ошибка при сохранении категории');

        document.getElementById(`result${i}`).textContent = 'Категория успешно сохранена!';
    } catch (error) {
        console.error(error);
        document.getElementById(`result${i}`).textContent = 'Не удалось сохранить категорию.';
    }
}
async function startround() {
    const secinput = document.getElementById("round-length");
    let roundlen = 300;
    if (secinput) {
        if (Number.isInteger(Number(secinput.value))){
            roundlen = Number(secinput.value);
        }
    }
    console.log(roundlen);
    try {
        const response = await fetch(`/api/Game/StartRound?remainingTime=${roundlen}`, {
                method: 'POST'
            });

        if (!response.ok) throw new Error('Ошибка при старте раунда');
        //startTimer();
        return response;

    } catch (error) {
        console.error(error);
    }
}
async function stopround() {

}
async function combinepage() {
    let userList = await loadUsers();
    let categoryList = await loadCategories();
    generateborders(playercount);
    for (let j = 1; j <= playercount; j++) {
        loadUsersToSelect(j, userList);
        loadCategoriesToSelect(j, categoryList);
    }
}

async function handleuserstatus(statuses) {
    if (statuses) {
    let data = JSON.parse(statuses);
    
        for (let j = 1; j <= playercount; j++) {
            const userId = document.getElementById(`userSelect${j}`).value;
            const status = data[userId]
            if (status) {
                let userimg = document.getElementById(`user-image${j}`);
                userimg.src = "images/key.png";
            }
            else {
                let userimg = document.getElementById(`user-image${j}`);
                userimg.src = "https://via.placeholder.com/200";
            }

        }
    }
}
async function handleMusic(mus) {
    if (mus) {
        let data = JSON.parse(mus);
        if (data == true) {
            let music = document.getElementById("winner-music");
            music.play();
        }
    }
}


document.addEventListener('DOMContentLoaded', combinepage());
var reconnectInterval = 1000;
var ws;

var connect = async function () {
    if (!serverIp) {
        await getIp();
    }
    ws = new WebSocket(`ws://${serverIp}:${port}/wsadmin`);
    ws.onopen = (event) => {
        console.log("Connection opened");
    };
    ws.onerror = (event) => {
        console.log("Connection error");
    };
    ws.onmessage = async function (event) {
        const mes = JSON.parse(event.data);
        let timeRemaining = mes["timer"];
        let status = mes["statuses"];
        let music = mes["all-win"];
        await timerHandler(timeRemaining);
        await handleuserstatus(status);
        await handleMusic(music);
    };
    ws.onclose = (event) => {
        console.log("Connection closed");
        setTimeout(connect, reconnectInterval);
    };
};
connect();