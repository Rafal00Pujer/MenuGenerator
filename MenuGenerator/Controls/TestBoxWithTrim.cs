using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MenuGenerator.Controls;

public class TestBoxWithTrim : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        Text = Text?.Trim();

        base.OnLostFocus(e);
    }
}