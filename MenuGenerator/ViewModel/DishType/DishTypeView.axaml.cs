using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MenuGenerator.ViewModel.DishType;

public partial class DishTypeView : UserControl
{
    public DishTypeView()
    {
        InitializeComponent();

        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        Dispatcher.UIThread.Post(AdjustDishTypesScrollHeight, args.NewSize, DispatcherPriority.Default);
    }

    private void AdjustDishTypesScrollHeight(object? controlSize)
    {
        if (controlSize is not Size size)
            throw new ArgumentException("ControlSize must be of type size", nameof(controlSize));

        AdjustDishTypesScrollHeight(size);
    }

    private void AdjustDishTypesScrollHeight(Size controlSize)
    {
        var addNewBtnSize = AddNewBtn.DesiredSize;

        DishTypesScroll.Height = controlSize.Height - addNewBtnSize.Height;
    }
}