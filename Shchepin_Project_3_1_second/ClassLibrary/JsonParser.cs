using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ClassLibrary
{
    public static class JsonParser
    {
        /// <summary>
        /// Перечислитель для состояний кончечного автомата
        /// </summary>
        public enum State
        {
            Program, // общее
            Elements, // считываем массив elements
            Object, // считываем один из элементов
            Key, // считываем ключ элемента
            Value, // считываем значение элемента
            ValueStr, // считываем значение элемента типа string
            ValueArr, // считываем значение элемента типа array
            ValueDict, // считываем значение элемента типа dictionary
            ValueNullOrBool // // считываем значение элемента типа null || bool
        }
        /// <summary>
        /// Метод, считывающий данные из json файла. Реализован при помощи конечного автомата
        /// </summary>
        /// <param name="json">Строка с данными в формате json</param>
        /// <returns>Массив элементов</returns>
        public static List<IJSONObject> ReadJson(string json)
        {
            List<IJSONObject> cults = new List<IJSONObject>();
            Cult cult = new Cult();

            State state = State.Program;
            string key = string.Empty;
            string value = string.Empty;
            // параметры, необходимые для определение конца значения вложенного объекта
            int squareBrackets = 0;
            int figureBrackets = 0;

            try
            {
                foreach (char symbol in json ?? "")
                {
                    switch (state)
                    {
                        case State.Program when symbol == '[':
                            state = State.Elements;
                            break;
                        case State.Program:
                            break;
                        case State.Elements when symbol == '{': // создать объект
                            state = State.Object;
                            cult = new Cult();
                            break;
                        case State.Elements when symbol == ']': // создать объект
                            state = State.Program;
                            break;
                        case State.Object when symbol == '"':
                            state = State.Key;
                            break;
                        case State.Object when symbol == ':':
                            state = State.Value;
                            break;
                        case State.Object when symbol == '}':
                            cults.Add(cult);
                            state = State.Elements;
                            break;
                        case State.Key when symbol != '"':
                            key += char.ToLower(symbol);
                            break;
                        case State.Key when symbol == '"':
                            state = State.Object;
                            break;
                        case State.Value when value == string.Empty && symbol == '"':
                            value += symbol;
                            state = State.ValueStr;
                            break;
                        case State.ValueStr when symbol != '"':
                            value += symbol;
                            break;
                        case State.ValueStr when symbol == '"':
                            value += symbol;
                            cult.SetField(key, value);
                            state = State.Object;
                            key = string.Empty;
                            value = string.Empty;
                            break;
                        case State.Value when value == string.Empty && symbol == '[':
                            value += symbol;
                            state = State.ValueArr;
                            squareBrackets++;
                            break;
                        case State.ValueArr when symbol == '[':
                            value += symbol;
                            squareBrackets++;
                            break;
                        case State.ValueArr when symbol == ']':
                            value += symbol;
                            squareBrackets--;
                            if (squareBrackets == 0)
                            {
                                cult.SetField(key, value);
                                key = string.Empty;
                                value = string.Empty;
                                state = State.Object;
                            }
                            break;
                        case State.ValueArr:
                            value += symbol;
                            break;
                        case State.Value when value == string.Empty && symbol == '{':
                            value += symbol;
                            state = State.ValueDict;
                            figureBrackets++;
                            break;
                        case State.ValueDict when symbol == '{':
                            value += symbol;
                            figureBrackets++;
                            break;
                        case State.ValueDict when symbol == '}':
                            value += symbol;
                            figureBrackets--;
                            if (figureBrackets == 0)
                            {
                                cult.SetField(key, value);
                                key = string.Empty;
                                value = string.Empty;
                                state = State.Object;
                            }
                            break;
                        case State.ValueDict:
                            value += symbol;
                            break;
                        case State.Value when value == string.Empty && (symbol == 'f' || symbol == 't' || symbol == 'n'):
                            value += symbol;
                            state = State.ValueNullOrBool;
                            break;
                        case State.ValueNullOrBool when value == "null" || value == "false" || value == "true":
                            cult.SetField(key, value);
                            key = string.Empty;
                            value = string.Empty;
                            state = State.Object;
                            break;
                        case State.ValueNullOrBool:
                            value += symbol;
                            break;
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                Menu.ShowError(e.Message);
            }
            return cults;
        }
        /// <summary>
        /// Метод для представления данных об элементах в формате json
        /// </summary>
        /// <param name="cults">Список элементов</param>
        /// <returns>Строка с данными в формате json</returns>
        public static string WriteJson(List<IJSONObject> cults)
        {
            string json = "{\n\t\"elements\": [\n";
            foreach (IJSONObject c in cults)
            {
                json += "\t\t{\n";
                foreach (string field in c.GetAllFields())
                {
                    string value = c.GetField(field);
                    if ((field == "slot" && c.GetField("slots") != "[]") || (field == "slots" && c.GetField("slots") == "[]"))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(value))
                    {
                        json += $"\t\t\t\"{field}\": {value.Replace("\n", "\n\t\t\t")},\n";
                    }
                }
                if (json.EndsWith(",\n"))
                {
                    json = json.Substring(0, json.Length - 2);
                    json += '\n';
                }
                json += "\t\t},\n";
            }
            if (json.EndsWith(",\n"))
            {
                json = json.Substring(0, json.Length - 2);
                json += '\n';
            }
            json += "\t]\n}";
            return json;
        }
        /// <summary>
        /// Метод для парса вложенного словаря (string: int) из формата json
        /// </summary>
        /// <param name="notParsedDict">Словарь в формате json</param>
        /// <returns>Проебразованный словарь</returns>
        /// <exception cref="InvalidDataException">Некорректное значение одного из элементов словаря</exception>
        internal static Dictionary<string, int> ParseDictStrInt(string notParsedDict)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            Regex re = new Regex("(\"[\\s\\S]*?\")\\s*:\\s*(\\d*)");
            MatchCollection matches = re.Matches(notParsedDict);

            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                try
                {
                    dict.Add(key.Trim('"'), int.Parse(value));
                }
                catch (FormatException ex)
                {
                    throw new InvalidDataException("Введены некорректные данные");
                }
            }
            if (matches.Count == 0 && notParsedDict.Replace(" ", "") != "{}")
            {
                throw new InvalidDataException("Введены некорректные данные");
            }
            return dict;
        }
        /// <summary>
        /// Метод для преобразования словаря (string: int) в формат json
        /// </summary>
        /// <param name="dict">Словарь с данными</param>
        /// <returns>Строка в формате json</returns>
        internal static string DictStrIntToJson(Dictionary<string, int> dict)
        {
            string result = "{";
            foreach (KeyValuePair<string, int> kvp in dict)
            {
                result += $"\"{kvp.Key}\": {kvp.Value}, ";
            }
            result = result.Length > 1 ? result.Substring(0, result.Length - 2) : result;
            result += "}";
            return result;
        }
        /// <summary>
        /// Проверка строки на соответствие формату json
        /// </summary>
        /// <param name="str">Строка с данными</param>
        /// <returns>Соответствует ли формату</returns>
        public static bool IsJsonString(string str)
        {
            return (str[0] == '"' && str[^1] == '"'); 
        }
    }
}