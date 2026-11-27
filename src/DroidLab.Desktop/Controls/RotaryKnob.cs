using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DroidLab.Desktop.Controls;

// Range semantics, keyboard navigation and automation are inherited from Slider.
public sealed class RotaryKnob : Slider
{
    public RotaryKnob()
    {
        MinWidth = 72;
        MinHeight = 72;
        IsMoveToPointEnabled = true;
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var center = new Point(ActualWidth / 2, ActualHeight / 2);
        var radius = Math.Max(0, Math.Min(ActualWidth, ActualHeight) / 2 - 8);
        if (radius <= 0) return;
        var fraction = Maximum > Minimum ? (Value - Minimum) / (Maximum - Minimum) : 0;
        var angle = (135 + 270 * fraction) * Math.PI / 180;
        dc.DrawEllipse(Background ?? Brushes.Transparent, new Pen(Foreground, 2), center, radius, radius);
        dc.DrawLine(new Pen(Foreground, 3), center,
            new Point(center.X + radius * .8 * Math.Cos(angle), center.Y + radius * .8 * Math.Sin(angle)));
        if (IsKeyboardFocused)
            dc.DrawEllipse(null, new Pen(SystemColors.HighlightBrush, 2), center, radius + 4, radius + 4);
    }

    protected override void OnValueChanged(double oldValue, double newValue)
    {
        base.OnValueChanged(oldValue, newValue);
        InvalidateVisual();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        Focus();
        CaptureMouse();
        SetFromPoint(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            SetFromPoint(e.GetPosition(this));
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (IsMouseCaptured) ReleaseMouseCapture();
        e.Handled = true;
    }

    protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsKeyboardFocusedChanged(e);
        InvalidateVisual();
    }

    private void SetFromPoint(Point p)
    {
        var angle = Math.Atan2(p.Y - ActualHeight / 2, p.X - ActualWidth / 2) * 180 / Math.PI;
        var sweep = (angle - 135 + 360) % 360;
        var fraction = sweep <= 270 ? sweep / 270 : sweep < 315 ? 1 : 0;
        var value = Minimum + fraction * (Maximum - Minimum);
        if (IsSnapToTickEnabled)
        {
            var nearest = Minimum;
            var distance = Math.Abs(value - nearest);
            void Consider(double tick)
            {
                if (tick < Minimum || tick > Maximum) return;
                var delta = Math.Abs(value - tick);
                if (delta < distance || (delta == distance && tick > nearest))
                {
                    nearest = tick;
                    distance = delta;
                }
            }
            Consider(Maximum);
            if (Ticks is { Count: > 0 })
            {
                foreach (var tick in Ticks) Consider(tick);
            }
            else if (TickFrequency > 0)
            {
                var index = (value - Minimum) / TickFrequency;
                Consider(Minimum + Math.Floor(index) * TickFrequency);
                Consider(Minimum + Math.Ceiling(index) * TickFrequency);
            }
            value = nearest;
        }
        SetCurrentValue(ValueProperty, value);
    }
}
