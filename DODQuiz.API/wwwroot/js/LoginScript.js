function login() {
    document.getElementById('submitLogin').addEventListener('click', function () {
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        // Проверка на заполненность полей
        if (!username || !password) {
            alert('Пожалуйста, заполните все поля.');
            return;
        }

        // Отправка данных на сервер
        fetch('api/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: username,
                password: password
            })
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Ошибка при входе');
                }
            })
            .then(data => {
                console.log('Успешный вход:', data);
                // Здесь можно добавить логику, например, перенаправление
                if (username == "Admin") {
                    window.location.href = 'admin.html';
                }
                else {
                    window.location.href = 'game.html';
                }
            })
            .catch(error => {
                console.error('Ошибка:', error);
                alert('Ошибка при входе. Попробуйте еще раз.');
            });
    });
}
login();