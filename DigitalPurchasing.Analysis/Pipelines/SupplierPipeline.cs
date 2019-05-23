using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DigitalPurchasing.Analysis.Pipelines
{
    public class SupplierPipeline : Pipeline<IEnumerable<AnalysisSupplier>>
    {
        public override IEnumerable<AnalysisSupplier> Process(IEnumerable<AnalysisSupplier> input)
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
