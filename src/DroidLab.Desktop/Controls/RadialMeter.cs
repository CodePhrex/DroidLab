using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace DroidLab.Desktop.Controls;

// Read-only telemetry control. Unknown values are explicitly distinguished from zero.
public sealed class RadialMeter : ProgressBar
{
    public static readonly DependencyProperty IsValueKnownProperty =
        DependencyProperty.Register(nameof(IsValueKnown), typeof(bool), typeof(RadialMeter),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    public bool IsValueKnown
    {
        get => (bool)GetValue(IsValueKnownProperty);
        set => SetValue(IsValueKnownProperty, value);
    }

    public RadialMeter()
    {
        MinWidth = 96;
        MinHeight = 96;
        Template = new ControlTemplate(typeof(ProgressBar));
        SetResourceReference(ForegroundProperty, "Accent");
        SetResourceReference(BackgroundProperty, "Edge");
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new MeterPeer(this);

    protected override void OnValueChanged(double oldValue, double newValue)
    {
        base.OnValueChanged(oldValue, newValue);
        InvalidateVisual();
    }

    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
        base.OnMinimumChanged(oldMinimum, newMinimum);
        InvalidateVisual();
    }

    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
        base.OnMaximumChanged(oldMaximum, newMaximum);
        InvalidateVisual();
    }

    private bool HasReading => IsValueKnown && !IsIndeterminate && Maximum > Minimum;

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var center = new Point(ActualWidth / 2, ActualHeight / 2);
        var radius = Math.Min(ActualWidth, ActualHeight) / 2 - 8;
        if (radius <= 0) return;
        var track = new Pen(Background ?? SystemColors.ControlDarkBrush, 6);
        var active = new Pen(Foreground ?? SystemColors.HighlightBrush, 6);
        dc.DrawEllipse(null, track, center, radius, radius);
        var fraction = HasReading ? Math.Clamp((Value - Minimum) / (Maximum - Minimum), 0, 1) : 0;
        if (fraction >= 1)
            dc.DrawEllipse(null, active, center, radius, radius);
        else if (fraction > 0)
        {
            var angle = 2 * Math.PI * fraction;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(new Point(center.X, center.Y - radius), false, false);
                context.ArcTo(new Point(center.X + radius * Math.Sin(angle),
                    center.Y - radius * Math.Cos(angle)), new Size(radius, radius),
                    0, fraction > .5, SweepDirection.Clockwise, true, false);
            }
            geometry.Freeze();
            dc.DrawGeometry(null, active, geometry);
        }
        var label = HasReading ? fraction.ToString("P0", CultureInfo.CurrentCulture) : "—";
        var text = new FormattedText(label, CultureInfo.CurrentCulture, FlowDirection,
            new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
            Math.Max(10, Math.Min(24, radius / 2)), Foreground ?? SystemColors.ControlTextBrush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
        dc.DrawText(text, new Point(center.X - text.Width / 2, center.Y - text.Height / 2));
    }

    private sealed class MeterPeer : ProgressBarAutomationPeer
    {
        private readonly RadialMeter meter;
        public MeterPeer(RadialMeter owner) : base(owner) => meter = owner;
        protected override string GetItemStatusCore() => meter.HasReading
            ? meter.Value.ToString(CultureInfo.CurrentCulture) : "Reading unavailable";
        public override object GetPattern(PatternInterface patternInterface) =>
            patternInterface == PatternInterface.RangeValue && !meter.HasReading
                ? null! : base.GetPattern(patternInterface);
    }
}
