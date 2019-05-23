using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalPurchasing.Core.Interfaces.Analysis;

namespace DigitalPurchasing.Analysis.Pipelines
{
    public class DataPipeline : Pipeline<IEnumerable<IEnumerable<AnalysisResultData>>>
    {
        public override IEnumerable<IEnumerable<AnalysisResultData>> Process(IEnumerable<IEnumerable<AnalysisResultData>> input)
        {
            var sw = new Stopwatch();

            foreach (var filter in Filters)
            {
                Console.WriteLine($"----- {filter.GetType().Name} = start");
                sw.Restart();
                input = filter.Execute(input);
                sw.Stop();
                Console.WriteLine($"----- {filter.GetType().Name} = end   = {sw.ElapsedMilliseconds} ms");
            }

            return input;
        }
    }
}
