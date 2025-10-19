using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate;

[EntityTypeConfiguration<MenuTemplateEntityConfiguration, MenuGeneratorTemplateEntity>]
public class MenuGeneratorTemplateEntity
{
	public const string DayTemplatesName = nameof(_dayTemplates);

	private readonly SortedSet<OrderedDayMenuTemplateEntity> _dayTemplates = [];

	private DayTemplatesReadOnlyDictionaryWrapper? _dayTemplatesReadOnlyDictionary;

	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public required string Name { get; set; } = null!;

	public IReadOnlyDictionary<int, DayMenuTemplateEntity> DayTemplatesReadOnly
		=> _dayTemplatesReadOnlyDictionary ??= new DayTemplatesReadOnlyDictionaryWrapper(_dayTemplates);

	public bool TryAddDayTemplate(int order, DayMenuTemplateEntity dayTemplateToAdd)
	{
		if (_dayTemplates.Any(x => x.DayMenuTemplateId == dayTemplateToAdd.Id))
		{
			return false;
		}

		return _dayTemplates.Add
		(
			new OrderedDayMenuTemplateEntity
			{
				MenuGeneratorTemplateId = Id,
				Order = order,
				DayMenuTemplateId = dayTemplateToAdd.Id,
				DayMenuTemplateEntity = dayTemplateToAdd
			}
		);
	}

	public bool RemoveDayTemplate(DayMenuTemplateEntity dayTemplateToRemove)
	{
		var orderedDayMenuTemplateToRemove = _dayTemplates.FirstOrDefault
			(x => x.DayMenuTemplateId == dayTemplateToRemove.Id);

		return orderedDayMenuTemplateToRemove is not null && _dayTemplates.Remove(orderedDayMenuTemplateToRemove);
	}

	public bool UpdateDayTemplateOrder(int newOrder, DayMenuTemplateEntity dayTemplateToUpdate)
	{
		var orderedDayMenuTemplateToUpdate = _dayTemplates.FirstOrDefault
			(x => x.DayMenuTemplateId == dayTemplateToUpdate.Id);

		if (orderedDayMenuTemplateToUpdate is null)
		{
			return false;
		}

		if (orderedDayMenuTemplateToUpdate.Order == newOrder)
		{
			return true;
		}

		if (!_dayTemplates.Remove(orderedDayMenuTemplateToUpdate))
		{
			return false;
		}

		orderedDayMenuTemplateToUpdate.Order = newOrder;

		return _dayTemplates.Add(orderedDayMenuTemplateToUpdate);
	}

	public void ClearDayTemplates() => _dayTemplates.Clear();

	public void NormalizeDayTemplatesOrder()
	{
		var defensiveCopyArray = _dayTemplates.ToArray();
		Array.Sort(defensiveCopyArray); // just to be sure that the order is correct
		_dayTemplates.Clear();

		for (var i = 0; i < defensiveCopyArray.Length; i++)
		{
			const int orderStep = 100_000;

			var normalizedOrder = orderStep * (i + 1);

			defensiveCopyArray[i].Order = normalizedOrder;

			_dayTemplates.Add(defensiveCopyArray[i]);
		}
	}

	private class DayTemplatesReadOnlyDictionaryWrapper
		(SortedSet<OrderedDayMenuTemplateEntity> dayTemplates) : IReadOnlyDictionary<int, DayMenuTemplateEntity>
	{
		public IEnumerator<KeyValuePair<int, DayMenuTemplateEntity>> GetEnumerator()
			=> dayTemplates.Select
						   (
							   x => new KeyValuePair<int, DayMenuTemplateEntity>
							   (
								   x.Order,
								   x.DayMenuTemplateEntity
							   )
						   )
						   .GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => dayTemplates.Count;

		public bool ContainsKey(int key) => dayTemplates.Any(x => x.Order == key);

		public bool TryGetValue(int key, [MaybeNullWhen(false)] out DayMenuTemplateEntity value)
		{
			var orderedValue = dayTemplates.FirstOrDefault(x => x.Order == key);

			value = orderedValue?.DayMenuTemplateEntity;

			return orderedValue is not null;
		}

		public DayMenuTemplateEntity this[int key] => dayTemplates.First(x => x.Order == key).DayMenuTemplateEntity;

		public IEnumerable<int> Keys => dayTemplates.Select(x => x.Order);

		public IEnumerable<DayMenuTemplateEntity> Values => dayTemplates.Select(x => x.DayMenuTemplateEntity);
	}
}
