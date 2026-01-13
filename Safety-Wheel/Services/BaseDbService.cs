using Safety_Wheel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Wheel.Services
{
    internal class BaseDbService
    {
        private static BaseDbService? instance;
        private SafetyWheelContext context;
        public SafetyWheelContext Context => context;
        private BaseDbService()
        {
            context = new SafetyWheelContext();
        }
        public static BaseDbService Instance
        {
            get
            {
                if (instance == null)
                    instance = new BaseDbService();
                return instance;
            }
        }
    }
}
