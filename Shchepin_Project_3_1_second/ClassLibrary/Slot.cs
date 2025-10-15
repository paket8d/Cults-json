using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClassLibrary
{
    public class Slot : IJSONObject
    {
        private string Id { get; set; } = "No id";
        private string Label { get; set; } = "No label";
        private string Description { get; set; } = "No description";
        private string ActionID { get; set; } = "No action ID";
        private Dictionary<string, int> Required { get; set; } = new Dictionary<string, int>();
        private Dictionary<string, int> Forbidden { get; set; } = new Dictionary<string, int>();


        public IEnumerable<string> GetAllFields()
        {
            return new List<string>() { "id", "label", "description", "actionid", "required", "forbidden" };
        }
        public string GetField(string fieldName)
        {
            switch (fieldName)
            {
                case "id": return $"\"{Id}\"";
                case "label": return $"\"{Label}\"";
                case "description": return $"\"{Description}\"";
                case "actionid": return $"\"{ActionID}\"";
                case "required": return JsonParser.DictStrIntToJson(Required); // строки нулевой длины
                case "forbidden": return JsonParser.DictStrIntToJson(Forbidden);
                default: return null;
            }
        }
        public void SetField(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "id": Id = value.Trim('"'); break;
                case "label": Label = value.Trim('"'); break;
                case "description": Description = value.Trim('"'); break;
                case "actionid": ActionID = value.Trim('"'); break;
                case "required": Required = JsonParser.ParseDictStrInt(value); break;
                case "forbidden": Forbidden = JsonParser.ParseDictStrInt(value); break;
                default:
                    throw new KeyNotFoundException($"Field '{fieldName}' not found.");
            }
        }

        /// <summary>
        /// Отдельный метод для удобного представления объекта в формате json
        /// </summary>
        /// <returns>Объект в формате json</returns>
        public string ToJson()
        {
            string result = "\n\t{";
            foreach (string field in GetAllFields())
            {
                result += $"\n\t\t\"{field}\": {GetField(field)}, ";
            }
            result = result.Length > 1 ? result.Substring(0, result.Length - 2) : result;
            result += "\n\t}";
            return result;
        }

        /// <summary>
        /// Отдельный метод для удобного парса объекта из формата json
        /// </summary>
        /// <param name="notParsedSlot">Объект в формате json</param>
        /// <exception cref="InvalidDataException">Некорректно введенный объект в формате json</exception>
        public void ParseSlot(string notParsedSlot)
        {
            Regex re = new Regex("(\"[\\s\\S]*?\")\\s*:\\s*(\"[\\s\\S]*?\"|\\{[\\s\\S]*?\\})");
            MatchCollection matches = re.Matches(notParsedSlot);

            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                if (Regex.IsMatch(value, "^\"[\\s\\S]*?\"$"))
                {
                    switch (key.Trim('"'))
                    {
                        case "id":
                            SetField("id", value);
                            break;
                        case "label":
                            SetField("label", value);
                            break;
                        case "description":
                            SetField("description", value);
                            break;
                        case "actionid":
                            SetField("actionid", value);
                            break;
                    }
                }
                else
                {
                    switch (key.Trim('"'))
                    {
                        case "required":
                            SetField("required", value);
                            break;
                        case "forbidden":
                            SetField("forbidden", value);
                            break;
                    }
                }
            }
            if (matches.Count == 0)
            {
                throw new InvalidDataException("Введены некорректные данные");
            }
        }
    }
}