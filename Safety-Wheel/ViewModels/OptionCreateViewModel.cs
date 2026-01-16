using Safety_Wheel.Models;

namespace Safety_Wheel.ViewModels
{
    public class OptionCreateViewModel : ObservableObject
    {
        public Option NewOption { get; set; } = new();

        private bool _isGhost;
        public bool IsGhost
        {
            get => _isGhost;
            set => SetProperty(ref _isGhost, value);
        }

        public bool IsImageOption { get; }

        private readonly QuestionCreateViewModel _parent;
        private readonly System.Action _onActivated;

        public string Value
        {
            get => NewOption.TextAnswer ?? "";
            set
            {
                NewOption.TextAnswer = value;
                OnPropertyChanged();

                if (IsGhost && !string.IsNullOrWhiteSpace(value))
                {
                    IsGhost = false;
                    _onActivated?.Invoke();
                }
            }
        }

        public bool? IsCorrect
        {
            get => NewOption.IsCorrect ?? false;
            set
            {
                NewOption.IsCorrect = value;
                OnPropertyChanged();

                _parent.RecalculateQuestionType();
            }
        }

        public OptionCreateViewModel(
            bool isGhost,
            bool isImageOption,
            QuestionCreateViewModel parent,
            System.Action onActivated)
        {
            IsGhost = isGhost;
            IsImageOption = isImageOption;
            _parent = parent;
            _onActivated = onActivated;
        }

        public void SetOptionImage(string path)
        {
            Value = path;
        }
    }
}
