// Щепин Кирилл Геннадьевич, БПИ2410-2, Вариант 3
// Анекдот: На чемпионате мира по футболу сборная цыган заняла полторы тыщи до вторника
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ClassLibrary;

class Program
{
    static void Main()
    {
        Streams streams = new Streams();
        List<IJSONObject> cults = new List<IJSONObject>();
        ConsoleKeyInfo inputKey;
        do
        {
            Menu.ShowMenu();
            inputKey = Console.ReadKey(true);
            Menu.Do(inputKey, ref streams, ref cults);
        }
        while (inputKey.Key != ConsoleKey.D7);
    }
}