using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Validators.Bookings;
using FluentValidation.TestHelper;

namespace Cinema.UnitTests.Validators;

public class HoldSeatsRequestValidatorTests
{
    private readonly HoldSeatsRequestValidator _validator = new();

    private static HoldSeatsRequest Valid() =>
        new(
            ShowtimeId: 1,
            CustomerName: "Nguyen Van A",
            CustomerPhone: "0901234567",
            SeatIds: [1, 2],
            Combos: null
        );

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShowtimeIdNotPositive_FailsValidation()
    {
        var request = Valid() with { ShowtimeId = 0 };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.ShowtimeId);
    }

    [Fact]
    public void EmptyCustomerName_FailsValidation()
    {
        var request = Valid() with { CustomerName = "" };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void NoSeatsSelected_FailsValidation()
    {
        var request = Valid() with { SeatIds = [] };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.SeatIds);
    }

    [Fact]
    public void SeatIdNotPositive_FailsValidation()
    {
        var request = Valid() with { SeatIds = [1, 0] };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor("SeatIds[1]");
    }

    [Fact]
    public void DuplicateSeatIds_FailsValidation()
    {
        var request = Valid() with { SeatIds = [1, 1] };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.SeatIds);
    }

    [Fact]
    public void InvalidPhoneFormat_FailsValidation()
    {
        var request = Valid() with { CustomerPhone = "123" };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.CustomerPhone);
    }

    [Fact]
    public void NullPhone_IsOptionalAndPasses()
    {
        var request = Valid() with { CustomerPhone = null };

        _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.CustomerPhone);
    }

    [Fact]
    public void ComboWithZeroQuantity_FailsValidation()
    {
        var request = Valid() with
        {
            Combos = [new ComboSelectionRequest(ComboId: 1, Quantity: 0)],
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor("Combos[0].Quantity");
    }
}

public class CounterHoldRequestValidatorTests
{
    private readonly CounterHoldRequestValidator _validator = new();

    private static CounterHoldRequest Valid() =>
        new(
            ShowtimeId: 1,
            SeatIds: [1],
            Combos: null,
            UserId: null,
            CustomerName: "Khach le",
            CustomerPhone: null
        );

    [Fact]
    public void ValidRequestWithCustomerName_PassesValidation()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidRequestWithUserIdInsteadOfCustomerName_PassesValidation()
    {
        var request = Valid() with { UserId = 5, CustomerName = null! };

        _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void NeitherUserIdNorCustomerName_FailsValidation()
    {
        var request = Valid() with { UserId = null, CustomerName = null! };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void UserIdNotPositive_FailsValidation()
    {
        var request = Valid() with { UserId = 0 };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.UserId);
    }
}

public class CheckoutRequestValidatorTests
{
    private readonly CheckoutRequestValidator _validator = new();

    [Theory]
    [InlineData("VNPay")]
    [InlineData("Momo")]
    [InlineData("Stripe")]
    public void SupportedMethod_PassesValidation(string method)
    {
        _validator.TestValidate(new CheckoutRequest(method)).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("PayPal")]
    [InlineData("cash")]
    public void UnsupportedOrEmptyMethod_FailsValidation(string method)
    {
        _validator
            .TestValidate(new CheckoutRequest(method))
            .ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }
}

public class CounterCheckoutRequestValidatorTests
{
    private readonly CounterCheckoutRequestValidator _validator = new();

    [Fact]
    public void ZeroAmountReceived_FailsValidation()
    {
        _validator
            .TestValidate(new CounterCheckoutRequest(0m))
            .ShouldHaveValidationErrorFor(x => x.AmountReceived);
    }

    [Fact]
    public void NegativeAmountReceived_FailsValidation()
    {
        _validator
            .TestValidate(new CounterCheckoutRequest(-1000m))
            .ShouldHaveValidationErrorFor(x => x.AmountReceived);
    }

    [Fact]
    public void PositiveAmountReceived_PassesValidation()
    {
        _validator
            .TestValidate(new CounterCheckoutRequest(100_000m))
            .ShouldNotHaveAnyValidationErrors();
    }
}

public class CheckinRequestValidatorTests
{
    private readonly CheckinRequestValidator _validator = new();

    [Fact]
    public void EmptyQrCode_FailsValidation()
    {
        _validator.TestValidate(new CheckinRequest("")).ShouldHaveValidationErrorFor(x => x.QrCode);
    }

    [Fact]
    public void NonEmptyQrCode_PassesValidation()
    {
        _validator
            .TestValidate(new CheckinRequest("BK20260101ABCD-A1"))
            .ShouldNotHaveAnyValidationErrors();
    }
}
