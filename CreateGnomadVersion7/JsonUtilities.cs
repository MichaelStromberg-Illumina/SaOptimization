using System;
using System.IO;
using Newtonsoft.Json;
using NirvanaCommon;
using Version7.Data;

namespace CreateGnomadVersion7
{
    public static class JsonUtilities
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
        
        public static SaEntry Deserialize(string line)
        {
            SaEntry entry = null;
            string  json  = null;
            
            try
            {
                string[] cols = line.Split('\t');
                if (cols.Length != 6)
                    throw new InvalidDataException($"Found an invalid number of columns: {cols.Length}");

                int    position  = int.Parse(cols[0]);
                string refAllele = cols[1];
                string altAllele = cols[2];

                json = string.Create(cols[5].Length + 2, cols[5], (chars, state) =>
                {
                    var index = 0;
                    chars[index++] = '{';

                    state.AsSpan().CopyTo(chars.Slice(1));
                    index += state.Length;

                    chars[index] = '}';
                });
                
                VariantType variantType = VariantTypeUtilities.GetVariantType(refAllele, altAllele);
                string      allele      = variantType == VariantType.deletion ? refAllele : altAllele;

                ulong positionAllele = PositionAllele.Convert(position, allele, variantType);
                var   gnomAD         = JsonConvert.DeserializeObject<GnomadEntry>(json, settings);

                entry = new SaEntry(position, positionAllele, gnomAD);
            }
            catch (Exception e)
            {
                string prettyJson = JsonPrettify(json);
                Console.WriteLine($"ERROR: {e.Message}\n{prettyJson}");
                Environment.Exit(1);
            }
            
            return entry;
        }

        private static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }
    }
}