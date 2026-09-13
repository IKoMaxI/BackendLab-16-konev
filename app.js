const tokenElement = document.getElementById('token');
const resultElement = document.getElementById('result');

function showResult(status, data) {
    resultElement.textContent = `Статус: ${status}\n${JSON.stringify(data, null, 2)}`;
}

async function login(username, password) {
    const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    });
    const data = await response.json();

    if (response.ok) {
        localStorage.setItem('jwt_token', data.token);
        tokenElement.textContent = data.token;
    }

    showResult(response.status, data);
}

async function testEndpoint(endpoint) {
    const token = localStorage.getItem('jwt_token');
    const response = await fetch(`/api/user/${endpoint}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token || ''}`,
            'Content-Type': 'application/json'
        }
    });

    const contentType = response.headers.get('content-type');
    const data = contentType?.includes('application/json')
        ? await response.json()
        : { message: response.statusText || 'Ответ без тела' };
    showResult(response.status, data);
}

document.getElementById('loginForm').addEventListener('submit', event => {
    event.preventDefault();
    login(
        document.getElementById('loginUsername').value,
        document.getElementById('loginPassword').value);
});

document.getElementById('registerForm').addEventListener('submit', async event => {
    event.preventDefault();
    const response = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            username: document.getElementById('registerUsername').value,
            password: document.getElementById('registerPassword').value,
            role: document.getElementById('registerRole').value
        })
    });
    showResult(response.status, await response.json());
});

document.querySelectorAll('[data-endpoint]').forEach(button => {
    button.addEventListener('click', () => testEndpoint(button.dataset.endpoint));
});

document.querySelectorAll('[data-user]').forEach(button => {
    button.addEventListener('click', () => login(button.dataset.user, button.dataset.password));
});

document.getElementById('clearToken').addEventListener('click', () => {
    localStorage.removeItem('jwt_token');
    tokenElement.textContent = 'Токен не получен';
    resultElement.textContent = 'Токен удалён. Защищённые запросы вернут 401.';
});

tokenElement.textContent = localStorage.getItem('jwt_token') || 'Токен не получен';
