var playercount = 3;
function starttimer() {
    let seconds = 0;
    setInterval(() => {
        seconds++;
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        document.getElementById('timer').textContent = `Таймер: ${String(minutes).padStart(2, '0')}:${String(remainingSeconds).padStart(2, '0')}`;
    }, 1000);
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
        <img id="user-image${i}" src="https://via.placeholder.com/200" alt="Изображение ${i}">
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
    //let buttonsDiv = document.createElement("div");
    //buttonsDiv.innerHTML = `<button class="button" id="stop-round-button">Стоп</button>
    //        <button class="button" id="start-round-button">Старт</button>`;
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

    buttonDiv.appendChild(buttonstart);
    buttonDiv.appendChild(buttonend);
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
        users.forEach(user => {
            const option = document.createElement('option');
            option.value = user.id;
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
        categories.forEach(category => {
            const option = document.createElement('option');
            option.value = category;
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
            //headers: {
            //    'Content-Type': 'application/json'
            //},
            //body: JSON.stringify({ userId, categoryName }) // Отправляем выбранные данные в формате JSON
        });

        if (!response.ok) throw new Error('Ошибка при сохранении категории');

        document.getElementById(`result${i}`).textContent = 'Категория успешно сохранена!';
    } catch (error) {
        console.error(error);
        document.getElementById(`result${i}`).textContent = 'Не удалось сохранить категорию.';
    }
}
async function startround() {
    try {
        const response = await fetch(`/api/Game/StartRound`, {
            method: 'POST'
        });

        if (!response.ok) throw new Error('Ошибка при старте раунда');
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
    console.log(`userList`);
    console.log(userList);
    console.log(`categoryList`);
    console.log(categoryList);
    generateborders(playercount);
    for (let j = 1; j <= playercount; j++) {
        loadUsersToSelect(j, userList);
        loadCategoriesToSelect(j, categoryList);
    }
}

function handleuserstatus(data) {
    for (let j = 1; j <= playercount; j++) {
        const userId = document.getElementById(`userSelect${j}`).value;
        console.log(userId);
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


document.addEventListener('DOMContentLoaded', combinepage());
starttimer();
var reconnectInterval = 1000;
var ws;

var connect = function () {
    ws = new WebSocket('ws://192.168.31.225:5072/wsadmin');
    ws.onopen = (event) => {
        console.log("Connection opened");
    };
    ws.onerror = (event) => {
        console.log("Connection error");
    };
    ws.onmessage = function (event) {
        const mes = JSON.parse(event.data);
        handleuserstatus(mes);
    };
    ws.onclose = (event) => {
        console.log("Connection closed");
        setTimeout(connect, reconnectInterval);
    };
};
connect();
//webSocket.onopen = (event) => {
//    console.log("Connection opened");
//};
//webSocket.onmessage = function (event) {
//    const mes = JSON.parse(event.data);
//    console.log(mes);
//};

//webSocket.onclose = (event) => {
//    console.log("Connection closed");
//};