using Microsoft.EntityFrameworkCore;
using Safety_Wheel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Wheel.Services
{
    public class QuestionService
    {
        private readonly SafetyWheelContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<Question> Questions { get; set; } = new();

        public QuestionService()
        {
            GetAll();
        }

        public Question Add(Question question, Test test, int i)
        {
            var _question = new Question
            {
                TestId = test.Id,
                Number = i,
                TestQuest = question.TestQuest,
                PicturePath = question.PicturePath,
                Comments = question.Comments,
                QuestionType = question.QuestionType
            };
            _db.Add(_question);
            Commit();
            return _question;
        }

        public int Commit() => _db.SaveChanges();

        public void GetAll()
        {
            IQueryable<Question> query = _db.Questions
                .Include(q => q.Test)
                .Include(q => q.Options)
                .Include(q => q.StudentAnswers);
            var questions = query.ToList();
            Questions.Clear();

            foreach (var question in questions)
            {
                Questions.Add(question);
            }
        }

        public void Remove(Question question)
        {
            _db.Remove(question);
            if (Commit() > 0)
                if (Questions.Contains(question))
                    Questions.Remove(question);
        }

        public void Update(Question question)
        {
            var existing = _db.Questions.Find(question.Id);
            if (existing != null)
            {
                existing.TestId = question.TestId;
                existing.Number = question.Number;
                existing.TestQuest = question.TestQuest;
                existing.PicturePath = question.PicturePath;
                existing.Comments = question.Comments;
                Commit();
            }
        }

        public List<Question> GetQoestiosForCurrentTest(int currentTest)
        {
            return Questions
                  .Where(q => q.TestId == currentTest)
                  .OrderBy(q => q.Number)
                  .ToList();
        }

        public void DeleteByTest(int testId)
        {

            var questions = _db.Questions
                .Where(q => q.TestId == testId)
                .ToList();

            if (!questions.Any())
                return;

            var questionIds = questions.Select(q => q.Id).ToList();

            var options = _db.Options
                .Where(o => questionIds.Contains(o.QuestionId))
                .ToList();

            _db.Options.RemoveRange(options);
            _db.Questions.RemoveRange(questions);

            _db.SaveChanges();
        }

    }
}
