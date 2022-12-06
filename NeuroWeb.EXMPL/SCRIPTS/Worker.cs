using System.IO;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS
{
    public class Worker
    {
        public Data ReadNetworkConfig(string path)
        {
            var data           = new Data();
            var tempData = File.ReadAllText(path).Split("\n");

            for (var i = 0; i < tempData.Length; i++) {
                if (tempData[i] != "NetWork") continue;
                var layouts = int.Parse(tempData[i + 1]);
                    
                data.Layout = layouts;
                data.Size   = new int[layouts];
                
                for (var j = 0; j < layouts; j++)
                    data.Size[i] = int.Parse(tempData[i + 1 + j]);
                break;
            }

            return data;
        }

        public DataInformation[] ReadData()
        {
            DataInformation[] data = new DataInformation[0];

            return null;
        } 
    }

    public struct DataInformation {
        private double[] pixels;
        private int digit;
    }
}