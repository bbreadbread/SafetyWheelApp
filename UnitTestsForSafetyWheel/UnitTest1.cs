
using Safety_Wheel.Models;
using Safety_Wheel.Services;

namespace UnitTestsForSafetyWheel
{
    public class BooksControllerTest

    {
        private StudentAnswerService CreateService(out SafetyWheelContext context)
        {
            context = TestDbContext.CreateDbContext();

            BaseDbService.Instance.SetContext(context);

            return new StudentAnswerService();
        }

        [Fact]
        public void Add_WhenAnswerIsValid_AddsAnswerToDatabase()
        {
            // Arrange
            var service = CreateService(out var context);
            var answer = new StudentAnswer
            {
                AttemptId = 1,
                QuestionId = 1,
                OptionId = 1,
                IsCorrect = true
            };

            // Act
            service.Add(answer);

            // Assert
            Assert.Single(context.StudentAnswers);
        }
        [Fact]
        public void GetAll_WhenCalled_ReturnsAllAnswers()
        {
            // Arrange
            var service = CreateService(out var context);
            context.StudentAnswers.Add(new StudentAnswer { AttemptId = 1, QuestionId = 1 });
            context.SaveChanges();

            // Act
            service.GetAll();

            // Assert
            Assert.Single(service.StudentAnswers);
        }
        [Fact]
        public void Remove_WhenAnswerExists_RemovesAnswer()
        {
            // Arrange
            var service = CreateService(out var context);
            var answer = new StudentAnswer { AttemptId = 1, QuestionId = 1 };
            context.StudentAnswers.Add(answer);
            context.SaveChanges();

            // Act
            service.Remove(answer);

            // Assert
            Assert.Empty(context.StudentAnswers);
        }
        [Fact]
        public void Update_WhenAnswerExists_UpdatesAnswer()
        {
            // Arrange
            var service = CreateService(out var context);
            var answer = new StudentAnswer
            {
                AttemptId = 1,
                QuestionId = 1,
                OptionId = 1
            };
            context.StudentAnswers.Add(answer);
            context.SaveChanges();

            answer.OptionId = 2;

            // Act
            service.Update(answer);

            // Assert
            Assert.Equal(2, context.StudentAnswers.First().OptionId);
        }
        [Fact]
        public void IsReady_WhenNotAllQuestionsAnswered_ReturnsFalse()
        {
            // Arrange
            var service = CreateService(out var context);
            var test = new Test { Id = 1 };
            context.Questions.Add(new Question { Id = 1, TestId = 1 });
            context.SaveChanges();

            var attempt = new Attempt { Id = 1 };

            // Act
            var result = service.IsReady(attempt, test);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void IsReady_WhenAllQuestionsAnswered_ReturnsTrue()
        {
            // Arrange
            var service = CreateService(out var context);
            context.Questions.Add(new Question { Id = 1, TestId = 1 });
            context.StudentAnswers.Add(new StudentAnswer { AttemptId = 1, QuestionId = 1 });
            context.SaveChanges();

            var test = new Test { Id = 1 };
            var attempt = new Attempt { Id = 1 };

            // Act
            var result = service.IsReady(attempt, test);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void GetQuestionCorrectness_WhenNoAnswers_ReturnsNull()
        {
            // Arrange
            var service = CreateService(out var context);
            var attempt = new Attempt { Id = 1 };

            // Act
            var result = service.GetQuestionCorrectness(attempt, 1);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void GetQuestionCorrectness_WhenCorrect_ReturnsTrue()
        {
            // Arrange
            var service = CreateService(out var context);
            context.Options.Add(new Option { Id = 1, QuestionId = 1, IsCorrect = true });
            context.StudentAnswers.Add(new StudentAnswer
            {
                AttemptId = 1,
                QuestionId = 1,
                OptionId = 1
            });
            context.SaveChanges();

            var attempt = new Attempt { Id = 1 };

            // Act
            var result = service.GetQuestionCorrectness(attempt, 1);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void GetAllQuestionCorrectness_WhenCalled_ReturnsDictionary()
        {
            // Arrange
            var service = CreateService(out var context);
            context.Options.Add(new Option { Id = 1, QuestionId = 1, IsCorrect = true });
            context.StudentAnswers.Add(new StudentAnswer
            {
                AttemptId = 1,
                QuestionId = 1,
                OptionId = 1
            });
            context.SaveChanges();

            var attempt = new Attempt { Id = 1 };

            // Act
            var result = service.GetAllQuestionCorrectness(attempt, new List<int> { 1 });

            // Assert
            Assert.True(result[1]);
        }
        [Fact]
        public void GetQoestiosForCurrentTest_WhenCalled_ReturnsOrderedQuestions()
        {
            // Arrange
            var service = CreateService(out var context);
            context.Questions.Add(new Question { Id = 1, TestId = 1, Number = 2 });
            context.Questions.Add(new Question { Id = 2, TestId = 1, Number = 1 });
            context.SaveChanges();

            // Act
            var result = service.GetQoestiosForCurrentTest(1);

            // Assert
            Assert.Equal(2, result.First().Id);
        }


    }
}