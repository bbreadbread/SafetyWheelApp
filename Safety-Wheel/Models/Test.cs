using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Safety_Wheel.Models;

public class Test : ObservableObject
{
    private int _id;
    private string? _name;
    private int? _subjectId;
    private int? _teacherId;
    private int? _penaltyMax;
    private int? _maxScore;
    private ObservableCollection<Question> _questions = new();
    private Subject? _subject;
    private Teacher? _teacher;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int? SubjectId
    {
        get => _subjectId;
        set => SetProperty(ref _subjectId, value);
    }

    public int? TeacherId
    {
        get => _teacherId;
        set => SetProperty(ref _teacherId, value);
    }

    public int? PenaltyMax
    {
        get => _penaltyMax;
        set => SetProperty(ref _penaltyMax, value);
    }

    public int? MaxScore
    {
        get => _maxScore;
        set => SetProperty(ref _maxScore, value);
    }

    public virtual ObservableCollection<Question> Questions
    {
        get => _questions;
        set => SetProperty(ref _questions, value);
    }

    public virtual Subject? Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    public virtual Teacher? Teacher
    {
        get => _teacher;
        set => SetProperty(ref _teacher, value);
    }
}
