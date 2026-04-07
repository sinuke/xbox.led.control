using XboxLedControl;

namespace XboxLedControl.Tests;

public class LedArgParserTests
{
    // --- Keyword patterns ---

    [Theory]
    [InlineData("off",        GipLedPattern.Off,             0)]
    [InlineData("0",          GipLedPattern.Off,             0)]
    [InlineData("on",         GipLedPattern.On,             47)]
    [InlineData("solid",      GipLedPattern.On,             47)]
    [InlineData("ramp",       GipLedPattern.RampToLevel,    47)]
    [InlineData("breathe",    GipLedPattern.RampToLevel,    47)]
    [InlineData("breath",     GipLedPattern.RampToLevel,    47)]
    [InlineData("fade",       GipLedPattern.RampToLevel,    47)]
    [InlineData("fastblink",  GipLedPattern.FastBlink,      47)]
    [InlineData("blink",      GipLedPattern.FastBlink,      47)]
    [InlineData("blink1",     GipLedPattern.FastBlink,      47)]
    [InlineData("slowblink",  GipLedPattern.SlowBlink,      47)]
    [InlineData("slow",       GipLedPattern.SlowBlink,      47)]
    [InlineData("blink2",     GipLedPattern.SlowBlink,      47)]
    [InlineData("charging",   GipLedPattern.ChargingBlink,  47)]
    [InlineData("charge",     GipLedPattern.ChargingBlink,  47)]
    [InlineData("full",       GipLedPattern.On,             47)]
    [InlineData("max",        GipLedPattern.On,             47)]
    [InlineData("100",        GipLedPattern.On,             47)]
    public void Parse_KnownKeyword_ReturnsCorrectResult(string arg, GipLedPattern expectedPattern, byte expectedIntensity)
    {
        var result = LedArgParser.Parse(arg);
        Assert.NotNull(result);
        Assert.Equal(expectedPattern,   result.Value.pattern);
        Assert.Equal(expectedIntensity, result.Value.intensity);
    }

    // --- Numeric brightness ---

    [Theory]
    [InlineData("1",   GipLedPattern.On,  0)]   // ScaleIntensity(1) = 0
    [InlineData("50",  GipLedPattern.On, 24)]   // ScaleIntensity(50) = 24
    [InlineData("99",  GipLedPattern.On, 47)]   // ScaleIntensity(99) = 47
    public void Parse_NumericBrightness_ScalesCorrectly(string arg, GipLedPattern expectedPattern, byte expectedIntensity)
    {
        var result = LedArgParser.Parse(arg);
        Assert.NotNull(result);
        Assert.Equal(expectedPattern,   result.Value.pattern);
        Assert.Equal(expectedIntensity, result.Value.intensity);
    }

    // --- Out-of-range values ---

    [Theory]
    [InlineData("-1")]
    [InlineData("-100")]
    [InlineData("101")]
    [InlineData("200")]
    [InlineData("255")]
    public void Parse_OutOfRange_ReturnsNull(string arg)
    {
        var result = LedArgParser.Parse(arg);
        Assert.Null(result);
    }

    // --- Unknown keyword ---

    [Fact]
    public void Parse_UnknownKeyword_DefaultsToOff()
    {
        var result = LedArgParser.Parse("unknown");
        Assert.NotNull(result);
        Assert.Equal(GipLedPattern.Off, result.Value.pattern);
        Assert.Equal((byte)0,           result.Value.intensity);
    }
}
