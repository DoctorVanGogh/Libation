﻿using System;
using System.Collections.Generic;
using System.Linq;
using AudibleApiDTOs;
using DataLayer;

namespace DtoImporterService
{
	public class SeriesImporter : ItemsImporterBase
	{
		public override IEnumerable<Exception> Validate(IEnumerable<Item> items)
		{
			var exceptions = new List<Exception>();

			var distinct = items .GetSeriesDistinct();
			if (distinct.Any(s => s.SeriesId is null))
				exceptions.Add(new ArgumentException($"Collection contains {nameof(Item.Series)} with null {nameof(AudibleApiDTOs.Series.SeriesId)}", nameof(items)));
			if (distinct.Any(s => s.SeriesName is null))
				exceptions.Add(new ArgumentException($"Collection contains {nameof(Item.Series)} with null {nameof(AudibleApiDTOs.Series.SeriesName)}", nameof(items)));

			return exceptions;
		}

		protected override int DoImport(IEnumerable<Item> items, LibationContext context)
		{
			// get distinct
			var series = items.GetSeriesDistinct().ToList();

			// load db existing => .Local
			var seriesIds = series.Select(s => s.SeriesId).ToList();
			loadLocal_series(seriesIds, context);

			// upsert
			var qtyNew = upsertSeries(series, context);
			return qtyNew;
		}

		private void loadLocal_series(List<string> seriesIds, LibationContext context)
		{
			var localIds = context.Series.Local.Select(s => s.AudibleSeriesId);
			var remainingSeriesIds = seriesIds
				.Distinct()
				.Except(localIds)
				.ToList();

			if (remainingSeriesIds.Any())
				context.Series.Where(s => remainingSeriesIds.Contains(s.AudibleSeriesId)).ToList();
		}

		private int upsertSeries(List<AudibleApiDTOs.Series> requestedSeries, LibationContext context)
		{
			var qtyNew = 0;

			foreach (var s in requestedSeries)
			{
				var series = context.Series.Local.SingleOrDefault(c => c.AudibleSeriesId == s.SeriesId);
				if (series is null)
				{
					series = context.Series.Add(new DataLayer.Series(new AudibleSeriesId(s.SeriesId))).Entity;
					qtyNew++;
				}
				series.UpdateName(s.SeriesName);
			}

			return qtyNew;
		}
	}
}