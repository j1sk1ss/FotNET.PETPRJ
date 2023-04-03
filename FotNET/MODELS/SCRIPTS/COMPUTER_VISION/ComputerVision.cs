using System.Drawing;
using FotNET.DATA.IMAGE;
using FotNET.DATA.IMAGE.REGIONS.SCRIPTS;
using FotNET.NETWORK;

namespace FotNET.MODELS.SCRIPTS.COMPUTER_VISION;

public static class ComputerVision {
    public static Bitmap Calculate(Bitmap bitmap, Network model, double minValue, int convolutionX, int convolutionY) {
        var graphics = Graphics.FromImage(bitmap);
        var pen = new Pen(Color.FromKnownColor(KnownColor.Black), 1);
        
        var objects = RegionsMaker.GetRegions(bitmap, 50, 3);

        for (var i = 0; i < objects.Count; i++) {
            var tensor = Parser.ImageToTensor(new Bitmap(bitmap.Clone(objects[i], bitmap.PixelFormat), 
                new Size(convolutionX, convolutionY)));
            
            var prediction = model.ForwardFeed(tensor, AnswerType.Class);
            var predictionValue = model.ForwardFeed(tensor, AnswerType.Value);
            if (predictionValue < minValue) continue;

            graphics.DrawRectangle(pen, objects[i]);
            graphics.DrawString($"class: {prediction}", new Font("Tahoma", 8), Brushes.Black, objects[i].Location);
        }
        
        return bitmap;
    }
}