using System;
namespace ClassLibrary
{
    /// <summary>
    /// Класс, объект которого занимается фильтрацией элементов
    /// </summary>
    public class Filtrator
    {
        string _fieldName = "id";
        List<string> _filterWords = new List<string>();
        public Filtrator(string fieldName, List<string> filterWords)
        {
            _fieldName = fieldName;
            _filterWords = filterWords;
        }
        /// <summary>
        /// Метод, фильтрующий список элементов
        /// </summary>
        /// <param name="cults">список элементов</param>
        public void FilterCults(ref List<IJSONObject> cults)
        {
            List<IJSONObject> filteredCults = new List<IJSONObject>();
            foreach (Cult cult in cults)
            {
                foreach (string word in _filterWords)
                {
                    if (word == cult.GetField(_fieldName)) filteredCults.Add(cult);
                }
            }
            cults = filteredCults;
        }
    }
}
