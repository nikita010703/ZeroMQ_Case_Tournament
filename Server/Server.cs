using NetMQ;
using NetMQ.Sockets;

// Создание сокета и привязка к порту
var server = new ResponseSocket();
try {
    server.Bind("tcp://localhost:5555");
}
catch (Exception ex) {
    server.Close();
    Environment.Exit(-1);
}

// Главный цикл, в котором приложения обмениваются сообщениями
try {
    while (true) {
        // Получение строки
        string message = server.ReceiveFrameString();

        // Получение сообщения об остановке работы
        if (message == "exit") break;

        // Обработка числа
        int num = int.Parse(message);
        num *= num;

        // Отправка результата в виде строки
        server.SendFrame(num.ToString());
    }
}
// Закрытие сокета
finally {
    try {
        server.Unbind("tcp://localhost:5555");
    }
    finally {
        server.Close();
    }
}