using UrlShortener.Services;
using Xunit;

namespace UrlShortener.Tests.Services;

public class Base62EncoderTests
{
    [Fact]
    public void Encode_Zero_ReturnsFirstCharacterOfAlphabet()
    {
        var result = Base62Encoder.Encode(0);

        Assert.Equal("0", result);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(61, "Z")]
    [InlineData(62, "10")]
    [InlineData(123456, "w7e")]
    public void Encode_KnownValues_ReturnsExpectedCode(long input, string expected)
    {
        var result = Base62Encoder.Encode(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(61)]
    [InlineData(62)]
    [InlineData(123456)]
    [InlineData(999999999)]
    public void Encode_Then_Decode_ReturnsOriginalValue(long original)
    {
        var encoded = Base62Encoder.Encode(original);
        var decoded = Base62Encoder.Decode(encoded);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Encode_NegativeNumberBehavior_ShouldBeDocumented()
    {
        // Base62Encoder non gestisce numeri negativi: questo test documenta
        // il comportamento attuale, utile se in futuro si vuole validare l'input
        var result = Base62Encoder.Encode(-5);

        Assert.Equal(string.Empty, result);
    }
}