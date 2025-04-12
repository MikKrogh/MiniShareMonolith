using System.Diagnostics.Metrics;

namespace UserModule.OpenTelemetry;

public class UserCreatedMeter
{
    public Meter Meter { get; private set; }
    public Counter<int> UserCreatedCounter { get; private set; }
    public static string MeterName => "UserModule";
    public UserCreatedMeter(string meterName, string meterVersion)
    {
        Meter = new Meter(meterName, "1.0.0");
        UserCreatedCounter = Meter.CreateCounter<int>("user.created");
    }
}
