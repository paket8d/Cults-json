using System;
using System.Collections.Generic;

namespace ClassLibrary {
    public struct Cult : IJSONObject
    {
        private string Id { get; set; } = "\"No id\"";
        private string Label { get; set; } = "\"No label\"";
        private string Description { get; set; } = "\"No description\"";
        private string Icon { get; set; } = "\"No icon\"";
        private Aspects Aspects { get; set; } = new Aspects();
        private Slots Slots { get; set; } = new Slots();
        private bool Unique { get; set; } = false;  

        public IEnumerable<string> GetAllFields()
        {
            return new List<string> { "id", "label", "description", "icon", "aspects", "slots", "unique" };
        }

        public string GetField(string fieldName)
        {
            switch (fieldName)
            {
                case "id": return Id;
                case "label": return Label;
                case "description": return Description;
                case "icon": return Icon;
                case "aspects": return Aspects.ToJson();
                case "slots": return Slots.ToJson();
                case "unique": return Unique.ToString().ToLower();
                default: return null;
            }
        }

        public void SetField(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "id": 
                    if (JsonParser.IsJsonString(value)) 
                        Id = value;
                    else
                    {
                        Menu.ShowError("Введены некорректные данные");
                        Menu.ShowError("Значение id не изменилось");
                    }
                    break;
                case "label":
                    if (JsonParser.IsJsonString(value))
                        Label = value;
                    else
                    {
                        Menu.ShowError("Введены некорректные данные");
                        Menu.ShowError("Значение label не изменилось");
                    }
                    break;
                case "description":
                    if (JsonParser.IsJsonString(value))
                        Description = value;
                    else
                    {
                        Menu.ShowError("Введены некорректные данные");
                        Menu.ShowError("Значение description не изменилось");
                    }
                    break;
                case "icon":
                    if (JsonParser.IsJsonString(value))
                        Icon = value;
                    else
                    {
                        Menu.ShowError("Введены некорректные данные");
                        Menu.ShowError("Значение icon не изменилось");
                    }
                    break;
                case "aspects":
                    try
                    {
                        Aspects.SetField("dictOfAspects", value);
                    }
                    catch (InvalidDataException e)
                    {
                        Menu.ShowError(e.Message);
                        Menu.ShowError("Значение aspects не изменилось");
                    }
                    break;
                case "slots":
                    try
                    {
                        Slots.SetField("listOfSlots", value);
                    }
                    catch (InvalidDataException e)
                    {
                        Menu.ShowError(e.Message);
                        Menu.ShowError("Значение slots не изменилось");
                    }
                    break;
                case "unique":
                    if (value != "true" && value != "false")
                    {
                        Menu.ShowError("Введены некорректные данные");
                        Menu.ShowError("Значение unique не изменилось");
                    }
                    else
                        Unique = bool.Parse(value);
                    break;
                default:
                    throw new KeyNotFoundException($"Field '{fieldName}' not found.");
            }
        }
        
        public Cult()
        {

        }
    }
}