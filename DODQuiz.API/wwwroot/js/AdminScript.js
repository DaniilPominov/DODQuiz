// Пример таймера
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
    const contentdiv = document.getElementById("protectedContent");
    contentdiv.innerHTML = '';
    for (let i = 1; i <= k; i++) {
        let cell = document.createElement("div");
        cell.id = `user-${i}-container`;
        cell.className = "image-container";
        cell.innerHTML = `<div class="image-frame">
        <img src="https://via.placeholder.com/200" alt="Изображение ${i}">
            <div id="category-container">
                <label for="userSelect">Выберите пользователя:</label>
                <select id="userSelect">
                    <option value="">Загрузка...</option>
                </select>

                <br><br>

                    <label for="categorySelect">Выберите категорию:</label>
                    <select id="categorySelect">
                        <option value="">Загрузка...</option>
                    </select>

                    <br><br>

                        <button id="submitUserCategory">Подтвердить выбор</button>
                        <div id="result"></div>
    </div>`;

        contentdiv.appendChild(cell);
    }
}
let i = 3;
generateborders(i);
starttimer();
//<div class="image-container">
//    <div class="image-frame">
//        <img src="https://via.placeholder.com/200" alt="Изображение 1">
//            <div id="category-container">
//                <label for="userSelect">Выберите пользователя:</label>
//                <select id="userSelect">
//                    <option value="">Загрузка...</option>
//                </select>

//                <br><br>

//                    <label for="categorySelect">Выберите категорию:</label>
//                    <select id="categorySelect">
//                        <option value="">Загрузка...</option>
//                    </select>

//                    <br><br>

//                        <button id="submitUserCategory">Подтвердить выбор</button>
//                        <div id="result"></div>
//    </div>
//</div>