using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class VietnamTimeTests
{
    [Fact]
    public void IanaId_DefaultZone_IsIanaFormat() =>
        VietnamTime.IanaId.ShouldBe("Asia/Ho_Chi_Minh");
}
