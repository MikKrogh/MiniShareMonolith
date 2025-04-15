using System.Diagnostics.Metrics;

namespace UserModule.OpenTelemetry;

public static class UserCreatedMeter
{
    public static readonly Meter Meter = new Meter("UserModule.UserCreated", "1.0.0");
    public static string MeterName => Meter.Name;    
    public static readonly Counter<int> UserCreatedCounter = Meter.CreateCounter<int>("user_created_count");

    //public Meter Meter { get; private set; }
    //public Counter<int> UserCreatedCounter { get; private set; }
    //public static string MeterName => "UserModule";
    //public UserCreatedMeter(string meterName, string meterVersion)
    //{
    //    Meter = new Meter(meterName, "1.0.0");
    //    UserCreatedCounter = Meter.CreateCounter<int>("user.created");
    //}
}
