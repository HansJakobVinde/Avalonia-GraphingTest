using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

public class OxyPlotGraph
{
    public PlotModel PlotModel { get; private set; }

    public OxyPlotGraph()
    {
        PlotModel = new PlotModel { Title = "Real-Time Data Visualization" };
    }

    public void UpdateGraph(List<DataPoint> dataPoints)
    {
        var lineSeries = new LineSeries
        {
            Title = "Altitude Over Time",
            MarkerType = MarkerType.Circle,
            MarkerSize = 3,
            MarkerStroke = OxyColors.Red
        };

        lineSeries.Points.AddRange(dataPoints);
        PlotModel.Series.Clear();
        PlotModel.Series.Add(lineSeries);
        PlotModel.InvalidatePlot(true);
    }
}
