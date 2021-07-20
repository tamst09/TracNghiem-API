using FrontEndWebApp.Areas.Admin.AdminServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamsController : Controller
    {
        private readonly IExamManage _examService;
        private readonly IQuestionManage _questionManage;

        public ExamsController(IExamManage examService, IQuestionManage questionManage)
        {
            _examService = examService;
            _questionManage = questionManage;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var model = new ExamPagingRequest()
            {
                keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var allExamPaged = await _examService.GetAllPaging(model);
            if (!allExamPaged.success)
            {
                ViewData["msg"] = allExamPaged.msg;
                return View();
            }
            allExamPaged.data.Items = allExamPaged.data.Items.OrderByDescending(e => e.TimeCreated).ToList();
            return View(allExamPaged.data);
        }

        public IActionResult Create()
        {
            ViewData["msg"] = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamModel model)
        {
            ViewData["msg"] = "";
            if (string.IsNullOrEmpty(model.ExamName))
            {
                return View(model);
            }

            var createResponse = await _examService.Create(model);
            ViewData["msg"] = createResponse.msg;
            return View();
        }
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["msg"] = "";
            var res = await _examService.GetByID(id);
            if (!res.success)
            {
                ViewData["msg"] = res.msg;
            }
            var exam = res.data;
            var model = new ExamModel()
            {
                ID = exam.ID,
                ExamName = exam.ExamName,
                ImageURL = exam.ImageURL,
                CategoryID = exam.CategoryID,
                isActive = exam.isActive,
                isPrivate = exam.isPrivate,
                Time = exam.Time
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ExamModel model)
        {
            ViewData["msg"] = "";
            if (string.IsNullOrEmpty(model.ExamName))
            {
                ViewData["msg"] = "Tên không được bỏ trống";
                return View(model);
            }
            var updateResponse = await _examService.Update(model);
            ViewData["msg"] = updateResponse.msg;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody] int[] s)
        {
            if (s.Length == 0)
            {
                return Json(new { deleteResult = false });
            }
            DeleteManyModel<int> deleteModel = new DeleteManyModel<int>();
            deleteModel.ListItem.AddRange(s);
            var result = await _examService.DeleteMany(deleteModel);
            return Json(new { deleteResult = result.success });
        }
        public async Task<IActionResult> ShowQuestions(int id, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            ViewData["SubTitle"] = string.Empty;
            ViewData["msg"] = string.Empty;
            ViewData["examID"] = id;

            var getExamResponse = await _examService.GetByID(id);
            ViewData["SubTitle"] = getExamResponse.success ? "Đề thi: " + getExamResponse.data.ExamName : string.Empty;

            var model = new QuestionPagingRequest()
            {
                ExamID = id,
                keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var getAllQuesPagedResponse = await _questionManage.GetAllPaging(model);
            if (!getAllQuesPagedResponse.success)
            {
                ViewData["msg"] = getAllQuesPagedResponse.msg;
                return View();
            }
            else
            {
                return View(getAllQuesPagedResponse.data);
            }
        }
    }
}
