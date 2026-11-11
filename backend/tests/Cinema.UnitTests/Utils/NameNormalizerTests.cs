using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class NameNormalizerTests
{
    [Theory]
    [InlineData("Hành động", "Hành động")]
    [InlineData("  Hành   động  ", "Hành động")]
    [InlineData("\tHành\u00A0động\n", "Hành động")]
    [InlineData("   ", "")]
    [InlineData(null, "")]
    public void Normalize_TrimsAndCollapsesWhitespace(string? input, string expected)
    {
        NameNormalizer.Normalize(input).ShouldBe(expected);
    }

    [Fact]
    public void Normalize_ComposedAndDecomposedForms_AreEqual()
    {
        var composed = "\u1EBE";
        var decomposed = "E\u0302\u0301";

        NameNormalizer.Normalize(decomposed).ShouldBe(composed);
    }

    [Theory]
    [InlineData("Hành động", "hành  ĐỘNG")]
    [InlineData("IMAX", " imax ")]
    [InlineData("T13", "t13")]
    public void AreSame_IgnoresCaseAndExtraWhitespace(string left, string right)
    {
        NameNormalizer.AreSame(left, right).ShouldBeTrue();
    }

    [Fact]
    public void AreSame_DifferentDiacritics_AreDifferent()
    {
        NameNormalizer.AreSame("Hành động", "Hanh dong").ShouldBeFalse();
    }

    [Theory]
    [InlineData("Hành\u0000động", true)]
    [InlineData("Hành\u0007động", true)]
    [InlineData("Hành\tđộng", false)]
    [InlineData("Hành động", false)]
    [InlineData(null, false)]
    public void ContainsDisallowedCharacters_OnlyFlagsNonWhitespaceControls(
        string? input,
        bool expected
    )
    {
        NameNormalizer.ContainsDisallowedCharacters(input).ShouldBe(expected);
    }
}
