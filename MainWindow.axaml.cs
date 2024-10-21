using Avalonia.Controls;
using OxyPlot;
using OxyPlot.Avalonia;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphingTest {
    
public partial class MainWindow : Window
{
    private List<DataPoint> dataPoints = new List<DataPoint>();
    private PlotModel plotModel = new PlotModel { Title = "Real-Time Data Visualization" };
    private OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries { Title = "Altitude" };
    private WebSocketHandler webSocketHandler;

    private DateTime _lastUpdate = DateTime.Now;

    public MainWindow()
    {
        InitializeComponent();
        InitializePlot();
        StartWebSocketConnection();
    }

    private void InitializePlot()
    {
        lineSeries.Color = OxyColors.SkyBlue;
        plotModel.Series.Add(lineSeries);
        PlotView.Model = plotModel;
    }

    private async void StartWebSocketConnection()
    {
        string url = "ws://localhost:8080/ws/datastream";
        webSocketHandler = new WebSocketHandler(url);
        webSocketHandler.OnDataReceived += OnDataReceived;

        // Start the WebSocket connection
        await webSocketHandler.Start();
    }

    private void OnDataReceived(DataMessage dataMessage)
    {
        Console.WriteLine("Received DataMessage:");
        foreach (var key in dataMessage.Data.Keys)
        {
            Console.WriteLine($"Key: {key}, Value: {dataMessage.Data[key]}");
        }

        if (dataMessage.Data.TryGetValue("vcu/114.InsEstimates1.pitch", out var altitude))
        {
            double alt = 0;
            if (altitude is JsonElement jsonElement && jsonElement.TryGetDouble(out double value))
            {
                alt = value;
            }
            else
            {
                Console.WriteLine("Invalid altitude value.");
                return;
            }

            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(dataMessage.Timestamp)
                .UtcDateTime.ToOADate();
            dataPoints.Add(new DataPoint(timestamp, alt));

            // Update the graph on the UI thread
            if ((DateTime.Now - _lastUpdate).TotalMilliseconds > 100)
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(UpdateGraph);
                _lastUpdate = DateTime.Now;
            }
        }
    }

    private void UpdateGraph()
    {
        lineSeries.Points.Clear();
        lineSeries.Points.AddRange(dataPoints);
        plotModel.InvalidatePlot(true);
    }
}

}