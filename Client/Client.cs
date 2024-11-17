using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;

// Запуск сервера
Process process = new Process();
try {
    process.StartInfo.FileName = "Server.exe";
    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.Start();
}
catch {
    Console.WriteLine("Сервер не был запщуен. Поместите оба приложения в одну директорию.");

    Console.WriteLine("\nНажмите любую клавишу для закрытия окна");
    Console.ReadKey();
    Environment.Exit(-1);
}
Console.WriteLine("Сервер запущен");

// Создание и подключение сокета к порту 5555
RequestSocket client = new RequestSocket();
try {
    client.Connect("tcp://localhost:5555");
}
catch (Exception ex) {
    Console.WriteLine("Ошибка при подключении к порту:\n" + ex.ToString());

    process.Close();
    Console.WriteLine("\nСервер остановлен");

    Console.WriteLine("Нажмите любую клавишу для закрытия окна");
    Console.ReadKey();
    Environment.Exit(-1);
}

// Главный цикл, в котором приложения обмениваются сообщениями
try {
    while (true) {
        Console.WriteLine("Введите число:");

        int num;
        string input = Console.ReadLine();

        // Сообщение о закрытии приложений
        if (input == "exit") {
            client.SendFrame("exit");
            break;
        }

        // Обработка неправильного ввода
        if (!int.TryParse(input, out num)) {
            Console.Write("Неправильный ввод. ");
            continue;
        }
        client.SendFrame(num.ToString());

        // Обработка невозможности получения сообщения (сервер закрылся раньше, чем клиент)
        string? message;
        if (!client.TryReceiveFrameString(TimeSpan.FromSeconds(1), out message))
            throw new Exception("Time out");

        Console.WriteLine(message);
    }
}
catch (Exception ex) {
    Console.WriteLine("Ошибка при сообщении клиент-сервер. " + ex.Message);
}
// Закрытие сокетов и сервера
finally {
    try {
        client.Disconnect("tcp://localhost:5555");
    }
    finally {
        client.Close();
    }

    process.Close();
    Console.WriteLine("Сервер остановлен");

    Console.WriteLine("\nНажмите любую клавишу для закрытия окна");
    Console.ReadKey();
}