using Microsoft.AspNetCore.Mvc;
using UIhub.AutomatedAssessment.ControlElementsAssessment;
using UIhub.AutomatedAssessment;
using System.Text;
using UIhub.AutomatedAssessment.LongParagraphsAssessment;
using Microsoft.AspNetCore.Identity;
using UIhub.Models;
using UIhub.Data;
using System.Security.Claims;

namespace UIhub.Controllers
{
    public class AutoAssessmentController: Controller
    {
        UserManager<User> _userManager;
        IUser _userService;
        public AutoAssessmentController(UserManager<User> userManager)
        {
            _userManager = userManager;
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
                rateMessage.AppendLine("<b>"+uploadedFile.FileName + "</b></br>");
                string fileContents = await GetFileText(uploadedFile);
                var assessmentNames = GetAssessmentNames();
                var assessment = GetAssessmentResult(fileContents, assessmentNames);
                rateMessage.Append(assessment.Item2 + "</br>");
                resultRate += assessment.Item1;
            }
            resultRate /= uploadedFiles.Count;
            return Tuple.Create(Math.Round(resultRate,1), rateMessage.ToString());
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
            ViewBag.Message = "<h4>Результаты автоматической оценки</h4></br>";
            double resultRate = 0;
            foreach (var uploadedFile in UploadedFiles)
            {
                ViewBag.Message+=uploadedFile.FileName + "</br>";
                string fileContents = await GetFileText(uploadedFile);
                var assessmentNames = GetAssessmentNames();
                var assessment = GetAssessmentResult(fileContents, assessmentNames);
                ViewBag.Message += assessment.Item2 + "</br>";
                resultRate += assessment.Item1;
            }
            resultRate /= UploadedFiles.Count;
            ViewBag.Message += "<b>Результирующий балл = " + Math.Round(resultRate, 1)+"</b>";
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
                    assessmentRes.Append(assessment.Item2 + "<br/>");
                    rate += assessment.Item1;
                    successAssesmentCount++;
                }
                catch
                { }
            }
            return Tuple.Create(Math.Round(rate/successAssesmentCount,1), assessmentRes.ToString());
        }
        [HttpPost]
        public async Task<IActionResult> AddAssessmentFilesAsync(IFormFile csFile, IFormFile jsonFile)
        {

            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Message = "Необходима авторизация!";
                return View();
            }
            var user = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;
            var roles = _userManager.GetRolesAsync(user).Result;
            if (roles.Contains("admin"))
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory() + @"\AutomatedAssessment",
                    csFile.FileName.Substring(0,(csFile.FileName.Length)-3));
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                string fileName = Path.GetFileName(csFile.FileName);
                string fileSavePath = Path.Combine(uploadFolder, fileName);
                using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await csFile.CopyToAsync(stream);
                }
                fileName = Path.GetFileName(jsonFile.FileName);
                fileSavePath = Path.Combine(uploadFolder, fileName);
                using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await jsonFile.CopyToAsync(stream);
                }
                ViewBag.Message = "Файлы успешно добавлены!";
            }
            else
            {
                ViewBag.Message = "У вас нет прав на совершение данной операции";
            }
            return View();
        }
    }
}
