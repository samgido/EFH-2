using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace EFH2
{
	public partial class HydrographDataViewModel : ObservableObject
	{
		private LinearAxis verticalGridLines = new LinearAxis()
		{
			Position = AxisPosition.Bottom,
			MajorGridlineStyle = LineStyle.Dash,
			MinorGridlineStyle = LineStyle.Solid,
			MaximumPadding = 0,
			MinimumPadding = 0,
			TickStyle = TickStyle.None,
			LabelFormatter = x => "",
		};

		private LinearAxis horizontalGridLines = new LinearAxis()
		{
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.Dash,
			MinorGridlineStyle = LineStyle.Solid,
			MaximumPadding = 0,
			MinimumPadding = 0,
			TickStyle = TickStyle.None,
			LabelFormatter = x => "",
		};

		private PlotModel _model;

		public PlotModel Model => _model;

		public HydrographDataViewModel(string county, string state)
		{
			// county string has the rftype appended to it, need to remove
			string[] splitCounty = county.Split(" ");
			if (splitCounty.Length > 1) county = county.Replace(splitCounty[splitCounty.Length - 1], "");

			_model = new PlotModel()
			{
				Title = "Hydrographs",
				Subtitle = county + " COUNTY, " + state,
			};

			// make legend
			_model.Legends.Add(new Legend());

			// add axes 
			LinearAxis axis = new LinearAxis();

			_model.Axes.Add(new LinearAxis() { Title = "Discharge (cfs)" });
			_model.Axes.Add(new LinearAxis() { Title = "Time (hrs)", Position = AxisPosition.Bottom });
			_model.Axes.Add(verticalGridLines);
			_model.Axes.Add(horizontalGridLines);
		}

		public void AddPlot(HydrographLineModel model)
		{
			LineSeries series = new LineSeries()
			{
				Title = model.Frequency + "-Yr",
				TrackerFormatString = "{0}\n{1}: {2:0.00}\n{3}: {4:0.00}",
			};

			for (int i = 0; i < model.Values.Count; i++)
			{
				double x = model.Start + i * model.Increment;
				double y = model.Values[i];

				series.Points.Add(new DataPoint(x, y));
			}

			_model.Series.Add(series);
		}

		public void ChangeSettings(bool showMarkers, bool showLines, bool showGrid)
		{
			for (int i = 0; i < _model.Series.Count; i++) 
			{
				if (_model.Series[i] is LineSeries series)
				{
					if (showMarkers)
					{
						int index = (i % 7) + 1;
						object? markerObject = Enum.GetValues(typeof(MarkerType)).GetValue(index);
						if (markerObject != null && markerObject is MarkerType markerType)
						{
							series.MarkerType = markerType;
						}
					}
					else
					{
						series.MarkerType = MarkerType.None;
					}

					if (showLines) series.StrokeThickness = 2.0;
					else series.StrokeThickness = 0;

					if (showGrid)
					{
						if (!_model.Axes.Contains(verticalGridLines))
						{
							_model.Axes.Add(verticalGridLines);
							_model.Axes.Add(horizontalGridLines);
						}
					}
					else
					{
						_model.Axes.Remove(verticalGridLines);
						_model.Axes.Remove(horizontalGridLines);
					}
				}
			}
		}
	}
}
