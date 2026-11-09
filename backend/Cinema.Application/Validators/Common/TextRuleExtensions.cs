using Cinema.Application.Utils;
using FluentValidation;

namespace Cinema.Application.Validators.Common;

public static class TextRuleExtensions
{
    public static IRuleBuilderOptions<T, string> NoControlCharacters<T>(
        this IRuleBuilder<T, string> ruleBuilder
    ) =>
        ruleBuilder
            .Must(value => !NameNormalizer.ContainsDisallowedCharacters(value))
            .WithMessage("Giá trị chứa ký tự không hợp lệ. Xóa ký tự lạ rồi thử lại.");
}
