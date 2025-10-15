using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ClassLibrary
{
    internal class Aspects : IJSONObject
    {
        private Dictionary<string, int> DictOfAspects { get; set; } = new Dictionary<string, int>();
        public IEnumerable<string> GetAllFields()
        {
            return new List<string>() { "dictOfAspects" };
        }
        public string GetField(string fieldName)
        {
            switch (fieldName)
            {
                case "dictOfAspects": return JsonParser.DictStrIntToJson(DictOfAspects);
                default: return null;
            }
        }
        public void SetField(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "dictOfAspects": DictOfAspects = JsonParser.ParseDictStrInt(value); break; // поймать исключение и сообщить пользователю
                default:
                    throw new KeyNotFoundException($"Field '{fieldName}' not found.");
            }
        }
        public string ToJson()
        {
            return GetField("dictOfAspects");
        }
    }
}
