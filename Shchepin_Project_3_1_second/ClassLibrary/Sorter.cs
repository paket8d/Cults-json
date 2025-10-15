using System;
namespace ClassLibrary
{
    /// <summary>
    /// Класс, объект которого занимается сортировкой данных
    /// </summary>
    public class Sorter
    {
        string _fieldName = "id";
        bool _sortOrder = true;
        public Sorter(string fieldName, bool sortOrder)
        {
            _fieldName = fieldName;
            _sortOrder = sortOrder;
        }
        public void Sort(ref List<IJSONObject> cults)
        {
            if (_sortOrder)
            {
                cults.Sort((c1, c2) => c1.GetField(_fieldName).CompareTo(c2.GetField(_fieldName)));
            }
            else
            {
                cults.Sort((c1, c2) => -c1.GetField(_fieldName).CompareTo(c2.GetField(_fieldName)));
            }
        }
    }
}
