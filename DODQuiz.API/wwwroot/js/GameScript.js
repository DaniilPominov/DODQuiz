// Проверка аутентификации и прав пользователя
function CheckAuth() {
    fetch('api/Login/CheckRoles') // Путь для проверки статуса аутентификации
        .then(response => {
            if (!response.ok) {
                // Если пользователь не авторизован, перенаправляем на страницу логина
                window.location.href = '/login.html'; // Замените на ваш URL страницы логина
                console.log(response);
                return;
            }
            return response.json(); // Получаем данные о пользователе
        })
        .then(data => {
            const access = true ? data.userroles.includes("user") : false;
            console.log(access);
            if (data && !access) {
                // Если пользователь авторизован, но не имеет доступа
                document.getElementById('errorMessage').textContent = 'У вас нет прав для доступа к этому содержимому.';
            } else {
                // Если пользователь авторизован и имеет доступ
                document.getElementById('protectedContent').style.display = 'block';
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            document.getElementById('errorMessage').textContent = 'Произошла ошибка при проверке аутентификации.';
        });
}
CheckAuth();