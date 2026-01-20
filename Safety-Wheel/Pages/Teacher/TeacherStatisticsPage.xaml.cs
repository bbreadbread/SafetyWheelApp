using Safety_Wheel.Models;
using Safety_Wheel.Services;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Safety_Wheel.Pages.Teacher
{
    public partial class TeacherStatisticsPage : UserControl
    {
        public static TeacherStatisticsPage DataPageTeacher { get; private set; }

        private readonly TestService _testService = new();
        private readonly AttemptService _attemptService = new();
        private readonly StudentAnswerService _studentAnswerService = new();

        public TeacherStatisticsPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        public void LoadStatistics(Models.Student student = null, Test test = null)
        {
            _testService.GetAll(null, CurrentUser.Id);

            StudentTitle.Text = student == null
                    ? "Общая статистика"
                    : $"Статистика студента: {student.Name}";

            var testStats = new List<TestStats>();
            var allTime = new List<OverallTimeData>();

            IEnumerable<Test> testsToProcess =
                test != null
                    ? new[] { test }
                    : _testService.Tests;

            foreach (var t in testsToProcess)
            {
                var stats = student == null
                    ? GetTestStatsOverall(t)
                    : GetTestStats(t, student);

                if (stats == null)
                    continue;

                testStats.Add(stats);

                allTime.Add(new OverallTimeData
                {
                    TestName = t.Name,
                    AverageDuration = stats.AverageDuration
                });
            }

            TestsItemsControl.ItemsSource = testStats;
            DrawOverallTimeChart(allTime);
        }


        private TestStats GetTestStatsOverall(Test test)
        {
            var attempts = _attemptService.GetAttemptsByTest(test.Id)
                .Where(a => a.StartedAt.HasValue && a.FinishedAt.HasValue)
                .ToList();

            if (!attempts.Any())
            {
                return new TestStats
                {
                    TestId = test.Id,
                    TestName = $"{test.Subject.Name} {test.Name}",
                    HasData = false,
                    EmptyMessage = $"Тест «{test.Name}» ещё никто не проходил"
                };
            }


            var durations = attempts
                .Select(a => (a.FinishedAt.Value - a.StartedAt.Value).TotalMinutes)
                .ToList();

            if (!durations.Any())
                return null;

            var stats = new TestStats
            {
                TestId = test.Id,
                TestName = $"{test.Subject.Name} {test.Name}",
                AverageDuration = Math.Round(durations.Average(), 2)
            };

            foreach (var q in test.Questions.OrderBy(q => q.Number))
            {
                var answers = _studentAnswerService.GetAnswersByQuestion(q.Id);

                if (!answers.Any())
                    continue;

                int correct = answers.Count(a => a.IsCorrect == true);
                double percent = (double)correct / answers.Count * 100;

                stats.QuestionNumbers.Add((double)q.Number);
                stats.SuccessRates.Add(Math.Round(percent, 2));
            }

            return stats;
        }


        private TestStats GetTestStats(Test test, Models.Student student)
        {
            var attempts = _attemptService.GetAttemptsByTest(test.Id)
                .Where(a => a.StudentsId == student.Id)
                .Where(a => a.StartedAt.HasValue && a.FinishedAt.HasValue)
                .ToList();

            if (!attempts.Any())
            {
                return new TestStats
                {
                    TestId = test.Id,
                    TestName = $"{test.Subject.Name} {test.Name}",
                    HasData = false,
                    EmptyMessage = $"Студент не проходил тест «{test.Subject.Name} {test.Name}»"
                };
            }


            var stats = new TestStats
            {
                TestId = test.Id,
                TestName = $"{test.Subject.Name} {test.Name}"
            };

            var durations = attempts
                .Select(a => (a.FinishedAt.Value - a.StartedAt.Value).TotalMinutes)
                .ToList();

            if (!durations.Any())
                return null;

            stats.AverageDuration = Math.Round(durations.Average(), 2);

            foreach (var q in test.Questions.OrderBy(q => q.Number))
            {
                var answers = _studentAnswerService.GetAnswersByQuestion(q.Id)
                    .Where(a => a.Attempt.StudentsId == student.Id)
                    .ToList();

                if (!answers.Any())
                    continue;

                int correct = answers.Count(a => a.IsCorrect == true);
                double percent = (double)correct / answers.Count * 100;

                stats.QuestionNumbers.Add((double)q.Number);
                stats.SuccessRates.Add(Math.Round(percent, 2));
            }

            return stats;
        }

        private void DrawOverallTimeChart(List<OverallTimeData> data)
        {
            OverallTimePlot.Plot.Clear();

            if (!data.Any())
                return;

            var x = Enumerable.Range(0, data.Count).Select(i => (double)i).ToArray();
            var y = data.Select(d => d.AverageDuration).ToArray();
            var labels = data.Select(d => d.TestName).ToArray();

            OverallTimePlot.Plot.Add.Bars(x, y);

            OverallTimePlot.Plot.Axes.Bottom.SetTicks(x, labels);
            OverallTimePlot.Plot.Axes.Bottom.TickLabelStyle.Rotation = 45;

            double maxY = y.Max();
            double yPadding = Math.Max(1, maxY * 0.1);

            OverallTimePlot.Plot.Axes.SetLimits(
                -0.5,
                x.Length - 0.5,
                0,
                maxY + yPadding
            );

            OverallTimePlot.Plot.Title("Среднее время выполнения тестов");
            OverallTimePlot.Plot.YLabel("Минуты");

            OverallTimePlot.Refresh();

        }

        private void PerformancePlot_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not WpfPlot plot)
                return;

            plot.Plot.Clear();

            if (plot.DataContext is not TestStats stats)
            {
                plot.Plot.Title("Нет данных");
                plot.Refresh();
                return;
            }

            if (!stats.HasData)
            {
                plot.Plot.Title(stats.EmptyMessage ?? "Нет данных");
                plot.Refresh();
                return;
            }

           

            plot.Plot.Title("Динамика успеваемости");
            plot.Plot.XLabel("Номер вопроса");
            plot.Plot.YLabel("Процент правильных (%)");

            if (!stats.QuestionNumbers.Any())
            {
                plot.Plot.Title("Нет данных для отображения");
                plot.Refresh();
                return;
            }

            plot.Plot.Add.Bars(
                stats.QuestionNumbers.ToArray(),
                stats.SuccessRates.ToArray()
            );

            double minX = stats.QuestionNumbers.Min() - 0.5;
            double maxX = stats.QuestionNumbers.Max() + 0.5;

            plot.Plot.Axes.SetLimits(
                minX,
                maxX,
                0,
                100
            );

            plot.Refresh();
        }

        private bool _isInitialized = false;

        private void TeacherStatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataPageTeacher = this;

            if (_isInitialized)
                return;

            _isInitialized = true;

            LoadStatistics(null);
        }
    }

    public class TestStats
    {
        public int TestId { get; set; }
        public string TestName { get; set; }

        public double AverageDuration { get; set; }

        public bool HasData { get; set; } = true;
        public string EmptyMessage { get; set; }

        public List<double> QuestionNumbers { get; set; } = new();
        public List<double> SuccessRates { get; set; } = new();
    }


    public class OverallTimeData
    {
        public string TestName { get; set; }
        public double AverageDuration { get; set; }
    }
}
