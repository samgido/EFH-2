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

namespace EFH2
{
	public partial class HydrographDataViewModel : ObservableObject
	{
		private PlotModel _model;

		public PlotModel Model => _model;

		public HydrographDataViewModel(string county, string state)
		{
			_model = new PlotModel()
			{
				Title = "Hydrographs",
				Subtitle = county + " COUNTY, " + state,
				IsLegendVisible = true,
			};
		}

		public void SetCountyAndState(string county, string state)
		{
			_model.Title = "Hydrographs  " +
				county + " COUNTY, " + state;
		}

		public void AddPlot(HydrographLineModel model)
		{
			LineSeries series = new LineSeries();

			for (int i = 0; i < model.Values.Count; i++)
			{
				double x = model.Start + i * model.Increment;
				double y = model.Values[i];

				series.Points.Add(new DataPoint(x, y));
			}

			_model.Series.Add(series);
		}
	}
}
