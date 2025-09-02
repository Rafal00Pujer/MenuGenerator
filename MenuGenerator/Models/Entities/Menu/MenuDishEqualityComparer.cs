using System.Collections.Generic;
using MenuGenerator.Models.Entities.Dish;

namespace MenuGenerator.Models.Entities.Menu;

public class MenuDishEqualityComparer : IEqualityComparer<DishEntity>
{
    public bool Equals(DishEntity? x, DishEntity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.Type == y.Type;
    }

    public int GetHashCode(DishEntity obj)
    {
        return (int)obj.Type;
    }
}