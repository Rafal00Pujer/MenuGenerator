using System;

namespace MenuGenerator.ViewModel.DishType;

public class DishTypeEditDesignViewModel : DishTypeEditViewModel
{
    public DishTypeEditDesignViewModel(bool empty = true, string sufix = "") : base(null!, null!, null!)
    {
        if (empty)
        {
            return;
        }

        _id = Guid.NewGuid();

        Name = "Dish Name";
        Description = "Dish Description";

        if (string.IsNullOrEmpty(sufix))
        {
            return;
        }

        Name = Name + " " + sufix;
    }
}