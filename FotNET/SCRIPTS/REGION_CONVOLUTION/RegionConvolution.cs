using System.Drawing;
using FotNET.DATA.IMAGE;
using FotNET.NETWORK;
using FotNET.SCRIPTS.REGION_CONVOLUTION.SCRIPTS;

namespace FotNET.SCRIPTS.REGION_CONVOLUTION;

public static class RegionConvolution {
    public static Bitmap ForwardFeed(Bitmap bitmap, int cellSize, int stepsCount, Network model, 
        double minValue, int convolutionX, int convolutionY) {
        var graphics = Graphics.FromImage(bitmap);
        var pen      = new Pen(Color.FromKnownColor(KnownColor.Black), 1);
        
        var objects = RegionsMaker.GetRegions(bitmap, cellSize, stepsCount);

        foreach (var rectangle in objects) {
            var tensor = Parser.ImageToTensor(new Bitmap(bitmap.Clone(rectangle, bitmap.PixelFormat), 
                new Size(convolutionX, convolutionY)));
            
            var prediction = model.ForwardFeed(tensor, AnswerType.Class);
            var predictionValue = model.ForwardFeed(tensor, AnswerType.Value);
            if (predictionValue < minValue) continue;

            graphics.DrawRectangle(pen, rectangle);
            graphics.DrawString($"class: {prediction}\n{predictionValue}", 
                new Font("Tahoma", 8), Brushes.Black, rectangle.Location);
        }
        
        return bitmap;
    }
}