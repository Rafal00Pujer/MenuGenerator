using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using MenuGenerator.ViewLocator;

namespace MenuGenerator.ViewModel.MainWindow;

[View(typeof(MainWindow))]
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string _selectedButton;

    [ObservableProperty] private bool _test;

    [ObservableProperty] private bool _test1;

    public List<string> Buttons { get; set; } =
    [
        "Menu History",
        "Menu Templates",
        "Dishes",
        "Dishes Types",
        "Dishes Attributes",
        "Allergens",
        "Occurence Rules"
    ];
}