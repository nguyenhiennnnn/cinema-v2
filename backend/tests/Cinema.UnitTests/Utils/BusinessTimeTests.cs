using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class BusinessTimeTests
{
    [Fact]
    public void IanaId_DefaultZone_IsIanaFormat() =>
        BusinessTime.IanaId.ShouldBe("Asia/Ho_Chi_Minh");
}
