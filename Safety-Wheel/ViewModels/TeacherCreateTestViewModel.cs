using Safety_Wheel.Models;
using Safety_Wheel.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Safety_Wheel.ViewModels
{
    public class TeacherCreateTestViewModel : ObservableObject
    {
        public bool IsEditMode { get; }
        public Test Test { get; set; } = new();

        public ObservableCollection<QuestionCreateViewModel> Questions { get; }
            = new();

        private readonly SubjectService _subjectService = new();
        public ObservableCollection<Subject> Subjects => _subjectService.Subjects;

        private Subject? _selectedSubject;
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();

                if (value != null)
                    Test.SubjectId = value.Id;
            }
        }

        private readonly TestService _testService = new();
        private readonly QuestionService _questionService = new();
        private readonly OptionService _optionService = new();

        public TeacherCreateTestViewModel(Test? test = null)
        {
            if (test == null)
            {
                IsEditMode = false;
                Test = new Test();
                AddGhostQuestion();
            }
            else
            {
                IsEditMode = true;
                LoadTestForEdit(test);
            }
        }
        public TeacherCreateTestViewModel()
        {
        }

        private void AddGhostQuestion()
        {
            Questions.Add(new QuestionCreateViewModel(true, OnQuestionActivated));
        }

        private void OnQuestionActivated()
        {
            if (Questions.Any(q => q.IsGhost)) return;
            AddGhostQuestion();
        }

        public void Save()
        {
            if (SelectedSubject == null)
            {
                MessageBox.Show("Выберите дисциплину");
                return;
            }
            if (IsEditMode)
            {
                _testService.Update(Test);

                _questionService.DeleteByTest(Test.Id);

                int i = 1;

                foreach (var q in Questions.Where(q => !q.IsGhost))
                {
                    q.NewQuestion.TestId = Test.Id;

                    var savedQuestion = _questionService.Add(q.NewQuestion, Test, i);

                    var realOptions = q.Options.Where(o => !o.IsGhost).ToList();

                    if (!realOptions.Any(o => o.NewOption.IsCorrect == true))
                    {
                        MessageBox.Show("В вопросе нет правильного ответа");
                        return;
                    }

                    int j = 1;
                    foreach (var o in realOptions)
                    {
                        o.NewOption.QuestionId = savedQuestion.Id;
                        _optionService.Add(o.NewOption, j++);
                    }

                    i++;
                }

                MessageBox.Show("Тест обновлён");
            }
            else
            {
                _testService.Add(Test, Questions.Count() - 1);
                int i = 1;
                int j = 1;
                foreach (var q in Questions.Where(q => !q.IsGhost))
                {
                    q.NewQuestion.TestId = Test.Id;

                    if (q.NewQuestion.QuestionType == 2)
                        q.NewQuestion.PicturePath = "//";

                    var savedQuestion = _questionService.Add(q.NewQuestion, _testService.GetLastTest(), i);

                    var realOptions = q.Options.Where(o => !o.IsGhost).ToList();

                    if (!realOptions.Any(o => o.NewOption.IsCorrect == true))
                    {
                        MessageBox.Show("В вопросе нет правильного ответа");
                        return;
                    }

                    j = 1;
                    foreach (var o in realOptions)
                    {
                        o.NewOption.QuestionId = savedQuestion.Id;
                        _optionService.Add(o.NewOption, j);
                        j++;
                    }

                    i++;
                }
            }

            MessageBox.Show("Тест сохранён");
        }
        private void LoadTestForEdit(Test test)
        {
            Test = test;
            SelectedSubject = Subjects.FirstOrDefault(s => s.Id == test.SubjectId);

            Questions.Clear();

            var list = _questionService.GetQoestiosForCurrentTest(test.Id);

            foreach (var q in list)
            {
                var qvm = new QuestionCreateViewModel(false, OnQuestionActivated)
                {
                    PicturePath = q.PicturePath
                };

                qvm.NewQuestion = q;

                _optionService.GetOptionsByQuestion(q.Id);

                foreach (var opt in _optionService.Options)
                {
                    qvm.Options.Add(new OptionCreateViewModel(
                        false,
                        q.QuestionType == 2,
                        qvm,
                        null)
                    {
                        NewOption = opt
                    });
                }

                qvm.Options.Add(new OptionCreateViewModel(
                    true,
                    q.QuestionType == 2,
                    qvm,
                    qvm.RecalculateQuestionType));

                Questions.Add(qvm);
            }

            AddGhostQuestion();
        }

    }
}
