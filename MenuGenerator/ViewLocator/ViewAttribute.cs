using System;

namespace MenuGenerator.ViewLocator;

[AttributeUsage(AttributeTargets.Class)]
public class ViewAttribute(Type viewType) : Attribute
{
	public Type ViewType { get; set; } = viewType;
}
