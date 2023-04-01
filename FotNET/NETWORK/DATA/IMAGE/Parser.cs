using System.Drawing;

namespace FotNET.NETWORK.DATA.IMAGE;

public class Parser {
    public double[,,] ImageToArray(string path) {
        var bitmap = new Bitmap(path);
        var array = new double[bitmap.Height, bitmap.Width, 3];
        
        for (var depth = 0; depth < 3; depth++)
            for (var i = 0; i < bitmap.Height; i++)
                for (var j = 0; j < bitmap.Width; j++)
                    array[i, j, depth] = depth switch {
                        0 => bitmap.GetPixel(i, j).R,
                        1 => bitmap.GetPixel(i, j).G,
                        2 => bitmap.GetPixel(i, j).B,
                        _ => 0
                    } / 255d;

        return array;
    }
}