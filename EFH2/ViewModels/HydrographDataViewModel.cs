using System;
using System.Collections.Generic;
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
		private PlotModel _model = new PlotModel();

		public PlotModel Model => _model;

		public void AddPlot(HydrographLineModel model, MarkerType marker, OxyColor color)
		{
			LineSeries series = new LineSeries()
			{
				// these should be automatic, not sure yet
				//MarkerType = marker, 
				//MarkerStroke = color,
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
