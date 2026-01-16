using Safety_Wheel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Wheel.ViewModels
{
    public class TestListItemViewModel
    {
        public Test? Test { get; }
        public bool IsCreateCard { get; }

        public TestListItemViewModel(Test test)
        {
            Test = test;
            IsCreateCard = false;
        }

        public TestListItemViewModel()
        {
            IsCreateCard = true;
        }
    }
}


