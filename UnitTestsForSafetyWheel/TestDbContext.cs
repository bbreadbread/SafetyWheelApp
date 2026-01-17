using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsForSafetyWheel
{
    private SafetyWheelContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SafetyWheelContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SafetyWheelContext(options);
    }

}
