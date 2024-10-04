async function checkAuthentication() {
    try {
        const response = await fetch('api/Login/CheckRoles'); // Замените на ваш API эндпоинт
        if (!response.ok) {
            window.location.href = '/login.html'; // Замените на ваш URL страницы логина
            //            console.log(response);
            return;
        }
        const data = await response.json();
        return data; // Ожидаем, что сервер вернет объект с информацией о пользователе
    } catch (error) {
        console.error("Ошибка:", error);
        return null; // Возвращаем null в случае ошибки
    }
}
async function initialize() {
    const userData = await checkAuthentication();
    const loadingDiv = document.getElementById('loading');
    const contentDiv = document.getElementById('protectedContent');
    const errorDiv = document.getElementById('errorMessage');

    if (userData) {
        // Проверяем, есть ли у пользователя необходимые роли
        if (userData.userroles.includes('admin')) { // Замените на ваши роли
            loadingDiv.style.display = 'none'; // Скрываем сообщение о загрузке
            contentDiv.style.display = 'block'; // Показываем содержимое
        } else {
            contentDiv.innerHTML = '';
            loadingDiv.style.display = 'none';
            errorDiv.textContent = "У вас нет доступа к этому содержимому.";
        }
    } else {
        loadingDiv.style.display = 'none';
        errorDiv.textContent = "Не удалось проверить аутентификацию.";
    }
}

// Запуск инициализации
initialize();