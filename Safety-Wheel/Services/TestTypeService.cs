using Microsoft.EntityFrameworkCore;
using Safety_Wheel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Safety_Wheel.Services
{
    public class TestTypeService
    {
        private readonly SafetyWheelContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<TestType> TestTypes { get; set; } = new();

        public TestTypeService(int? type = null)
        {
            GetAll();
        }

        public void Add(TestType testType)
        {
            var _testType = new TestType
            {
                Name = testType.Name,
                Description = testType.Description,
                TimeLimitSecond = testType.TimeLimitSecond
            };
            _db.Add(_testType);
            Commit();
        }

        public int Commit() => _db.SaveChanges();

        public void GetAll()
        {
            var query = _db.TestTypes.ToList();

            foreach (var testType in query)
            {
                TestTypes.Add(testType);
            }
        }
        public TestType GetTypeById(int? type)
        {
            var testType = TestTypes
                .FirstOrDefault(t => t.Id == type);

            return testType;
        }

        public void Remove(TestType testType)
        {
            _db.Remove(testType);
            if (Commit() > 0)
                if (TestTypes.Contains(testType))
                    TestTypes.Remove(testType);
        }

        public void Update(TestType testType)
        {
            var existing = _db.TestTypes.Find(testType.Id);
            if (existing != null)
            {
                existing.Name = testType.Name;
                existing.Description = testType.Description;
                existing.TimeLimitSecond = testType.TimeLimitSecond;
                Commit();
            }
        }
    }
}
