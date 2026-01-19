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

        public TeacherCreateTestViewModel() { }

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

        private void AddGhostQuestion()
        {
            Questions.Add(new QuestionCreateViewModel(true, OnQuestionActivated));
        }

        private void OnQuestionActivated()
        {
            if (Questions.Any(q => q.IsGhost))
                return;

            AddGhostQuestion();
        }

        public void Save()
        {
            int qi = 1;
            if (SelectedSubject == null)
            {
                MessageBox.Show("Выберите дисциплину");
                return;
            }

            if (IsEditMode)
            {
                _testService.Update(Test);

                var dbQuestions = _questionService
                    .GetQuestiosForCurrentTest(Test.Id)
                    .ToList();

                var vmQuestions = Questions
                    .Where(q => !q.IsGhost)
                    .ToList();

                int qj = 1;

                foreach (var qvm in vmQuestions)
                {
                    qvm.NewQuestion.TestId = Test.Id;
                    qvm.NewQuestion.Number = qj++;

                    if (qvm.NewQuestion.QuestionType == 2)
                        qvm.NewQuestion.PicturePath = "//";

                    Question savedQuestion;

                    if (qvm.NewQuestion.Id == 0)
                    {
                        savedQuestion = _questionService.Add(
                            qvm.NewQuestion,
                            Test,
                            (int)qvm.NewQuestion.Number);
                    }
                    else
                    {
                        _questionService.Update(qvm.NewQuestion);
                        savedQuestion = qvm.NewQuestion;
                    }

                    var dbOptions = _optionService
                        .GetOptionsByQuestion(savedQuestion.Id)
                        .ToList();

                    var vmOptions = qvm.Options
                        .Where(o => !o.IsGhost)
                        .ToList();

                    if (!vmOptions.Any(o => o.NewOption.IsCorrect == true))
                    {
                        MessageBox.Show("В вопросе нет правильного ответа");
                        return;
                    }

                    int oi = 1;

                    foreach (var ovm in vmOptions)
                    {
                        ovm.NewOption.QuestionId = savedQuestion.Id;
                        ovm.NewOption.Number = oi++;

                        if (ovm.NewOption.Id == 0)
                        {
                            _optionService.Add(ovm.NewOption, (int)ovm.NewOption.Number);
                        }
                        else
                        {
                            _optionService.Update(ovm.NewOption);
                        }
                    }

                    foreach (var dbOpt in dbOptions)
                    {
                        if (!vmOptions.Any(o => o.NewOption.Id == dbOpt.Id))
                        {
                            _optionService.Remove(dbOpt);
                        }
                    }
                }

                foreach (var dbQ in dbQuestions)
                {
                    if (!vmQuestions.Any(q => q.NewQuestion.Id == dbQ.Id))
                    {
                        _questionService.Remove(dbQ);
                    }
                }

                MessageBox.Show("Тест обновлён");
                return;
            }


            _testService.Add(Test, Questions.Count(q => !q.IsGhost));



            foreach (var q in Questions.Where(q => !q.IsGhost))
            {
                q.NewQuestion.TestId = Test.Id;

                if (q.NewQuestion.QuestionType == 2)
                    q.NewQuestion.PicturePath = "//";

                var savedQuestion = _questionService.Add(
                    q.NewQuestion,
                    _testService.GetLastTest(),
                    qi++);

                var realOptions = q.Options.Where(o => !o.IsGhost).ToList();

                if (!realOptions.Any(o => o.NewOption.IsCorrect == true))
                {
                    MessageBox.Show("В вопросе нет правильного ответа");
                    return;
                }

                int oi = 1;
                foreach (var o in realOptions)
                {
                    o.NewOption.QuestionId = savedQuestion.Id;
                    _optionService.Add(o.NewOption, oi++);
                }
            }

            MessageBox.Show("Тест сохранён");
        }

        private void LoadTestForEdit(Test test)
        {
            Test = test;
            SelectedSubject = Subjects.FirstOrDefault(s => s.Id == test.SubjectId);

            Questions.Clear();

            var questions = _questionService.GetQuestiosForCurrentTest(test.Id);

            foreach (var q in questions)
            {
                var qvm = new QuestionCreateViewModel(false, OnQuestionActivated)
                {
                    NewQuestion = q
                };

                if (!string.IsNullOrEmpty(q.PicturePath))
                    qvm.PreviewImagePath = q.PicturePath;



                qvm.Options.Clear();

                var options = _optionService.GetOptionsByQuestion(q.Id);

                foreach (var opt in options)
                {
                    qvm.Options.Add(new OptionCreateViewModel(
                        false,
                        q.QuestionType == 2,
                        qvm)
                    {
                        NewOption = opt
                    });
                }

                qvm.SyncGhostOptions();
                Questions.Add(qvm);
            }

            AddGhostQuestion();
        }
    }
}