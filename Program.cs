using XboxLedControl;

/*
 * Xbox Controller LED Control
 * ===========================
 * Controls the Guide-button LED on a USB-connected Xbox One / Series controller.
 * Sends a GIP LED command directly via \\.\XboxGIP (xboxgip.sys).
 * No admin rights required.
 *
 * Usage:  XboxLedControl [--debug] <brightness|pattern>
 *
 * Protocol: MS-GIPUSB §3.1.5.5.7 (LED eButton Command)
 */

bool debug     = args.Any(a => a.Equals("--debug", StringComparison.OrdinalIgnoreCase));
string[] valueArgs = args.Where(a => !a.StartsWith("--", StringComparison.Ordinal)).ToArray();

if (valueArgs.Length == 0)
{
    Console.Error.WriteLine("Usage: XboxLedControl [--debug] <brightness|pattern>");
    Console.Error.WriteLine("  brightness:  0-100");
    Console.Error.WriteLine("  pattern:     off | on | ramp | fastblink | slowblink | charging");
    return 1;
}

var result = LedArgParser.Parse(valueArgs[0].ToLowerInvariant());
if (result is null)
    return 1;

var (pattern, intensity) = result.Value;
byte[] frame = GipLedCommand.BuildRaw(pattern, intensity);

if (debug)
{
    Console.WriteLine($"Pattern:   {pattern}");
    Console.WriteLine($"Intensity: {intensity}/47");
    Console.WriteLine($"GIP frame: {BitConverter.ToString(frame)}");
    Console.WriteLine();
}

bool ok = GipDirectSender.TrySend(frame, debug);
if (!ok)
    Console.Error.WriteLine("Failed: no USB Xbox controller found via \\\\.\\XboxGIP.");
return ok ? 0 : 1;
