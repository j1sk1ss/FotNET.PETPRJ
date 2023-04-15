using System.Drawing;
using FotNET.DATA.DATA_OBJECTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.DATA.IMAGE;

public static class Parser {
    public static List<IData> FilesToImages(string directoryPath, string labelsPath) {
        var files = Directory.GetFiles(directoryPath);
        var images = new List<IData>();

        var labels = File.ReadAllText(labelsPath).Split("\n", StringSplitOptions.RemoveEmptyEntries);
        
        for (var i = 0; i < files.Length; i++) {
            var currentLabel = Array.ConvertAll(
                File.ReadAllText(labels[i]).Split(" ", StringSplitOptions.RemoveEmptyEntries), double.Parse);
            images.Add(new DATA_OBJECTS.IMAGE.Image(ImageToArray(files[i]), currentLabel));
        }

        return images;
    }

    public static double[,,] ImageToArray(string path) {
        var bitmap = (Bitmap)Image.FromFile(path);
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

    public static Tensor ImageToTensor(string path) {
        var bitmap = (Bitmap)Image.FromFile(path);
        var tensor = new Tensor(new List<Matrix>());

        for (var depth = 0; depth < 3; depth++) {
            tensor.Channels.Add(new Matrix(bitmap.Height, bitmap.Width));
            for (var i = 0; i < bitmap.Height; i++)
                for (var j = 0; j < bitmap.Width; j++) {
                    tensor.Channels[^1].Body[i,j] = depth switch {
                        0 => bitmap.GetPixel(i, j).R,
                        1 => bitmap.GetPixel(i, j).G,
                        2 => bitmap.GetPixel(i, j).B,
                        _ => 0
                    } / 255d;
                }
        }

        return tensor;
    }

    public static Tensor ImageToTensor(Bitmap bitmap) {
        var tensor = new Tensor(new List<Matrix>());
        
        for (var depth = 0; depth < 3; depth++) {
            tensor.Channels.Add(new Matrix(bitmap.Height, bitmap.Width));
            for (var i = 0; i < bitmap.Height; i++)
                for (var j = 0; j < bitmap.Width; j++) {
                    tensor.Channels[^1].Body[i,j] = depth switch {
                        0 => bitmap.GetPixel(j, i).R,
                        1 => bitmap.GetPixel(j, i).G,
                        2 => bitmap.GetPixel(j, i).B,
                        _ => 0
                    } / 255d;
                }
        }

        return tensor;
    }
    
    public static Bitmap TensorToImage(Tensor tensor) {
        var height = tensor.Channels[0].Rows;
        var width = tensor.Channels[0].Columns;
        
        var image = new Bitmap(width, height);
        
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var pixelColor = Color.FromArgb(
                    (int)tensor.Channels[0].Body[y, x],
                    (int)tensor.Channels[1].Body[y, x],
                    (int)tensor.Channels[2].Body[y, x] 
                );
                image.SetPixel(x, y, pixelColor);
            }
        }
        
        return image;
    }
}