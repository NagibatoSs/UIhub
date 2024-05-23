using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UIhub.AutomatedAssessment.ControlElementsAssessment;

namespace UIhub.AutomatedAssessment.TipsAssessment
{
    [Serializable]
    class TipsData
    {
        public string[] Tags;
        public double Koef;
        public int Etalon;
    }
    public class TipsAssessment : Assessment
    {
        private int tipsCount;
        TipsData data;
        public override Tuple<double,string> DoAssessment(string xamlText)
        {
            try
            {
                this.xamlText = xamlText;
                GetJsonDataAsync().Wait();
                CountTips();
                RateMessage = "Оценка по наличию подсказок: ";
                if (tipsCount > data.Etalon)
                {
                    CalculateRate();
                    RateMessage += Rate +". Подсказок слишком много - " + tipsCount + ". Рекомендуемое максимальное количество - " + data.Etalon;
                }
                else if (tipsCount == 0)
                {
                    Rate = 0;
                    RateMessage += Rate +". Подсказки отсутствуют!";
                }
                else
                {
                    Rate = 10;
                    RateMessage += Rate +". Подсказок оптимальное количество - " + tipsCount;
                }

                return Tuple.Create(Rate, RateMessage);
            }
            catch
            {
                 throw new Exception("Ошибка в оценке на наличие подсказок");
            }
        }
        private void CountTips()
        {
            string[] tags = data.Tags;
            tipsCount = 0;
            foreach (var tag in tags)
            {
                tipsCount += Regex.Matches(xamlText, "<" + tag + "|" + tag +"=").Count;
            }
        }
        private void CalculateRate()
        {
            var percent = tipsCount * 100 / data.Etalon;
            var minusPoints = Math.Round(Math.Abs(100 - percent) * data.Koef, 1);
            Rate = 10 - minusPoints;
        }
        private async Task GetJsonDataAsync()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\AutomatedAssessment\TipsAssessment\TipsAssessment.json";
            string jsonString = File.ReadAllText(fileName);
            data = JsonConvert.DeserializeObject<TipsData>(jsonString);
        }
    }
}
