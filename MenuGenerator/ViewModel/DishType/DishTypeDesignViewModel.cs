using System.Linq;

namespace MenuGenerator.ViewModel.DishType;

public class DishTypeDesignViewModel : DishTypeViewModel
{
    public DishTypeDesignViewModel()
    {
        SelectedDishType = new DishTypeEditDesignViewModel();

        foreach (var i in Enumerable.Range(1, 100))
        {
            DishTypeSummaries.Add(i);
        }
    }
}