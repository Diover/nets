using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeuroNet.Model.Net
{
    public class BinaryFileSerializer
    {
 
        public static void SaveNetState(string filename, INet net)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, net);
            }
        }

        public static INet LoadNetState(string filename)
        {
            SimpleFuzzyNet result = null;
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bf = new BinaryFormatter();
                result = (SimpleFuzzyNet)bf.Deserialize(fs);
            }
            return result;
        }
    }
}