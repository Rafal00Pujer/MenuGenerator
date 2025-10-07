using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate;

[EntityTypeConfiguration<MenuTemplateEntityConfiguration, MenuTemplateEntity>]
public class MenuTemplateEntity
{
	private readonly List<DayTemplateEntity> _dayTemplates = [];
	
	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public required string Name { get; set; } = null!;

	public IReadOnlyList<DayTemplateEntity> DayTemplates => _dayTemplates.AsReadOnly();

	public bool TryAddDayTemplate(DayTemplateEntity dayTemplateToAdd)
	{
		if (_dayTemplates.Any(dayTemplate => dayTemplate.Order == dayTemplateToAdd.Order))
		{
			return false;
		}

		_dayTemplates.Add(dayTemplateToAdd);

		SortAndNormalizeDayTemplatesOrder();

		return true;
	}

	public bool RemoveDayTemplate(DayTemplateEntity dayTemplate)
	{
		if (!_dayTemplates.Remove(dayTemplate)) return false;

		SortAndNormalizeDayTemplatesOrder();

		return true;
	}

	public void ClearDayTemplates() => _dayTemplates.Clear();

	public bool TrySortAndNormalizeDayTemplatesOrder()
	{
		if (_dayTemplates.GroupBy(x => x.Order).Any(x => x.Count() > 1))
		{
			return false;
		}

		SortAndNormalizeDayTemplatesOrder();

		return true;
	}
	
	private void SortAndNormalizeDayTemplatesOrder()
	{
		_dayTemplates.Sort((x, y) => x.Order.CompareTo(y.Order));

		for (var i = 0; i < _dayTemplates.Count; i++)
		{
			const int orderStep = 100_000;

			var dayTemplate = _dayTemplates[i];
			dayTemplate.Order = i * orderStep;
		}
	}
}
