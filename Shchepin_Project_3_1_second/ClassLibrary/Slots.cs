using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ClassLibrary
{
    internal class Slots : IJSONObject
    {
        private List<Slot> ListOfSlots { get; set; } = new List<Slot>();
        public IEnumerable<string> GetAllFields()
        {
            return new List<string>() { "listOfSlots" };
        }
        public string GetField(string fieldName)
        {
            switch (fieldName)
            {
                case "listOfSlots":
                    string list = "[\n";
                    foreach (Slot slot in ListOfSlots)
                    {
                        list += "{\n" + slot.GetField(fieldName) + "\n},";
                    }
                    list.Substring(0, list.Length - 1);
                    list += "\n]";
                    return list;
                default: return null;
            }
        }
        public void SetField(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "listOfSlots":
                    ParseSlots(value);
                    break;
                default:
                    throw new KeyNotFoundException($"Поле \"{fieldName}\" не найдено.");
            }
        }

        /// <summary>
        /// Метод для парса значения поля slots из формата json
        /// </summary>
        /// <param name="notParsedSlots">значение поля slot в формате json</param>
        /// <exception cref="InvalidDataException">Ошибка при некорректно введенных данных</exception>
        public void ParseSlots(string notParsedSlots)
        {
            Regex re = new Regex("\\{\\s*(\"[\\s\\S]*?\")\\s*:\\s*((\"[\\s\\S]*?\")|(\\{((\"[\\s\\S]*?\")\\s*:\\s*(\\d*?)\\s*,?\\s*)?\\}))\\s*\\}");
            MatchCollection matches = re.Matches(notParsedSlots);
            if (matches.Count == 0 && notParsedSlots.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "") != "[]")
            {
                throw new InvalidDataException("Введены некорректные данные");
            }
            List<Slot> slots = new List<Slot>();
            foreach (Match match in matches)
            {
                Slot slot = new Slot();
                slot.ParseSlot(match.Value);
                slots.Add(slot);
            }
            ListOfSlots = slots;
        }
        public string ToJson()
        {
            string result = "[";
            foreach (Slot slot in ListOfSlots)
            {
                result += slot.ToJson() + ", ";
            }
            result = (result.Length > 1 ? result.Substring(0, result.Length - 2) : result) + "\n]";
            return result;
        }
    }
}
