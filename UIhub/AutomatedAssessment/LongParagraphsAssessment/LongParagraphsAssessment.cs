using Azure;
using Newtonsoft.Json;
using UIhub.AutomatedAssessment.ControlElementsAssessment;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UIhub.AutomatedAssessment.LongParagraphsAssessment
{
    [Serializable]
    class LongParagraphsData
    {
        public string[] Tags;
        public double MinusPerParagraph;
        public int WordsInParagraph;
    }
    public class LongParagraphsAssessment:Assessment
    {
        private LongParagraphsData data;
        private string[] lines;
        private int parCount = 0;
        
        public override Tuple<double,string> DoAssessment(string xamlText)
        {
            try
            {
                GetJsonDataAsync().Wait();
                Rate = 10;

                char[] chars = { '\r', '\n' };
                lines = xamlText.Split(chars);
                lines = lines.Where((element, index) => index % 2 == 0).ToArray();
                var tags = data.Tags;
                foreach (var tag in tags)
                {
                    if (tag == "TextBlock" || tag == "TextBox")
                        AnalysTag(tag, "Text");
                    if (tag == "Label")
                        AnalysTag(tag, "Content");
                }
                return Tuple.Create(Rate, "Оценка на количество длинных абзацев: "+ Rate +". Всего " + parCount + " длинных абзацев. " + RateMessage);
            }

            catch
            {
                throw new Exception("Ошибка в оценке по количеству длинных абзацев");
            }
        }
        private async Task GetJsonDataAsync()
        {
            string fileName = Directory.GetCurrentDirectory() + @"\AutomatedAssessment\LongParagraphsAssessment\LongParagraphsAssessment.json";
            string jsonString = File.ReadAllText(fileName);
            data = JsonConvert.DeserializeObject<LongParagraphsData>(jsonString);
        }

        private void AnalysTag(string tag, string contentTag)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("<" + tag))
                {
                    if (lines[i].Contains("/>") || lines[i].Contains("</" + tag))
                        AnalysTagOnOneLine(tag, i, contentTag);
                    else AnalysTagMultiLines(tag, i,contentTag);
                }
            }
        }
        private void AnalysTagOnOneLine(string tag, int i, string contentTag)
        {
            int textEndIndex;
            var textStartIndex = lines[i].IndexOf(contentTag + "=");
            if (textStartIndex >= 0)
            {
                textStartIndex += 6; // Длина "Text=" + 1
                var secondPartOfStr = lines[i].Substring(textStartIndex);
                textEndIndex = secondPartOfStr.IndexOf("\"");
                if (textEndIndex > 0)
                    textEndIndex += textStartIndex;
            }
            //Если нет атрибута текст
            else
            {
                textEndIndex = 0;
                if (tag == "Label")
                    textEndIndex = lines[i].IndexOf("</Label");
                if (tag.Contains("Text"))
                    textEndIndex = lines[i].IndexOf("</Text");
                textStartIndex = lines[i].IndexOf(">") + 1;
            }
            if (!(lines[i].Contains(contentTag +"=")) && textEndIndex<0)
            {
                return;
            }
            var paragraph = lines[i].Substring(textStartIndex, textEndIndex - textStartIndex);
            var count = paragraph.Split(' ').Count();
            if (count >= data.WordsInParagraph)
            {
                parCount++;
                RateMessage += "Слишком длинный абзац :" + paragraph.Substring(0, 15) + "... ";
                Rate -= data.MinusPerParagraph;
            }
        }
        private void AnalysTagMultiLines(string tag, int i, string contentTag)
        {
            string paragraph = "";
            int textEndIndex;
            for (int j = i; j < lines.Length; j++)
            {
                var textStartIndex = lines[j].IndexOf(contentTag+"=");
                if (textStartIndex >= 0)
                {
                    textStartIndex += 6;
                    var text = lines[j].Substring(textStartIndex);
                    textEndIndex = text.IndexOf("\"");
                    for (int k = j; k < lines.Length; k++)
                    {
                        if (textEndIndex > 0)
                        {
                            paragraph += lines[k].Substring(0, textEndIndex);
                            var count = paragraph.Split(' ').Count();
                            if (count >= data.WordsInParagraph)
                            {
                                parCount++;
                                RateMessage += "Слишком длинный абзац :" + paragraph.Substring(0, 15) + "... ";
                                Rate -= data.MinusPerParagraph;
                                break;
                            }
                            break;
                        }
                        else
                        {
                            paragraph += text;
                        }
                        textEndIndex = lines[k].IndexOf("\"");
                    }
                    break;
                }

                else if (lines[j].IndexOf(">") > 0)
                {
                    paragraph += lines[j].Substring(lines[j].IndexOf(">")+1);
                    for (var k = j+1; k < lines.Length; k++)
                    {
                        textEndIndex = 0;
                        if (tag == "Label")
                            textEndIndex = lines[k].IndexOf("</Label");
                        if (tag.Contains("Text"))
                            textEndIndex = lines[k].IndexOf("</Text");
                        if (textEndIndex > 0)
                        {
                            paragraph += lines[k].Substring(0, textEndIndex);
                            paragraph = paragraph.Trim();
                            var count = paragraph.Split(' ').Count();
                            if (count >= data.WordsInParagraph)
                            {
                                parCount++;
                                RateMessage += "Слишком длинный абзац :" + paragraph.Substring(0, 15) + "... ";
                                Rate -= data.MinusPerParagraph;
                            }
                            break;
                        }
                        else paragraph += lines[k];
                    }
                    break;
                }
                   
            }
        }
    }
}

