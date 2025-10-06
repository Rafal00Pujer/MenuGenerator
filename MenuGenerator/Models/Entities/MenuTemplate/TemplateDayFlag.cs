using System;

namespace MenuGenerator.Models.Entities.MenuTemplate;

[Flags]
public enum TemplateDayFlag
{
	None = 0,
	Monday = 1,
	Tuesday = 2,
	Wednesday = 4,
	Thursday = 8,
	Friday = 16,
	Saturday = 32,
	Sunday = 64,
	Week = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
	All = Week,
	FirstFlag = Monday,
	LastFlag = Sunday
}
