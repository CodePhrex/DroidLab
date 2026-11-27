using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DroidLab.Desktop.Controls;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var knob = new RotaryKnob { Minimum = 10, Maximum = 60, Value = 10, SmallChange = 5, LargeChange = 10 };
        AutomationProperties.SetName(knob, "Discovery interval in seconds");
        var peer = UIElementAutomationPeer.CreatePeerForElement(knob) ?? throw new Exception("Missing knob automation peer");
        Check(peer.GetName() == "Discovery interval in seconds", "Knob accessible name");
        var range = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider ?? throw new Exception("Missing knob range provider");
        Check(!range.IsReadOnly && range.Minimum == 10 && range.Maximum == 60, "Knob range");
        range.SetValue(35);
        Check(knob.Value == 35, "Automation updates knob");
        knob.Value = 100;
        Check(knob.Value == 60, "Upper bound");
        knob.Value = -1;
        Check(knob.Value == 10, "Lower bound");

        var meter = new RadialMeter { Minimum = 0, Maximum = 100 };
        AutomationProperties.SetName(meter, "Battery charge");
        var meterPeer = UIElementAutomationPeer.CreatePeerForElement(meter) ?? throw new Exception("Missing meter automation peer");
        Check(meterPeer.GetName() == "Battery charge", "Meter accessible name");
        Check(!meter.IsValueKnown && meterPeer.GetPattern(PatternInterface.RangeValue) == null, "Unknown reading hides numeric range");
        Check(meterPeer.GetItemStatus() == "Reading unavailable", "Unknown reading status");
        meter.IsValueKnown = true;
        meter.Value = 0;
        var reading = meterPeer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider ?? throw new Exception("Known zero missing range");
        Check(reading.IsReadOnly && reading.Value == 0, "Known zero is read-only and distinct from unknown");
        meter.Value = 75;
        Check(reading.Value == 75, "Updated reading");
        meter.IsIndeterminate = true;
        Check(meterPeer.GetPattern(PatternInterface.RangeValue) == null, "Indeterminate reading hides numeric range");
        Console.WriteLine("WPF runtime control smoke checks passed.");
    }

    private static void Check(bool condition, string label)
    {
        if (!condition) throw new InvalidOperationException(label);
        Console.WriteLine("PASS: " + label);
    }
}
