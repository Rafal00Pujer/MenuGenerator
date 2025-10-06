using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MenuGenerator.Models.Entities.MenuTemplate;

[EntityTypeConfiguration<MenuTemplateEntityConfiguration, MenuTemplateEntity>]
public class MenuTemplateEntity
{
	private readonly List<DayTemplateEntity> _dayTemplates = [];
	private readonly List<int> _dayTemplateIds = [];

	public required Guid Id { get; set; } = Guid.CreateVersion7();

	public required string Name { get; set; } = null!;

	public IReadOnlyList<DayTemplateEntity> DayTemplates => _dayTemplates.AsReadOnly();

	public IReadOnlyList<int> DayTemplateIds => _dayTemplateIds.AsReadOnly();

	public bool TryAddDayTemplate(DayTemplateEntity dayTemplateToAdd)
	{
		if (_dayTemplates.Any(dayTemplate => dayTemplate.Order == dayTemplateToAdd.Order))
		{
			return false;
		}

		_dayTemplates.Add(dayTemplateToAdd);

		// TODO - Uncomment after testing
		//ReorderDayTemplates();

		return true;
	}

	public bool RemoveDayTemplate(DayTemplateEntity dayTemplate)
	{
		if (!_dayTemplates.Remove(dayTemplate)) return false;

		ReorderDayTemplates();

		return true;
	}

	public void ClearDayTemplates()
	{
		_dayTemplates.Clear();
		_dayTemplateIds.Clear();
	}

	public void ReorderDayTemplates()
	{
		if (_dayTemplates.Count == 0)
		{
			return;
		}

		SortAndNormalizeDayTemplatesOrder();

		_dayTemplateIds.Clear();

		// this dictionary will store the current flags value for every template with more than one day
		var templatesWithMoreThanOneDayLeft = new Dictionary<DayTemplateEntity, TemplateDayFlag>(_dayTemplates.Count);
		var currentTemplatesToCreateIndexQueue = new Queue<DayTemplateEntity>(_dayTemplates);
		var nextTemplatesToCreateIndexQueue = new Queue<DayTemplateEntity>(currentTemplatesToCreateIndexQueue.Count);
		var currentDayFlag = TemplateDayFlag.FirstFlag;

		do
		{
			while (currentTemplatesToCreateIndexQueue.Count > 0)
			{
				var currentTemplate = currentTemplatesToCreateIndexQueue.Dequeue();

				var currentDayFlags = GetCurrentDayFlagsToCheck(currentTemplate);

				if (currentDayFlags.HasFlag(currentDayFlag))
				{
					var currentTemplateIndex = _dayTemplates.IndexOf(currentTemplate);
					_dayTemplateIds.Add(currentTemplateIndex);

					currentDayFlags = (int)currentDayFlags - currentDayFlag;
					
					IncrementCurrentDayFlag();
				}

				EnqueueIfAnyFlagLeft(currentDayFlags, currentTemplate);
			}

			currentTemplatesToCreateIndexQueue = nextTemplatesToCreateIndexQueue;
			nextTemplatesToCreateIndexQueue = new Queue<DayTemplateEntity>(currentTemplatesToCreateIndexQueue.Count);
		}
		while (currentTemplatesToCreateIndexQueue.Count > 0);

		return;

		void IncrementCurrentDayFlag()
		{
			currentDayFlag = (TemplateDayFlag)((int)currentDayFlag << 1);

			if (currentDayFlag > TemplateDayFlag.LastFlag)
			{
				currentDayFlag = TemplateDayFlag.FirstFlag;
			}
		}

		void EnqueueIfAnyFlagLeft(TemplateDayFlag currentDayFlags, DayTemplateEntity currentTemplate)
		{
			if (currentDayFlags <= TemplateDayFlag.None)
			{
				templatesWithMoreThanOneDayLeft.Remove(currentTemplate);

				return;
			}

			// this will also add day template to the dictionary if not added before
			templatesWithMoreThanOneDayLeft[currentTemplate] = currentDayFlags;

			nextTemplatesToCreateIndexQueue.Enqueue(currentTemplate);
		}

		TemplateDayFlag GetCurrentDayFlagsToCheck(DayTemplateEntity currentTemplateToCheck)
		{
			if (!templatesWithMoreThanOneDayLeft.TryGetValue
					(currentTemplateToCheck, out var currentTemplateDayFlagsToCheck))
			{
				currentTemplateDayFlagsToCheck = currentTemplateToCheck.TemplateDays;
			}

			return currentTemplateDayFlagsToCheck;
		}
	}

	private void SortAndNormalizeDayTemplatesOrder()
	{
		if (_dayTemplates.GroupBy(x => x.Order).Any(x => x.Count() > 1))
		{
			throw new InvalidOperationException("Day templates with the same order are not allowed.");
		}

		_dayTemplates.Sort((x, y) => x.Order.CompareTo(y.Order));

		for (var i = 0; i < _dayTemplates.Count; i++)
		{
			const int orderStep = 100_000;

			var dayTemplate = _dayTemplates[i];
			dayTemplate.Order = i * orderStep;
		}
	}
}
