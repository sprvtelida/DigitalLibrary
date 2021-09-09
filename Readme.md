# Digital Library
- О чём проект? Клиент-серверное веб-приложение которое позволяет отправлять запросы для получения физический копии книги из локальной библиотеки, либо скачивать электронную
версию книги.
- Зачем этот проект? Приложение было написано для дипломного проекта.
- Инструкция по сборке и запуску проекта:
Для того чтобы скомпилировать и запустить клиент-серверное приложение необходимо иметь установленную на системе: 
   - .NET 5 SDK; 
   -  Rider/Visual Studio 2019/.NET CLI;
   -  Node JS.
   
   ---
   
   Скачайте проект в любую удобную вам папку: `gh repo clone sprvtelida/DigitalLibrary`
   Запустите 2 проекта:
      - DigitalLibrary.API;
      - DigitalLibrary.Client;
   По-умолчанию серверная часть приложения запуститься на https://localhost:5001, а клиентская часть на https://localhost:5003.
   
   ---
   
   Чтобы функции: регистрации; восстановления пароля работали, необходимо указать электронную почту. В корне проекта DigitalLibrary.API создаем файл appsettings.json.</br>
   В этом файле добавляем следующее поле и изменяем значение каждого под-поля:
   ```json 
   "MailSettings": { 
    "Mail": "Example@gmail.com", 
    "DisplayName" : "Digital Library", 
    "Password": "password123", 
    "Host": "smtp.gmail.com",
    "Port": 587
  } 
  ```
  ```
  Mail: электронная почта
  DisplayName: имя email
  Password: пароль от email
  Host: имя почтового сервера
  Port: порт почтового сервера
  ```

Логин/пароль администратора:
	admin:admin
Логин/пароль модератора:
	moder:moder
Логин/пароль пользователя:
	user:user

- Структура проекта, архитектура приложения, API
   - Клиентская часть - SPA приложение разработанная на фреймворке Angular. Клиентская часть общается с сервером с помощью http-запросов;
   - Серверная часть - REST API разработанная на фреймворке ASP.NET Core.



