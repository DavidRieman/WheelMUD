// Copyright (c) WheelMUD Development Team.  See LICENSE.txt or https://github.com/DavidRieman/WheelMUD/#license

using WheelMUD.Telnet.Demo;

Console.WriteLine("Please press one of the following keys:");
Console.WriteLine(" S) Run demo in Server mode.");
Console.WriteLine(" C) Run demo in Client mode.");
Console.WriteLine(" Q) Exit.");

// For simplicity, just try to read key input until the first key press that we recognize.
while (true)
{
    var key = Console.ReadKey();
    Console.WriteLine();
    Console.WriteLine();

    switch (key.Key)
    {
        case ConsoleKey.S:
            Console.WriteLine("Running in Server mode. Creating server at port 32111. Will prefer to echo for client.");
            DemoServer.Run();
            return;
        case ConsoleKey.C:
            Console.WriteLine("Running in Client mode. Will connect to local port 32111. Will not echo the server.");
            DemoClient.Run();
            return;
        case ConsoleKey.Q:
            Console.WriteLine("Quitting.");
            return;
        default:
            Console.WriteLine("Invalid selection");
            break;
    }
}
