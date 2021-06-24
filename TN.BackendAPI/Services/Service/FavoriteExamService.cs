using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;

namespace TN.BackendAPI.Services.Service
{
    public class FavoriteExamService : IFavoriteExamService
    {
        private readonly TNDbContext _db;

        public FavoriteExamService(TNDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Add(int userId, int examId)
        {
            var favoriteExam = await _db.FavoriteExams.Where(e => e.ExamID == examId && e.UserID == userId).FirstOrDefaultAsync();
            if (favoriteExam == null)
            {
                var exam = await _db.Exams.FindAsync(examId);
                var user = await _db.Users.FindAsync(userId);
                if (exam != null && user != null)
                {
                    favoriteExam.AppUser = user;
                    favoriteExam.Exam = exam;
                    _db.FavoriteExams.Add(favoriteExam);
                    try
                    {
                        _db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
                else return false;
            }
            return true;
        }

        public async Task<bool> Delete(int userId, int examId)
        {
            var exam = await _db.FavoriteExams.Where(e => e.ExamID == examId && e.UserID == userId).FirstOrDefaultAsync();
            if (exam != null)
            {
                try
                {
                    _db.FavoriteExams.Remove(exam);
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }

        public async Task<List<Exam>> GetByUser(int userId)
        {
            List<Exam> exams = new List<Exam>();
            var favoritedExams = await _db.FavoriteExams.Where(e => e.UserID == userId).ToListAsync();
            foreach (var exam in favoritedExams)
            {
                var e = await _db.Exams.FindAsync(exam.ExamID);
                if (e != null)
                {
                    exams.Add(e);
                }
            }
            return exams;
        }
    }
}
