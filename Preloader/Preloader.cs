using System.Collections.Generic;
using System.IO;

namespace Preloader
{
    public static class Preloader
    {
        public static List<int> GetPositions(string tsvPath)
        {
            var positions = new List<int>();

            using (var fileStream = new FileStream(tsvPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    positions.Add(int.Parse(line));
                }
            }

            return positions;
        }
    }
}