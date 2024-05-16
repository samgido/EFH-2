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
		}

		public void AddPlot(HydrographLineModel model)
		{
			LineSeries series = new LineSeries()
			{
				Title = model.Frequency + "-Yr",
			};

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
