using System;
using System.Collections.Generic;

namespace MenuGenerator.Models.Entities.MenuTemplate;

public class OrderedDayMenuTemplateEntity : IComparable<OrderedDayMenuTemplateEntity>, IComparable
{
	public required int Order { get; set; }
	
	public required Guid MenuGeneratorTemplateId { get; set; } = Guid.Empty;

	public required Guid DayMenuTemplateId { get; set; } = Guid.Empty;

	public required DayMenuTemplateEntity DayMenuTemplateEntity { get; set; }

	public int CompareTo(OrderedDayMenuTemplateEntity? other)
	{
		if (ReferenceEquals(this, other)) return 0;

		return other is null ? 1 : Order.CompareTo(other.Order);
	}

	public int CompareTo(object? obj)
	{
		if (obj is null) return 1;
		if (ReferenceEquals(this, obj)) return 0;

		return obj is OrderedDayMenuTemplateEntity other
			? CompareTo(other)
			: throw new ArgumentException($"Object must be of type {nameof(OrderedDayMenuTemplateEntity)}");
	}

	public static bool operator <
		(OrderedDayMenuTemplateEntity? left, OrderedDayMenuTemplateEntity? right)
		=> Comparer<OrderedDayMenuTemplateEntity>.Default.Compare(left, right) < 0;

	public static bool operator >
		(OrderedDayMenuTemplateEntity? left, OrderedDayMenuTemplateEntity? right)
		=> Comparer<OrderedDayMenuTemplateEntity>.Default.Compare(left, right) > 0;

	public static bool operator <=
		(OrderedDayMenuTemplateEntity? left, OrderedDayMenuTemplateEntity? right)
		=> Comparer<OrderedDayMenuTemplateEntity>.Default.Compare(left, right) <= 0;

	public static bool operator >=
		(OrderedDayMenuTemplateEntity? left, OrderedDayMenuTemplateEntity? right)
		=> Comparer<OrderedDayMenuTemplateEntity>.Default.Compare(left, right) >= 0;
}
