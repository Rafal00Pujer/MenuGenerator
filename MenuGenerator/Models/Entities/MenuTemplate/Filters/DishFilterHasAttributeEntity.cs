using System;
using System.Linq;
using MenuGenerator.Models.Entities.Dish;
using MenuGenerator.Models.Entities.DishAttribute;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate.Filters;

[EntityTypeConfiguration(typeof(DishFilterHasAttributeEntityConfiguration))]
public class DishFilterHasAttributeEntity : DishFilterEntity
{
	public required Guid AttributeId { get; set; } = Guid.Empty;
	
	public required DishAttributeEntity Attribute { get; set; }
	
	public bool Negate { get; set; }
	
	// XOR truth table:
	// Console.WriteLine(true ^ true);   output: False
	// Console.WriteLine(true ^ false);  output: True
	// Console.WriteLine(false ^ true);  output: True
	// Console.WriteLine(false ^ false); output: False
	public override bool CanBeUsed(DishEntity dish) => Negate ^ dish.AttributeList.Any(x => x.Id == AttributeId);
}
