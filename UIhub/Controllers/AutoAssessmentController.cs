using Microsoft.AspNetCore.Mvc;
using UIhub.AutomatedAssessment.ControlElementsAssessment;
using UIhub.AutomatedAssessment;
using System.Text;
using UIhub.AutomatedAssessment.LongParagraphsAssessment;

namespace UIhub.Controllers
{
    public class AutoAssessmentController: Controller
    {
        public AutoAssessmentController()
        {
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<Tuple<double, string>> AssessFilesAsync(List<IFormFile> uploadedFiles)
        {
            double resultRate = 0;
            StringBuilder rateMessage = new StringBuilder();
            foreach (var uploadedFile in uploadedFiles)
            {
                rateMessage.Append(uploadedFile.FileName + " ");
                string fileContents = await GetFileText(uploadedFile);
                var assessmentNames = GetAssessmentNames();
                var assessment = GetAssessmentResult(fileContents, assessmentNames);
                rateMessage.Append(assessment.Item2);
                resultRate += assessment.Item1;
            }
            resultRate /= uploadedFiles.Count;
            rateMessage.Append("Результирующий балл = " + Math.Round(resultRate, 1));
            return Tuple.Create(resultRate, rateMessage.ToString());
        }
        public async Task<string> GetFileText(IFormFile uploadedFile)
        {
            string fileContents;
            using (var stream = uploadedFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                fileContents = await reader.ReadToEndAsync();
            }
            return fileContents;
        }
        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> UploadedFiles)
        {
            double resultRate = 0;
            foreach (var uploadedFile in UploadedFiles)
            {
                ViewBag.Message+=uploadedFile.FileName + " ";
                string fileContents = await GetFileText(uploadedFile);
                var assessmentNames = GetAssessmentNames();
                var assessment = GetAssessmentResult(fileContents, assessmentNames);
                ViewBag.Message += assessment.Item2;
                resultRate += assessment.Item1;
            }
            resultRate /= UploadedFiles.Count;
            ViewBag.Message += "Результирующий балл = " + Math.Round(resultRate, 1);
            return View();
        }
        private List<string> GetAssessmentNames()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\AutomatedAssessment");
            return directoryInfo.GetDirectories().Select(d => d.Name).ToList();
        }
        private Tuple<double, string> GetAssessmentResult(string fileContents, List<string> assessmentNames)
        {
            double rate = 0;
            StringBuilder assessmentRes = new StringBuilder();
            var successAssesmentCount = 0;
            for (int i = 0; i < assessmentNames.Count; i++)
            {
                try
                {
                    var type = Type.GetType("UIhub.AutomatedAssessment." + assessmentNames[i] + "." + assessmentNames[i]);
                    var ctor = type.GetConstructor(new Type[] { });
                    var result = ctor.Invoke(new object[] { });
                    var assessment = (Tuple<double, string>)type.GetMethod("DoAssessment").Invoke(result, new object[] { fileContents });
                    assessmentRes.Append(assessment.Item2);/* + " Оценка по критерию: " + assessment.Item1);*/
                    assessmentRes.AppendLine("\n");
                    rate += assessment.Item1;
                    successAssesmentCount++;
                }
                catch
                { }
            }
            return Tuple.Create(rate/successAssesmentCount, assessmentRes.ToString());
        }
    }
}
