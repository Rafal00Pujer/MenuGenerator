using Avalonia.Controls;
using Avalonia.Logging;

namespace MenuGenerator.ViewModel.DishType;

public partial class DishTypeView : UserControl
{
    public DishTypeView()
    {
        InitializeComponent();

        SizeChanged += (sender, args) =>
        {
            Logger.Sink!.Log(LogEventLevel.Warning,"DishTypeView",this,"SizeChanged");
        };
    }
}