namespace XboxLedControl;

internal static class LedArgParser
{
    internal static (GipLedPattern pattern, byte intensity)? Parse(string arg) =>
        arg switch
        {
            "off"  or "0"                            => (GipLedPattern.Off,            0),
            "on"   or "solid"                        => (GipLedPattern.On,            47),
            "ramp" or "breathe" or "breath" or "fade"
                                                     => (GipLedPattern.RampToLevel,   47),
            "fastblink" or "blink" or "blink1"       => (GipLedPattern.FastBlink,     47),
            "slowblink" or "slow"  or "blink2"       => (GipLedPattern.SlowBlink,     47),
            "charging"  or "charge"                  => (GipLedPattern.ChargingBlink, 47),
            "full" or "max" or "100"                 => (GipLedPattern.On,            47),
            _ when int.TryParse(arg, out int n) && n < 0
                                                     => InvalidArg(arg, "brightness must be 0-100"),
            _ when byte.TryParse(arg, out byte b)    => b > 100
                                                         ? InvalidArg(arg, "brightness must be 0-100")
                                                         : b == 0
                                                             ? (GipLedPattern.Off, (byte)0)
                                                             : (GipLedPattern.On, GipLedCommand.ScaleIntensity(b)),
            _                                        => FallbackOff(arg),
        };

    private static (GipLedPattern, byte)? InvalidArg(string a, string reason)
    {
        Console.Error.WriteLine($"Invalid value '{a}': {reason}.");
        return null;
    }

    private static (GipLedPattern, byte)? FallbackOff(string a)
    {
        Console.Error.WriteLine($"Unknown argument '{a}', defaulting to off.");
        return (GipLedPattern.Off, 0);
    }
}
