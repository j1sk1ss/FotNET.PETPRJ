using System.Collections.Concurrent;
using System.Drawing;

namespace FotNET.DATA.IMAGE.REGIONS.SCRIPTS;

public static class RegionsMaker {
    private static Bitmap _grayImage = null!;
    private static Bitmap _firstBlurred = null!;
    private static Bitmap _secondBlurred = null!;
    
    public static List<Rectangle> GetRegions(Bitmap bitmap, int minSize, int count) {
        var regions = DivideImageIntoRegions(bitmap, minSize);
        
        _grayImage = Grayscale(bitmap); 
        _firstBlurred = GaussianBlur(_grayImage, 5, 5, 1.0f); 
        _secondBlurred = GaussianBlur(_grayImage, 5, 5, 2.0f); 
        
        for (var i = 0; i < count; i++) { 
            var similarities = ComputeSimilarities(regions, bitmap); 
            var mostSimilar = similarities.MaxBy(s => s.Item3);
            if (mostSimilar == null) continue;
            
            var mergedRegion = MergeRegions(mostSimilar.Item1, mostSimilar.Item2); 
            regions.Remove(mostSimilar.Item1); 
            regions.Remove(mostSimilar.Item2); 
            
            regions.Add(mergedRegion); 
        }
        
        return regions; 
    } 

    private static List<Rectangle> DivideImageIntoRegions(Image image, int minSize) { 
        var regions   = new List<Rectangle>(); 
        var width  = image.Width; 
        var height = image.Height; 

        for (var y = 0; y < height; y += minSize) 
            for (var x = 0; x < width; x += minSize) 
                regions.Add(new Rectangle(x, y, 
                    Math.Min(minSize, width - x), Math.Min(minSize, height - y))); 
        
        return regions; 
    } 

    private static IEnumerable<Tuple<Rectangle, Rectangle, float>> ComputeSimilarities(IReadOnlyList<Rectangle> regions, Bitmap bitmap) { 
        var similarities = new ConcurrentQueue<Tuple<Rectangle, Rectangle, float>>();

        for (var i = 0; i < regions.Count; i++) 
            for (var j = i + 1; j < regions.Count; j++) 
                similarities.Enqueue(new Tuple<Rectangle, Rectangle, float>(regions[i], regions[j], 
                    ComputeSimilarity(regions[i], regions[j], bitmap)));
        
        return similarities;
    }
    
    private static float ComputeSimilarity(Rectangle firstRectangle, Rectangle secondRectangle, Bitmap bitmap) => 
        ComputeColorSimilarity(bitmap, firstRectangle, secondRectangle) * 
        ComputeTextureSimilarity(firstRectangle, secondRectangle) * 
        ComputeSizeSimilarity(firstRectangle, secondRectangle); 
    
    private static float ComputeColorSimilarity(Bitmap bitmap, Rectangle firstRectangle, Rectangle secondRectangle) =>
        ComputeColorDistance(ComputeAverageColor(bitmap.Clone(firstRectangle, bitmap.PixelFormat)), 
            ComputeAverageColor(bitmap.Clone(secondRectangle, bitmap.PixelFormat))); 
    
    private static float ComputeColorDistance(Color firstColor, Color secondColor) =>
         (float)Math.Sqrt(Math.Pow(firstColor.R - secondColor.R, 2) + Math.Pow(firstColor.G - secondColor.G, 2) +
                                Math.Pow(firstColor.B - secondColor.B, 2));

    private static float ComputeTextureSimilarity(Rectangle firstRectangle, Rectangle secondRectangle) {
        unsafe {
            var firstTexture  = GetTexture(firstRectangle);
            var secondTexture = GetTexture(secondRectangle);
            
            if (firstTexture.GetLength(0) != secondTexture.GetLength(0) ||
                firstTexture.GetLength(1) != secondTexture.GetLength(1)) 
                return 0;
            
            var sumOfSquaredDifferences = stackalloc float[Environment.ProcessorCount];
            for (var i = 0; i < Environment.ProcessorCount; i++) 
                sumOfSquaredDifferences[i] = 0.0f;
            
            Parallel.For(0, firstRectangle.Width, 
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, x => {
                    for (var y = 0; y < firstRectangle.Height; y++) 
                        sumOfSquaredDifferences[Environment.CurrentManagedThreadId % Environment.ProcessorCount] 
                            += (float)Math.Pow(firstTexture[x, y] - secondTexture[x, y], 2.0f);
                });
                
            var meanSquaredDifference = 0.0f;
            for (var i = 0; i < Environment.ProcessorCount; i++)
                meanSquaredDifference += sumOfSquaredDifferences[i];
            
            return 1.0f / (1.0f + meanSquaredDifference / firstRectangle.Width * firstRectangle.Height);
        }
    }
    
    private static float[,] GetTexture(Rectangle rectangle) {
        var texture = new float[rectangle.Width, rectangle.Height]; 
        
        for (var x = 0; x < rectangle.Width; x++) 
            for (var y = 0; y < rectangle.Height; y++) 
                texture[x, y] = _secondBlurred.GetPixel(x + rectangle.X, y + rectangle.Y).R 
                                - _firstBlurred.GetPixel(x + rectangle.X, y + rectangle.Y).R; 
        
        return texture; 
    } 

    private static float ComputeSizeSimilarity(Rectangle firstRectangle, Rectangle secondRectangle) { 
        var area1 = firstRectangle.Width * firstRectangle.Height; 
        var area2 = secondRectangle.Width * secondRectangle.Height; 
        var ratio = Math.Min(area1, area2) / Math.Max(area1, area2);
        return ratio; 
    } 

    private static Rectangle MergeRegions(Rectangle firstRectangle, Rectangle secondRectangle) {
        var x = Math.Min(firstRectangle.X, secondRectangle.X); 
        var y = Math.Min(firstRectangle.Y, secondRectangle.Y); 
        
        var width  = Math.Max(firstRectangle.X + firstRectangle.Width, secondRectangle.X + secondRectangle.Width) - x; 
        var height = Math.Max(firstRectangle.Y + firstRectangle.Height, secondRectangle.Y + secondRectangle.Height) - y; 
        
        return new Rectangle(x, y, width, height); 
    } 

    private static Color ComputeAverageColor(Bitmap bitmap) { 
        long red = 0, green = 0, blue = 0; 
        
        for (var x = 0; x < bitmap.Width; x++) 
            for (var y = 0; y < bitmap.Height; y++) { 
                var color = bitmap.GetPixel(x, y); 
                red   += color.R; 
                green += color.G; 
                blue  += color.B; 
            } 
        
        long numPixels = bitmap.Width * bitmap.Height; 
        
        var avgRed   = (byte)(red / numPixels); 
        var avgGreen = (byte)(green / numPixels); 
        var avgBlue  = (byte)(blue / numPixels); 
        
        return Color.FromArgb(avgRed, avgGreen, avgBlue); 
    } 

    private static Bitmap Grayscale(Bitmap bitmap) { 
        var grayImage = new Bitmap(bitmap.Width, bitmap.Height);
        
        for (var x = 0; x < bitmap.Width; x++)  
            for (var y = 0; y < bitmap.Height; y++) { 
                var color = bitmap.GetPixel(x, y); 
                var gray = (byte)(0.299f * color.R + 0.587f * color.G + 0.114f * color.B); 
                grayImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray)); 
            } 
         
        return grayImage; 
    } 

    private static Bitmap GaussianBlur(Bitmap image, int kernelSizeX, int kernelSizeY, float sigma) { 
        var radiusX = kernelSizeX / 2; 
        var radiusY = kernelSizeY / 2; 
        var kernel = ComputeGaussianKernel(kernelSizeX, kernelSizeY, sigma); 
        var blurredImage = new Bitmap(image.Width, image.Height); 
        
        for (var x = 0; x < image.Width; x++)  
            for (var y = 0; y < image.Height; y++) { 
                float red = 0, green = 0, blue = 0; 
                float weight = 0;
                
                for (var i = -radiusX; i <= radiusX; i++) 
                    for (var j = -radiusY; j <= radiusY; j++) {
                        if (x + i < 0 || x + i >= image.Width || y + j < 0 || y + j >= image.Height) continue;
                        
                        var color = image.GetPixel(x + i, y + j); 
                        var kernelValue = kernel[i + radiusX, j + radiusY]; 
                        
                        red   += kernelValue * color.R; 
                        green += kernelValue * color.G; 
                        blue  += kernelValue * color.B; 
                        
                        weight += kernelValue;
                    } 
                
                if (weight > 0) { 
                    red   /= weight; 
                    green /= weight; 
                    blue  /= weight; 
                } 
                
                red   = Math.Min(Math.Max(red, 0), 255); 
                green = Math.Min(Math.Max(green, 0), 255); 
                blue  = Math.Min(Math.Max(blue, 0), 255); 
                blurredImage.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue)); 
            } 
         
        return blurredImage; 
    } 

    private static float[,] ComputeGaussianKernel(int sizeX, int sizeY, float sigma) {
        var kernel = new float[sizeX, sizeY]; 
        var radiusX = sizeX / 2; 
        var radiusY = sizeY / 2; 
        var twoSigmaSquared = 2 * sigma * sigma; 
        var kernelValues = new ConcurrentBag<float>();
        
        Parallel.For(-radiusX, (long)radiusX, i => {
            for (var j = -radiusY; j <= radiusY; j++) {
                var distanceSquared = Math.Pow(i, 2) + Math.Pow(j, 2);
                kernel[i + radiusX, j + radiusY] = (float)Math.Exp(-distanceSquared / twoSigmaSquared) / (float)(Math.PI * twoSigmaSquared);
                kernelValues.Add(kernel[i + radiusX, j + radiusY]); 
            }
        });
        
        var sum = kernelValues.Sum();
        
        for (var i = 0; i < sizeX; i++)
            for (var j = 0; j < sizeY; j++)
                kernel[i, j] /= Volatile.Read(ref sum);

        return kernel;
    }
}