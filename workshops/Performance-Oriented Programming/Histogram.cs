//Sample provided by Fabio Galuppo
//November 2021

using System;
using System.Collections.Generic;
using System.Diagnostics;
using HdrHistogram;

namespace PerformancePlayground
{
    public static class Histogram
    {
		private static void SaveToTemp(HistogramBase histogram, string filename)
        {
			//var temp = Environment.GetEnvironmentVariable("TEMP");
			var temp = @"C:\temp";
			using (var writer = new System.IO.StreamWriter(System.IO.Path.Combine(temp, filename)))
				histogram.OutputPercentileDistribution(writer);
		}

		private static void RunPing()
		{
			const string PING_ADDRESS = "www.google.com";
			Console.Write($"Ping Address [default = {PING_ADDRESS}]: ");
			string pingAddress = Console.ReadLine();
			if (String.IsNullOrEmpty(pingAddress)) pingAddress = PING_ADDRESS;

			int N = 25;
			Console.Write($"Number of tentatives [default = {N}]: ");
			string n = Console.ReadLine();
			if (Int32.TryParse(n, out int n_)) N = n_;

			var histogram = new LongHistogram(highestTrackableValue: TimeStamp.Minutes(5), numberOfSignificantValueDigits: 5);
			using (var ping = new System.Net.NetworkInformation.Ping())
			{
				Console.WriteLine($"Pinging [{pingAddress}] {N} time(s)");
				for (int i = 0; i < N; i++)
				{
					Console.Write("* ");
					var start = Stopwatch.GetTimestamp();
					{
						ping.Send(pingAddress);
					}
					var finish = Stopwatch.GetTimestamp();
					var elapsedTimestamp = finish - start;
					histogram.RecordValue(elapsedTimestamp);
				}
			}
			Console.WriteLine();
			Console.WriteLine();
			histogram.OutputPercentileDistribution(Console.Out, percentileTicksPerHalfDistance: 5, OutputScalingFactor.TimeStampToMilliseconds);
			SaveToTemp(histogram, "HistogramResults-Ping.hgrm");
		}

		private static void FillList(HistogramBase histogram, int N, int capacity = 0)
        {
			var list = capacity > 0 ? new List<int>(capacity) : new List<int>(); 
			for (int i = 0; i < N; i++)
			{
				var start = Stopwatch.GetTimestamp();
				list.Add(i + 1);
				var finish = Stopwatch.GetTimestamp();
				var elapsedTimestamp = finish - start;
				histogram.RecordValue(elapsedTimestamp);
			}
		}

		private static void FillArray(HistogramBase histogram, int N)
		{
			var array = new int[N];
			for (int i = 0; i < N; i++)
			{
				var start = Stopwatch.GetTimestamp();
				array[i] = i + 1;
				var finish = Stopwatch.GetTimestamp();
				var elapsedTimestamp = finish - start;
				histogram.RecordValue(elapsedTimestamp);
			}
		}

		private static void RunList()
        {
			int N = 1000000;
			
			var histogram1 = new LongHistogram(highestTrackableValue: TimeStamp.Minutes(1), numberOfSignificantValueDigits: 5);
			FillList(histogram1, N);
			var histogram2 = new LongHistogram(highestTrackableValue: TimeStamp.Minutes(1), numberOfSignificantValueDigits: 5);
			FillList(histogram2, N, 2);
			var histogram3 = new LongHistogram(highestTrackableValue: TimeStamp.Minutes(1), numberOfSignificantValueDigits: 5);
			FillList(histogram3, N, 1024);
			var histogram4 = new LongHistogram(highestTrackableValue: TimeStamp.Minutes(1), numberOfSignificantValueDigits: 5);
			FillList(histogram4, N, N);
			var histogram5 = new LongHistogram(highestTrackableValue: TimeStamp.Minutes(1), numberOfSignificantValueDigits: 5);
			FillArray(histogram5, N);

			Console.WriteLine("List DefaultCapacity:");
			histogram1.OutputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: OutputScalingFactor.TimeStampToMicroseconds);
			SaveToTemp(histogram1, "HistogramResults-Lists-DefaultCapacity.hgrm");

			Console.WriteLine("List Capacity2:");
			histogram2.OutputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: OutputScalingFactor.TimeStampToMicroseconds);
			SaveToTemp(histogram2, "HistogramResults-Lists-Capacity2.hgrm");

			Console.WriteLine("List Capacity1024:");
			histogram3.OutputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: OutputScalingFactor.TimeStampToMicroseconds);
			SaveToTemp(histogram3, "HistogramResults-Lists-Capacity1024.hgrm");

			Console.WriteLine("List CapacityN:");
			histogram4.OutputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: OutputScalingFactor.TimeStampToMicroseconds);
			SaveToTemp(histogram4, "HistogramResults-Lists-CapacityN.hgrm");

			Console.WriteLine("List ArrayN:");
			histogram5.OutputPercentileDistribution(Console.Out, outputValueUnitScalingRatio: OutputScalingFactor.TimeStampToMicroseconds);
			SaveToTemp(histogram5, "HistogramResults-Lists-ArrayN.hgrm");
		}

		public static void Run(string[] args)
        {
			//http://hdrhistogram.github.io/HdrHistogram/plotFiles.html
			//RunPing();
			RunList();
        }
    }
}
