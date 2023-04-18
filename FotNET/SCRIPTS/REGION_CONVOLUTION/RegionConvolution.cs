using System.Drawing;

using FotNET.DATA.IMAGE;
using FotNET.NETWORK;
using FotNET.SCRIPTS.REGION_CONVOLUTION.SCRIPTS;

namespace FotNET.SCRIPTS.REGION_CONVOLUTION;

/// <summary>
/// R-CNN Model
/// </summary>
public static class RegionConvolution {
    /// <summary>
    /// Forward pass during R-CNN model
    /// </summary>
    /// <param name="bitmap"> Input image </param>
    /// <param name="cellSize"> Min size of region </param>
    /// <param name="stepsCount"> Count of calculation epochs </param>
    /// <param name="model"> Pre-trained (Or default) CNN model </param>
    /// <param name="minValue"> Min value of prediction </param>
    /// <param name="convolutionX"> Resize of region to fit CNN </param>
    /// <param name="convolutionY"> Resize of region to fit CNN </param>
    /// <returns> Returns image with selected objects </returns>
    public static Bitmap ForwardFeed(Bitmap bitmap, int cellSize, int stepsCount, Network model, 
        double minValue, int convolutionX, int convolutionY) {
        var graphics = Graphics.FromImage(bitmap);
        var pen      = new Pen(Color.FromKnownColor(KnownColor.Black), 1);
        
        var objects = RegionsMaker.GetRegions(bitmap, cellSize, stepsCount, .4);
        
        foreach (var rectangle in objects) {
            var tensor = Parser.ImageToTensor(new Bitmap(bitmap.Clone(rectangle, bitmap.PixelFormat), 
                new Size(convolutionX, convolutionY)));
            
            var prediction = model.ForwardFeed(tensor, AnswerType.Class);
            var predictionValue = model.ForwardFeed(tensor, AnswerType.Value);
            if (predictionValue < minValue) continue;

            graphics.DrawRectangle(pen, rectangle);
            graphics.DrawString($"class: {prediction}\nValue: {Math.Round(predictionValue, 3)}", 
                new Font("Tahoma", 8), Brushes.Black, rectangle.Location);
        }
        
        return bitmap;
    }
    
    /// <summary>
    /// Forward pass during R-CNN model
    /// </summary>
    /// <param name="bitmap"> Input image </param>
    /// <param name="cellSize"> Min size of region </param>
    /// <param name="stepsCount"> Count of calculation epochs </param>
    /// <param name="model"> Pre-trained (Or default) CNN model </param>
    /// <param name="minValue"> Min value of prediction </param>
    /// <param name="similarityValue"> Value of similarity, that shows when regions unite </param>
    /// <param name="expectedClass"> Class that should be selected </param>
    /// <param name="convolutionX"> Resize of region to fit CNN </param>
    /// <param name="convolutionY"> Resize of region to fit CNN </param>
    /// <returns> Returns image with selected objects </returns>
    public static Bitmap ForwardFeed(Bitmap bitmap, int cellSize, int stepsCount, Network model, 
        double minValue, double similarityValue, int expectedClass, int convolutionX, int convolutionY) {
        var graphics = Graphics.FromImage(bitmap);
        var pen      = new Pen(Color.FromKnownColor(KnownColor.Black), 1);
        
        var objects = RegionsMaker.GetRegions(bitmap, cellSize, stepsCount, similarityValue);

        foreach (var rectangle in objects) {
            var tensor = Parser.ImageToTensor(new Bitmap(bitmap.Clone(rectangle, bitmap.PixelFormat), 
                new Size(convolutionX, convolutionY)));
            
            var prediction = model.ForwardFeed(tensor, AnswerType.Class);
            var predictionValue = model.ForwardFeed(tensor, AnswerType.Value);
            
            if (predictionValue < minValue) continue;
            if (Math.Abs(expectedClass - prediction) > .2d) continue;

            graphics.DrawRectangle(pen, rectangle);
            graphics.DrawString($"class: {prediction}\nValue: {Math.Round(predictionValue, 3)}", 
                new Font("Tahoma", 8), Brushes.Black, rectangle.Location);
        }
        
        return bitmap;
    }
}