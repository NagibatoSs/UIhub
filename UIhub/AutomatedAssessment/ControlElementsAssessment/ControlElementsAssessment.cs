using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using System;
using System.IO;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace UIhub.AutomatedAssessment.ControlElementsAssessment
{
    [Serializable]
    class ControlElementsData
    {
        public string[] Tags;
        public double Koef;
        public int Etalon;
    }
    public class ControlElementsAssessment: Assessment
    {
        private int elementsCount;
        int maxEtalon;
        ControlElementsData data;
        public override Tuple<double,string> DoAssessment(string xamlText)
        {
            try
            {
                this.xamlText = xamlText;
                GetJsonDataAsync().Wait();
                CountElements();
                RateMessage = "Оценка по количеству управляющих элементов: ";
                if (elementsCount > maxEtalon)
                {
                    CalculateRate();
                    RateMessage += Rate + ". Cлишком много элементов - " + elementsCount;
                }
                else
                {
                    Rate = 10;
                    RateMessage += Rate + ". Элементов самый раз - " + elementsCount;
                }
                return Tuple.Create(Rate, RateMessage);
            }

            catch
            {
                throw new Exception("Ошибка в оценке по количеству управляющих элементов");
            }
        }

        private void CountElements()
        {
            string[] tags = data.Tags;
            elementsCount = 0;
            foreach (var tag in tags)
                elementsCount += Regex.Matches(xamlText, "<" + tag).Count;
        }
        private void CalculateRate()
        {
            var percent = elementsCount * 100 / data.Etalon;
            var minusPoints = Math.Abs(100 - percent) * data.Koef;
            Rate = Math.Round(10 - minusPoints,1);
        }
        private async Task GetJsonDataAsync()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\AutomatedAssessment\ControlElementsAssessment\ControlElementsAssessment.json";
            string jsonString = File.ReadAllText(fileName);
            data = JsonConvert.DeserializeObject<ControlElementsData>(jsonString);
            maxEtalon = data.Etalon + 2;
        }
    }
}
