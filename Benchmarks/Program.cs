using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // var bitArray = new BitArray1(1000);
            // bitArray.Set(1);
            // bitArray.Set(8);
            // bitArray.Set(9);
            // bitArray.Set(31);
            // bitArray.Set(32);
            // bitArray.Set(33);
            // var heaps = new Heaps();
            // heaps.Array();
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}