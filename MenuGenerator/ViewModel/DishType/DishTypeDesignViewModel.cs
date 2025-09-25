using System;
using System.Linq;

namespace MenuGenerator.ViewModel.DishType;

public class DishTypeDesignViewModel : DishTypeViewModel
{
    public DishTypeDesignViewModel()
    {
        DishType = new DishTypeEditDesignViewModel(false);

        foreach (var i in Enumerable.Range(1, 100))
        {
            DishTypeSummaries.Add(new DishTypeSummary(Guid.AllBitsSet, $"Dish Type {i}"));
        }
    }
}