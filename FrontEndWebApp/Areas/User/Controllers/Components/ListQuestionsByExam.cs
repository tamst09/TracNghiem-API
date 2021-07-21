using FrontEndWebApp.Areas.User.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Question;

namespace FrontEndWebApp.Areas.User.Controllers.Components
{
    [ViewComponent(Name = "QuestionsByExam")]
    public class ListQuestionsByExam : ViewComponent
    {
        private readonly IQuestionService questionService;

        public ListQuestionsByExam(IQuestionService questionService)
        {
            this.questionService = questionService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int examId)
        {
            var listQuestions = await questionService.GetByExamID(examId);
            var data = listQuestions.data.Select(q => new QuestionModel()
            {
                ExamID = q.ExamID,
                ID = q.ID,
                Answer = q.Answer,
                Option1 = q.Option1,
                Option2 = q.Option2,
                Option3 = q.Option3,
                Option4 = q.Option4,
                QuesContent = q.QuesContent,
                STT = q.STT
            }).ToList();
            return View("Default", data);
        }
    }
}
