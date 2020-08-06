using System.Collections.Generic;
using NirvanaCommon;
using Preloader;

namespace Benchmarks.Data
{
    public static class PreloadedData
    {
        public static readonly VcfPreloadData PedigreeData;
        public static readonly List<string> PedigreeLines;
        
        public static readonly VcfPreloadData TumorNormalData;
        public static readonly List<string> TumorNormalLines;

        static PreloadedData()
        {
            PedigreeLines = Preloader.Preloader.GetLines(Datasets.PedigreeTsvPath);
            PedigreeData  = Preloader.Preloader.GetPositions(PedigreeLines);

            // TumorNormalLines = Preloader.Preloader.GetLines(Datasets.TumorNormalTsvPath);
            // TumorNormalData  = Preloader.Preloader.GetPositions(TumorNormalLines);
        }
    }
}