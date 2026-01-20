using Safety_Wheel.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Safety_Wheel.ViewModels
{
    public class QuestionCreateViewModel : ObservableObject
    {
        public Question NewQuestion { get; set; }

        public ObservableCollection<OptionCreateViewModel> Options { get; }
            = new();

        private bool _isGhost;
        public bool IsGhost
        {
            get => _isGhost;
            private set => SetProperty(ref _isGhost, value);
        }

        private readonly Action _onActivated;

        private string? _previewImagePath;
        public string? PreviewImagePath
        {
            get => _previewImagePath;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _previewImagePath = null;
                    OnPropertyChanged();
                    return;
                }

                var fullPath = Path.IsPathRooted(value)
                    ? value
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);

                _previewImagePath = fullPath;
                OnPropertyChanged();
            }
        }



        public string Text
        {
            get => NewQuestion.TestQuest ?? "";
            set
            {
                NewQuestion.TestQuest = value;
                OnPropertyChanged();
                RecalculateGhostState();
                _onActivated?.Invoke();
            }
        }

        public string Comments
        {
            get => NewQuestion.Comments ?? "";
            set
            {
                NewQuestion.Comments = value;
                OnPropertyChanged();
                _onActivated?.Invoke();
            }
        }

        public bool IsMultiImage
        {
            get => NewQuestion.QuestionType == 2;
            set
            {
                NewQuestion.QuestionType = value ? 2 : 1;

                if (!value && string.IsNullOrEmpty(PreviewImagePath))
                    PreviewImagePath = PicturePath;

                ResetOptions();
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSingleImage));
            }
        }

        public bool IsSingleImage => !IsMultiImage;

        public string PicturePath
        {
            get => NewQuestion.PicturePath;
            set
            {
                var fullPath = Path.IsPathRooted(value)
                    ? value
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);

                NewQuestion.PicturePath = fullPath;
                OnPropertyChanged();
            }
        }



        public QuestionCreateViewModel(Question question, bool isGhost, Action onActivated)
        {
            NewQuestion = question;
            IsGhost = isGhost;
            _onActivated = onActivated;

            PreviewImagePath = question.PicturePath;
            SyncGhostOptions();
        }

        private void RecalculateGhostState()
        {
            IsGhost = string.IsNullOrWhiteSpace(Text);
        }

        private void ResetOptions()
        {
            Options.Clear();
            SyncGhostOptions();
        }

        public void SyncGhostOptions()
        {
            var realOptions = Options.Where(o => !o.IsGhost).ToList();
            var ghostOptions = Options.Where(o => o.IsGhost).ToList();

            if (!realOptions.Any())
            {
                Options.Clear();
                Options.Add(CreateGhostOption());
                return;
            }

            if (ghostOptions.Count == 0)
                Options.Add(CreateGhostOption());

            if (ghostOptions.Count > 1)
            {
                foreach (var extra in ghostOptions.Skip(1).ToList())
                    Options.Remove(extra);
            }
        }

        private OptionCreateViewModel CreateGhostOption()
        {
            return new OptionCreateViewModel(true, IsMultiImage, this);
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
                .Count(o => (bool)o.IsCorrect);

            NewQuestion.QuestionType = correctCount <= 1 ? 1 : 3;
        }

        public void SetQuestionImage(string path)
        {
            var fullPath = Path.IsPathRooted(path)
                ? path
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            PicturePath = fullPath;
            PreviewImagePath = fullPath;
        }

    }
}
