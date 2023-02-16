using System;
using System.Windows;
using System.Windows.Controls;

namespace TycoonFactory.UI.Controls;

public partial class TimePicker
{
    public static readonly DependencyProperty HoursProperty = DependencyProperty.Register(nameof(Hours), typeof(int),
        typeof(TimePicker),
        new FrameworkPropertyMetadata
        {
            DefaultValue = DateTime.Now.Hour,
            CoerceValueCallback = CoerceHours
        },
        Validate);

    public static readonly DependencyProperty MinutesProperty =
        DependencyProperty.Register(nameof(Minutes), typeof(int), typeof(TimePicker), new FrameworkPropertyMetadata
            {
                DefaultValue = DateTime.Now.Minute,
                CoerceValueCallback = CoerceMinutes
            },
            Validate);

    public TimePicker()
    {
        InitializeComponent();
    }

    public int Hours
    {
        get => (int) GetValue(HoursProperty);
        set => SetValue(HoursProperty, value);
    }

    public int Minutes
    {
        get => (int) GetValue(MinutesProperty);
        set => SetValue(MinutesProperty, value);
    }

    private static bool Validate(object value)
    {
        return value is int;
    }

    private static object CoerceHours(DependencyObject d, object value)
    {
        var hours = (int) value;

        if (hours > 23)
            return 23;
        if (hours < 0)
            return 0;

        return value;
    }
    
    private static object CoerceMinutes(DependencyObject d, object value)
    {
        var minutes = (int) value;

        if (minutes > 59)
            return 59;
        if (minutes < 0)
            return 0;

        return value;
    }
}