using Safety_Wheel.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Safety_Wheel.ViewModels
{
    public class QuestionCreateViewModel : ObservableObject
    {
        public Question NewQuestion { get; set; } = new();

        public ObservableCollection<OptionCreateViewModel> Options { get; }
            = new();

        private bool _isGhost;
        public bool IsGhost
        {
            get => _isGhost;
            set => SetProperty(ref _isGhost, value);
        }

        private readonly Action _onActivated;

        private bool _wasEverFilled = false;

        public string Text
        {
            get => NewQuestion.TestQuest ?? "";
            set
            {
                NewQuestion.TestQuest = value;
                OnPropertyChanged();

                if (IsGhost && !_wasEverFilled && value.Length > 0)
                {
                    _wasEverFilled = true;
                    IsGhost = false;
                    _onActivated?.Invoke();
                }
            }
        }


        public bool IsMultiImage
        {
            get => NewQuestion.QuestionType == 2;
            set
            {
                NewQuestion.QuestionType = value ? 2 : 1;

                if (value)
                    NewQuestion.PicturePath = "//";

                ResetOptions();
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSingleImage));
            }
        }

        public bool IsSingleImage => !IsMultiImage;

        public string PicturePath
        {
            get => NewQuestion.PicturePath ?? "";
            set
            {
                NewQuestion.PicturePath = value;
                OnPropertyChanged();
            }
        }

        public QuestionCreateViewModel(bool isGhost, Action onActivated)
        {
            IsGhost = isGhost;
            _onActivated = onActivated;
            AddGhostOption();
        }

        private void AddGhostOption()
        {
            Options.Add(new OptionCreateViewModel(
                true,
                IsMultiImage,
                this,
                OnOptionActivated));
        }

        private void OnOptionActivated()
        {
            if (Options.Any(o => o.IsGhost)) return;
            AddGhostOption();
        }

        private void ResetOptions()
        {
            Options.Clear();
            AddGhostOption();
        }

        public void RecalculateQuestionType()
        {
            if (IsMultiImage)
            {
                NewQuestion.QuestionType = 2;
                return;
            }

            int correctCount = Options
                                .Where(o => !o.IsGhost)
                                .Count(o => o.NewOption.IsCorrect == true);

            NewQuestion.QuestionType = correctCount <= 1 ? 1 : 3;
        }

        public void SetQuestionImage(string path)
        {
            PicturePath = path;
        }
    }
}
