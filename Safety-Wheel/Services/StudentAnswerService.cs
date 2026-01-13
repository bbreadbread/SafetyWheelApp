using Microsoft.EntityFrameworkCore;
using Safety_Wheel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Safety_Wheel.Services
{
    public class StudentAnswerService
    {
        private readonly SafetyWheelContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<StudentAnswer> StudentAnswers { get; set; } = new();

        public StudentAnswerService()
        {
            GetAll();
        }

        public void Add(StudentAnswer studentAnswer)
        {
            var _studentAnswer = new StudentAnswer
            {
                AttemptId = studentAnswer.AttemptId,
                QuestionId = studentAnswer.QuestionId,
                OptionId = studentAnswer.OptionId,
                IsCorrect = studentAnswer.IsCorrect,
                AnsweredAt = studentAnswer.AnsweredAt,
                Attempt = studentAnswer.Attempt,
                Option = studentAnswer.Option,
                Question = studentAnswer.Question
            };
            _db.Add(_studentAnswer);
            Commit();
        }

        public int Commit() => _db.SaveChanges();

        public void GetAll(decimal? attemptId = null, decimal? questionId = null)
        {
            IQueryable<StudentAnswer> query = _db.StudentAnswers
                .Include(sa => sa.Attempt)
                .Include(sa => sa.Option)
                .Include(sa => sa.Question);

            if (attemptId != null)
                query = query.Where(sa => sa.AttemptId == attemptId);
            if (questionId != null)
                query = query.Where(sa => sa.QuestionId == questionId);

            var answers = query.ToList();
            StudentAnswers.Clear();

            foreach (var answer in answers)
            {
                StudentAnswers.Add(answer);
            }
        }

        public void Remove(StudentAnswer studentAnswer)
        {
            _db.Remove(studentAnswer);
            if (Commit() > 0)
                if (StudentAnswers.Contains(studentAnswer))
                    StudentAnswers.Remove(studentAnswer);
        }

        public void Update(StudentAnswer studentAnswer)
        {
            var existing = _db.StudentAnswers
                .FirstOrDefault(sa => sa.AttemptId == studentAnswer.AttemptId && sa.QuestionId == studentAnswer.QuestionId);
            if (existing != null)
            {
                existing.OptionId = studentAnswer.OptionId;
                existing.IsCorrect = studentAnswer.IsCorrect;
                existing.AnsweredAt = studentAnswer.AnsweredAt;
                Commit();
            }
        }

        public bool IsReady(Attempt attempt, Test test)
        {
            var testQuestions = _db.Questions
                .Where(q => q.TestId == test.Id)
                .ToList();

            var studentAnswers = _db.StudentAnswers
                .Where(w => w.AttemptId == attempt.Id)
                .ToList();

            foreach (var question in testQuestions)
            {
                var answersForQuestion = studentAnswers
                    .Where(a => a.QuestionId == question.Id)
                    .ToList();

                if (!answersForQuestion.Any())
                {
                    return false;
                }
            }

            return true;
        }

        public List<StudentAnswer> FinalListAnswers(Attempt attempt, Test test)
        {
            var testQuestions = _db.Questions
                .Where(q => q.TestId == test.Id)
                .ToList();

            var studentAnswers = _db.StudentAnswers
                .Where(w => w.AttemptId == attempt.Id)
                .ToList();

            return studentAnswers;
        }

        public bool? GetQuestionCorrectness(Attempt attempt, int questionId)
        {
            var studentOptions = _db.StudentAnswers
                                    .Where(sa => sa.AttemptId == attempt.Id &&
                                                 sa.QuestionId == questionId)
                                    .Select(sa => sa.OptionId)
                                    .ToList();

            if (!studentOptions.Any())
                return null;

            var correctOptions = _db.Options
                                    .Where(o => o.QuestionId == questionId &&
                                                o.IsCorrect == true)
                                    .Select(o => o.Id)
                                    .ToHashSet();

            return correctOptions.SetEquals(studentOptions.ToHashSet());
        }


        public StudentAnswer GetByQuestionAndAttempt(int questionId, int attemptId)
        {
            return _db.StudentAnswers
                .Include(sa => sa.Attempt)
                .Include(sa => sa.Question)
                .Include(sa => sa.Option)
                .FirstOrDefault(sa => sa.QuestionId == questionId &&
                                     sa.AttemptId == attemptId);
        }

        public Dictionary<int, bool?> GetAllQuestionCorrectness(Attempt attempt, List<int> questionIds)
        {
            var result = new Dictionary<int, bool?>();

            var allStudentAnswers = _db.StudentAnswers
                .Include(sa => sa.Option)
                .Where(sa => sa.AttemptId == attempt.Id && questionIds.Contains(sa.QuestionId))
                .ToList()
                .GroupBy(sa => sa.QuestionId)
                .ToDictionary(g => g.Key, g => g.Select(sa => sa.OptionId).ToList());

            var allCorrectOptions = _db.Options
                .Where(o => questionIds.Contains(o.QuestionId) && o.IsCorrect == true)
                .ToList()
                .GroupBy(o => o.QuestionId)
                .ToDictionary(g => g.Key, g => g.Select(o => o.Id).ToHashSet());

            foreach (var questionId in questionIds)
            {
                if (!allStudentAnswers.TryGetValue(questionId, out var studentOptions) ||
                    !studentOptions.Any())
                {
                    result[questionId] = null;
                    continue;
                }

                if (allCorrectOptions.TryGetValue(questionId, out var correctOptions))
                {
                    result[questionId] = correctOptions.SetEquals(studentOptions.ToHashSet());
                }
                else
                {
                    result[questionId] = false;
                }
            }

            return result;

        }

        public List<Question> GetQoestiosForCurrentTest(int currentTest)
        {
            return _db.Questions
                  .Include(q => q.Options)
                  .Where(q => q.TestId == currentTest)
                  .OrderBy(q => q.Number)
                  .ToList();



            //Image image = new Image
            //{
            //    Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)),
            //    Tag = tagValue,
            //    Width = 230,
            //    Height = 230,
            //    Stretch = Stretch.Uniform,
            //    RenderTransformOrigin = new Point(0.5, 0.5),
            //    Cursor = Cursors.Hand
            //};

            //ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0);
            //image.RenderTransform = scaleTransform;

            //Image magnifier = new Image
            //{
            //    Source = new BitmapImage(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/magnifier_icon.png"), UriKind.RelativeOrAbsolute)),
            //    Width = 32,
            //    Height = 32,
            //    Cursor = Cursors.Hand,
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    Margin = new Thickness(0, 5, 5, 0),
            //    Opacity = 0.8
            //};

            //Canvas overlay = new Canvas
            //{
            //    Width = 240,
            //    Height = 240,
            //    Background = Brushes.Transparent
            //};
            //Canvas.SetLeft(image, 0);
            //Canvas.SetTop(image, 0);
            //Canvas.SetLeft(magnifier, 230 - 32 - 5);
            //Canvas.SetTop(magnifier, 5);

            //overlay.Children.Add(image);
            //overlay.Children.Add(magnifier);

            //Border border = new Border
            //{
            //    Child = overlay,
            //    BorderBrush = Brushes.LightGray,
            //    BorderThickness = new Thickness(1),
            //    Margin = new Thickness(15),
            //    Padding = new Thickness(15),
            //    CornerRadius = new CornerRadius(5),
            //    Background = Brushes.White,
            //    Cursor = Cursors.Hand
            //};
        }
    }
}
