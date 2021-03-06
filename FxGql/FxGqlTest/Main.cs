using System;
using System.Collections.Generic;
using System.Linq;

namespace FxGqlTest
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			Console.WriteLine (DateTime.Now);
			Console.WriteLine ("{0} - {1} on {2}, OS: {3}, CPUcount: {4}",
			                   Environment.MachineName,
			                   Environment.Is64BitProcess ? "64bit" : "32bit",
			                   Environment.Is64BitOperatingSystem ? "64bit" : "32bit",
			                   Environment.OSVersion,
			                   Environment.ProcessorCount);
			Console.WriteLine ("{0} - {1}",
			                  Environment.GetEnvironmentVariable ("PROCESSOR_IDENTIFIER"),
			                  Environment.GetEnvironmentVariable ("PROCESSOR_REVISION"));
			using (GqlSamplesTest gqlSamplesTest = new GqlSamplesTest ()) {
				bool result = true;
				if (args.Length > 0 && args [0] == "develop") {
					result = gqlSamplesTest.RunDevelop ();
				} else if (args.Length > 0 && args [0] == "performance") {
					int count;
					if (args.Length <= 1 || !int.TryParse (args [1], out count))
						count = 1;
					var processorTimeList = new List<long> ();
					var stopwatchList = new List<long> ();
					var stopwatchParsingList = new List<long> ();
					var stopwatchExecutionList = new List<long> ();
					while (count-- > 0) {
						gqlSamplesTest.engineHash.Reset ();
						gqlSamplesTest.engineOutput.Reset ();

						gqlSamplesTest.Performance = true;
						TimeSpan processorTime = System.Diagnostics.Process.GetCurrentProcess ().TotalProcessorTime;
						System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew ();
						result = gqlSamplesTest.Run ();
						stopwatch.Stop ();
						processorTime = System.Diagnostics.Process.GetCurrentProcess ().TotalProcessorTime - processorTime;
						Console.WriteLine ("Elapsed: {0}, CPU: {1} ({2:0.00}%), {3}, {4}", 
						                   stopwatch.Elapsed, processorTime, 
						                   processorTime.Ticks * 100.0 / stopwatch.Elapsed.Ticks,
						                   gqlSamplesTest.engineHash.ParsingStopWatch.Elapsed,
						                   gqlSamplesTest.engineHash.ExecutionStopWatch.Elapsed);
						processorTimeList.Add (processorTime.Ticks);
						stopwatchList.Add (stopwatch.Elapsed.Ticks);
						stopwatchParsingList.Add (gqlSamplesTest.engineHash.ParsingStopWatch.Elapsed.Ticks);
						stopwatchExecutionList.Add (gqlSamplesTest.engineHash.ExecutionStopWatch.Elapsed.Ticks);
					}

					if (processorTimeList.Count > 1) {
						//        (F)       (S)
						//  orig skip take left
						//     1    0    1    0
						//     2    0    1    1
						//     3    1    1    1
						//     4    1    2    1
						//     5    1    2    2
						//     6    2    2    2
						int skip = stopwatchList.Count / 3;
						int take = (stopwatchList.Count + 2) / 3;
						stopwatchList = stopwatchList.OrderBy (p => p).Skip (skip).Take (take).ToList ();
						processorTimeList = processorTimeList.OrderBy (p => p).Skip (skip).Take (take).ToList ();
						stopwatchParsingList = stopwatchParsingList.OrderBy (p => p).Skip (skip).Take (take).ToList ();
						stopwatchExecutionList = stopwatchExecutionList.OrderBy (p => p).Skip (skip).Take (take).ToList ();
						TimeSpan stopwatch = new TimeSpan ((long)stopwatchList.Average ());
						TimeSpan processorTime = new TimeSpan ((long)processorTimeList.Average ());
						TimeSpan parsingTime = new TimeSpan ((long)stopwatchParsingList.Average ());
						TimeSpan executingTime = new TimeSpan ((long)stopwatchExecutionList.Average ());
						Console.WriteLine ("TOTAL  : {0}, CPU: {1} ({2:0.00}%), {3}, {4}", stopwatch, processorTime, 
						                   processorTime.Ticks * 100.0 / stopwatch.Ticks,
						                   parsingTime, executingTime);
					}
				} else {
					if (args.Length > 0 && args [0] == "breakonfailed") {
						gqlSamplesTest.BreakOnFailed = true;
					}					
					result = gqlSamplesTest.Run ();
				}

				var oldColor = Console.ForegroundColor;
				if (result) {
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine ("***** SUCCEEDED *****");
				} else {
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine ("***** FAILED *****");
				}
				Console.ForegroundColor = oldColor;

				return result ? 0 : 1;
			}
		}
	}
}
