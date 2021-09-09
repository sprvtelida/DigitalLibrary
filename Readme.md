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
   В этом файле добавляем следующее поле:
   `json 
   "MailSettings": { </br>
    "Mail": "Example@gmail.com", // электронная почта </br>
    "DisplayName" : "Digital Library", // имя электронной почты </br>
    "Password": "password123", // пароль от электронной почты </br>
    "Host": "smtp.gmail.com", // имя сервера почты </br>
    "Port": 587 // порт сервера почты </br>
  } </br>
  `

Логин/пароль администратора:
	admin:admin
Логин/пароль модератора:
	moder:moder
Логин/пароль пользователя:
	user:user

- Структура проекта, архитектура приложения, API



