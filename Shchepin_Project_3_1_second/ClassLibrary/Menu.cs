using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ClassLibrary
{
    public static class Menu
    {
        // Следующие несколько методов нужны для информативного контакта с пользователем путем вывода сообщений с акцентом на их значение
        public static void ShowMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Метод для вывода основного меню
        /// </summary>
        public static void ShowMenu()
        {
            ShowMessage("Выберите действие:");
            ShowInfo("1. Ввести данные (консоль/файл)");
            ShowInfo("2. Отфильтровать данные");
            ShowInfo("3. Отсортировать данные");
            ShowInfo("4. <основная задача индивидуального варианта>");
            ShowInfo("5. <дополнительная задача индивидуального варианта>");
            ShowInfo("6. Вывести данные (консоль/файл)");
            ShowInfo("7. Окончание работы");
        }

        /// <summary>
        /// Метод обрабатывающий команды основного меню
        /// </summary>
        /// <param name="inputKey">Клавиша, нажатая пользователем</param>
        /// <param name="streams">Объект управляющий потоками ввода и вывода</param>
        /// <param name="cults">Список с данными об элементах</param>
        public static void Do(ConsoleKeyInfo inputKey, ref Streams streams, ref List<IJSONObject> cults)
        {
            switch (inputKey.Key)
            {
                case ConsoleKey.D1:
                    InputData(ref streams, ref cults, "upload");
                    break;
                case ConsoleKey.D2:
                    if (cults.Count == 0)
                    {
                        ShowError("Данные еще не введены");
                    }
                    else
                    {
                        Filter(ref cults);
                    }
                    break;
                case ConsoleKey.D3:
                    if (cults.Count == 0)
                    {
                        ShowError("Данные еще не введены");
                    }
                    else
                    {
                        Sort(ref cults);
                    }
                    break;
                case ConsoleKey.D4:
                    BaseTask(ref streams, ref cults);
                    break;
                case ConsoleKey.D5:
                    ShowError("Тут ничего нет");
                    break;
                case ConsoleKey.D6:
                    if (cults.Count == 0)
                    {
                        ShowError("Данные еще не введены");
                    }
                    else
                    {
                        OutputData(ref streams, ref cults);
                    }
                    break;
            }
        }

        /// <summary>
        /// Метода для обеспечения ввода данных для различных задач
        /// </summary>
        /// <param name="streams">Объект управляющий потоками ввода и вывода</param>
        /// <param name="cults">Список с данными об элементах</param>
        /// <param name="mode">Режим работы метода</param>
        public static void InputData(ref Streams streams, ref List<IJSONObject> cults, string mode)
        {
            string json1, json2;
            switch (mode)
            {
                case "upload":
                    json1 = JsonFromInput(ref streams);
                    if (!string.IsNullOrEmpty(json1)) cults = JsonParser.ReadJson(json1);
                    break;
                case "update":
                    json1 = JsonFromInput(ref streams);
                    List<IJSONObject> newCults;
                    if (!string.IsNullOrEmpty(json1))
                    {
                        newCults = JsonParser.ReadJson(json1);
                        foreach (IJSONObject newCult in newCults)
                        {
                            bool f = true;
                            foreach (IJSONObject cult in cults)
                            {
                                if (cult.GetField("id") == newCult.GetField("id"))
                                {
                                    f = false;
                                    foreach (string fieldName in cult.GetAllFields())
                                    {
                                        cult.SetField(fieldName, newCult.GetField(fieldName));
                                    }
                                    break;
                                }
                            }
                            if (f) cults.Add(newCult);
                        }
                    }
                    break;
                case "merge":
                    ShowMessage("Файл 1:");
                    json1 = JsonFromInput(ref streams);
                    ShowMessage("Файл 2:");
                    json2 = JsonFromInput(ref streams);
                    if (!string.IsNullOrEmpty(json1) && !string.IsNullOrEmpty(json2))
                    {
                        List<IJSONObject> cults2;
                        cults = JsonParser.ReadJson(json1);
                        cults2 = JsonParser.ReadJson(json2);
                        foreach (IJSONObject cult2 in cults2)
                        {
                            bool f = false;
                            foreach (IJSONObject cult in cults)
                            {
                                if (cult.GetField("id") == cult2.GetField("id"))
                                {
                                    f = true;
                                    ShowMessage($"Выберите файл, из которого взять данные о культе {cult.GetField("id")}:");
                                    ShowInfo("1. Файл 1");
                                    ShowInfo("Другая клавиша. Файл 2");
                                    ConsoleKeyInfo fileKey = Console.ReadKey(true);
                                    switch (fileKey.Key)
                                    {
                                        case ConsoleKey.D1:
                                            break;
                                        default:
                                            foreach (string fieldName in cult.GetAllFields())
                                            {
                                                cult.SetField(fieldName, cult2.GetField(fieldName));
                                            }
                                            break;
                                    }
                                }
                            }
                            if (!f) cults.Add(cult2);
                        }
                    }
                    if (json1 == null || json2 == null)
                    {
                        ShowError("Один из файлов некорректен");
                    }
                    break;
                default:
                    ShowError("Такое действие не предусмотренно программой");
                    break;
            }
        }

        /// <summary>
        /// Метод для представления вводимых данных формата json одной строкой
        /// </summary>
        /// <param name="streams">Объект управляющий потоками ввода и вывода</param>
        /// <returns>Строка с данными в формате json</returns>
        private static string JsonFromInput(ref Streams streams)
        {
            ShowMessage("Выберите как ввести данные:");
            ShowInfo("1. Ввести путь до файла");
            ShowInfo("2. Вставить содержимое файла в консоль");
            ShowInfo("Другая клавиша. Отмена");
            ConsoleKeyInfo keyInput = Console.ReadKey(true);
            string line, json = string.Empty;
            switch (keyInput.Key)
            {
                case ConsoleKey.D1:
                    ShowMessage("Введите путь до файла:");
                    string path = Console.ReadLine() ?? "";
                    if (!streams.StreamReadStart(path))
                    {
                        ShowError("Не получилось считать данные.");
                        return null;
                    }
                    break;
                case ConsoleKey.D2:
                    ShowMessage("Введите содрежимое файла в консоль, закончив ввод пустой строкой:");
                    break;
                default:
                    return "";
            }
            line = Console.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                json += line + '\n';
                line = Console.ReadLine();
            }
            if (keyInput.Key == ConsoleKey.D1) streams.StreamReadEnd();
            ShowMessage("Данные считаны");
            return json;
        }

        /// <summary>
        /// Метод для фильтрации данных
        /// </summary>
        /// <param name="cults">Список с данными об элементах</param>
        public static void Filter(ref List<IJSONObject> cults)
        {
            ShowMessage("Список полей для фильтрации, выберите одно из списка ниже:");
            int filterIndex = 0;
            foreach (string fieldName in new Cult().GetAllFields())
            {
                ShowInfo($"{filterIndex + 1}. {fieldName}");
                filterIndex++;
                if (filterIndex > 3) break;
            }
            ShowInfo("Другая клавиша. Отмена");
            ConsoleKeyInfo keyFilter = Console.ReadKey(true);
            List<string> filterWords = new List<string>();
            string word, field = string.Empty;
            switch (keyFilter.Key)
            {
                case ConsoleKey.D1:
                    field = "id";
                    break;
                case ConsoleKey.D2:
                    field = "label";
                    break;
                case ConsoleKey.D3:
                    field = "description";
                    break;
                case ConsoleKey.D4:
                    field = "icon";
                    break;
            }
            if (!string.IsNullOrEmpty(field))
            {
                ShowMessage($"Введите множество значений поля {field} для фильтрации (в формате json), пустая строка - окончание ввода:");
                word = Console.ReadLine();
                while (!string.IsNullOrEmpty(word))
                {
                    filterWords.Add(word);
                    word = Console.ReadLine();
                }
                Filtrator filter = new Filtrator(field, filterWords);
                filter.FilterCults(ref cults);
                ShowMessage("Данные отфильтрованы");
            }
        }

        /// <summary>
        /// Методя для сортировки данных
        /// </summary>
        /// <param name="cults">Список с данными об элементах</param>
        public static void Sort(ref List<IJSONObject> cults)
        {
            ShowMessage("Список полей для сортировки, выберите одно из списка ниже:");
            int sortIndex = 0;
            foreach (string fieldName in new Cult().GetAllFields())
            {
                ShowInfo($"{sortIndex + 1}. {fieldName}");
                sortIndex++;
                if (sortIndex > 3) break;
            }
            ShowInfo("Другая клавиша. Отмена");
            ConsoleKeyInfo keySort = Console.ReadKey(true);
            string field = string.Empty;
            switch (keySort.Key)
            {
                case ConsoleKey.D1:
                    field = "id";
                    break;
                case ConsoleKey.D2:
                    field = "label";
                    break;
                case ConsoleKey.D3:
                    field = "description";
                    break;
                case ConsoleKey.D4:
                    field = "icon";
                    break;
            }
            if (!string.IsNullOrEmpty(field))
            {
                ShowMessage($"Выберите направление сортировки:");
                ShowInfo($"1. От A до Z");
                ShowInfo($"2. От Z до A");
                ShowInfo("Другая клавиша. Отмена");
                ConsoleKeyInfo keySortDirection = Console.ReadKey(true);
                Sorter sorter = null;
                switch (keySortDirection.Key)
                {
                    case ConsoleKey.D1:
                        sorter = new Sorter(field, true);
                        break;
                    case ConsoleKey.D2:
                        sorter = new Sorter(field, false);
                        break;
                }
                if (sorter != null)
                {
                    sorter.Sort(ref cults);
                    ShowMessage("Данные отсортированы");
                }
            }
        }

        /// <summary>
        /// Метод для изменения данных
        /// </summary>
        /// <param name="cults">Список с данными об элементах</param>
        public static void Edit(ref List<IJSONObject> cults)
        {
            ShowMessage("Введите id элемента (в формате json), который вы хотите редактировать:");
            string id = Console.ReadLine() ?? "";
            bool f = false;
            EditElement(ref cults, id, ref f);
            if (!f)
            {
                ShowMessage("Элемента с таким id нет, создаем новый");
                Cult cult = new Cult();
                cult.SetField("id", id);
                cults.Add(cult);
                EditElement(ref cults, id, ref f);
                ShowMessage("Элемент создан");
            }
            else
            {
                ShowMessage("Элемент отредактирован");
            }
        }

        /// <summary>
        /// Метод для удаления данных
        /// </summary>
        /// <param name="cults">Список с данными об элементах</param>
        public static void Delete(ref List<IJSONObject> cults)
        {
            ShowMessage("Введите id элемента (в формате json), который вы хотите удалить:");
            string id = Console.ReadLine() ?? "";
            bool f = false;
            foreach (IJSONObject cult in cults)
            {
                if (cult.GetField("id") == id)
                {
                    cults.Remove(cult);
                    ShowMessage("Элемент удален");
                    f = true;
                }
            }
            if (!f)
            {
                ShowError("Элемент с таким id не найден");
            }
        }

        /// <summary>
        /// Метод редактирующий элемент с заданными id
        /// </summary>
        /// <param name="cults">Список с данными об элементах</param>
        /// <param name="id">Id элемента</param>
        /// <param name="f">Успешно ли изменение</param>
        public static void EditElement(ref List<IJSONObject> cults, string id, ref bool f)
        {
            foreach (IJSONObject cult in cults)
            {
                if (cult.GetField("id") == id)
                {
                    ShowMessage("Введите значения остальных полей объекта (в формате json):");
                    foreach (string fieldName in cult.GetAllFields())
                    {
                        if (fieldName != "id")
                        {
                            ShowInfo(fieldName + ':');
                            try
                            {
                                cult.SetField(fieldName, Console.ReadLine() ?? "");
                            }
                            catch (KeyNotFoundException e)
                            {
                                ShowInfo(e.Message);
                            }
                        }
                    }
                    f = true;
                }
            }
        }

        /// <summary>
        /// Метод, отображающий меню основной задачи
        /// </summary>
        /// <param name="streams">Объект управляющий потоками ввода и вывода</param>
        /// <param name="cults">Список с данными об элементах</param>
        public static void BaseTask(ref Streams streams, ref List<IJSONObject> cults)
        {
            ShowMessage("Выберите действие:");
            ShowInfo("1. Дозагрузка данных");
            ShowInfo("2. Редактирование данных");
            ShowInfo("3. Удаление данных");
            ShowInfo("4. Слияние данных");
            ShowInfo("Другая клавиша. Выход в основное меню");
            ConsoleKeyInfo keyBaseTask = Console.ReadKey(true);
            switch (keyBaseTask.Key)
            {
                case ConsoleKey.D1:
                    InputData(ref streams, ref cults, "update");
                    break;
                case ConsoleKey.D2:
                    if (cults.Count == 0)
                    {
                        ShowError("Данные еще не введены");
                    }
                    else
                    {
                        Edit(ref cults);
                    }
                    break;
                case ConsoleKey.D3:
                    if (cults.Count == 0)
                    {
                        ShowError("Данные еще не введены");
                    }
                    else
                    {
                        Delete(ref cults);
                    }
                    break;
                case ConsoleKey.D4:
                    InputData(ref streams, ref cults, "merge");
                    break;
            }
        }

        /// <summary>
        /// Метод для обеспечения вывода данных
        /// </summary>
        /// <param name="streams">Объект управляющий потоками ввода и вывода</param>
        /// <param name="cults">Список с данными об элементах</param>
        public static void OutputData(ref Streams streams, ref List<IJSONObject> cults)
        {
            ShowMessage("Выберите как вывести данные:");
            ShowInfo("1. Ввести путь до файла");
            ShowInfo("2. Вывести данные в консоль");
            ShowInfo("Другая клавиша. Отмена");
            ConsoleKeyInfo keyOutput = Console.ReadKey(true);
            if (keyOutput.Key == ConsoleKey.D1)
            {
                ShowMessage("Введите путь до файла:");
                string path = Console.ReadLine();
                if (streams.StreamWriteStart(path))
                {
                    Console.WriteLine(JsonParser.WriteJson(cults));
                    streams.StreamWriteEnd();
                    ShowMessage("Данные выведены в файл");
                }
                else
                {
                    ShowError("Не получилось вывести данные в файл.");
                }
            }
            else if (keyOutput.Key == ConsoleKey.D2)
            {
                ShowMessage("Данные:");
                Console.WriteLine(JsonParser.WriteJson(cults));
            }
        }
    }
}