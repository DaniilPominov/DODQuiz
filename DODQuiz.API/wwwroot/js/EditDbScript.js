//const apiUrl = '/api'; // Базовый URL для API

// Функция для получения всех объектов и отображения их в таблице
async function fetchObjects() {
    const response = await fetch(`api/Admin/GetAllQuestions`);
    const objects = await response.json();
    const tableBody = document.getElementById('objectsTable').querySelector('tbody');
    tableBody.innerHTML = ''; // Очищаем таблицу перед добавлением новых данных

    objects.forEach(obj => {
        const row = document.createElement('tr');
        row.innerHTML = `
                    <td>${obj.id}</td>
                    <td>${obj.name}</td>
                    <td>${obj.description}</td>
                    <td>${obj.category}</td>
                    <td><img src="${obj.imageUri}" alt="${obj.name}" style="width: 100px;"></td>
                    <td>
                        
                        <button class="button" onclick="deleteObject('${obj.id}')">Удалить</button>
                    </td>
                `;
        //<button class="button" onclick="editObject('${obj.id}')">Редактировать</button>
        tableBody.appendChild(row);
    });
}

// Функция для добавления или редактирования объекта
async function saveObject() {
    const id = document.getElementById('objectId').value;
    const name = document.getElementById('name').value;
    const description = document.getElementById('description').value;
    const category = document.getElementById('category').value;
    const imageUri = document.getElementById('imageUri').value;
    const response = await fetch(`api/Admin/AddQuestion`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id, name, description, category, imageUri })
    });

    if (response.ok) {
        resetForm();
        fetchObjects(); // Обновляем таблицу
    } else {
        alert('Ошибка при сохранении объекта.');
    }
}
function findRowById(id) {
    const table = document.getElementById('objectsTable');
    const rows = table.getElementsByTagName('tr'); // Получаем все строки таблицы

    for (let i = 1; i < rows.length; i++) { // Начинаем с 1, чтобы пропустить заголовок
        const cells = rows[i].getElementsByTagName('td');
        if (cells.length > 0 && cells[0].innerText === id.toString()) { // Сравниваем ID

            return rows[i]; // Завершаем выполнение функции
        }
    }
}

// Функция для редактирования объекта
function editObject(id) {
    const row = findRowById(id);
    console.log(row);
    document.getElementById('objectId').value = id;
    document.getElementById('name').value = row.children[1].innerText;
    document.getElementById('description').value = row.children[2].innerText;
    document.getElementById('category').value = row.children[3].innerText;
    document.getElementById('imageUri').value = row.children[4].querySelector('img').src;
}

// Функция для удаления объекта
async function deleteObject(id) {
    console.log(id);
    const response = await fetch(`api/Admin/DeleteQuestion?id=${id}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }//,
        //body: JSON.stringify({ id })
    });

    if (response.ok) {
        fetchObjects(); // Обновляем таблицу
    } else {
        alert('Ошибка при удалении объекта.');
    }
}

// Функция для сброса формы
function resetForm() {
    document.getElementById('objectId').value = '';
    document.getElementById('name').value = '';
    document.getElementById('description').value = '';
    document.getElementById('category').value = '';
    document.getElementById('imageUri').value = '';
}

// Инициализация
document.getElementById('saveButton').addEventListener('click', saveObject);
fetchObjects(); // Загружаем объекты при загрузке страницы