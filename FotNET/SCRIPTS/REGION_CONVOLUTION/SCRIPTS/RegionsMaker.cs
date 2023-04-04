using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;

namespace FotNET.SCRIPTS.REGION_CONVOLUTION.SCRIPTS;

public static class RegionsMaker {
    private static Bitmap _grayImage     = null!;
    private static Bitmap _firstBlurred  = null!;
    private static Bitmap _secondBlurred = null!;
    
    public static List<Rectangle> GetRegions(Bitmap bitmap, int defaultRectangleSize, int stepsCount) {
        var regions = RectanglesFromBitmap(bitmap, defaultRectangleSize);
        
        _grayImage     = Grayscale(bitmap); 
        _firstBlurred  = GaussianBlur(_grayImage, 5, 5, 1.0f); 
        _secondBlurred = GaussianBlur(_grayImage, 5, 5, 2.0f); 
        
        for (var i = 0; i < stepsCount; i++) { 
            var similar = Similarities(regions, bitmap).MaxBy(s => s.Item3);
            if (similar == null) continue;
            
            regions.Remove(similar.Item1); 
            regions.Remove(similar.Item2); 
            
            regions.Add(MergeRegions(similar.Item1, similar.Item2)); 
        }
        
        return regions; 
    } 

    private static List<Rectangle> RectanglesFromBitmap(Image image, int minSize) {
        var width  = image.Width; 
        var height = image.Height; 

        var regions   = new List<Rectangle>(); 
        for (var y = 0; y < height; y += minSize) 
            for (var x = 0; x < width; x += minSize) 
                regions.Add(new Rectangle(x, y, Math.Min(minSize, width - x), Math.Min(minSize, height - y))); 
        
        return regions; 
    } 

    private static IEnumerable<Tuple<Rectangle, Rectangle, float>> Similarities(IReadOnlyList<Rectangle> regions, Bitmap bitmap) {
        var similarities = new ConcurrentQueue<Tuple<Rectangle, Rectangle, float>>();

        var bitmapLock = new object();
        Parallel.ForEach(Partitioner.Create(0, regions.Count), range => {
            for (var i = range.Item1; i < range.Item2; i++) 
                for (var j = i + 1; j < regions.Count; j++) 
                    lock (bitmapLock) {
                        var bitmapCopy = (Bitmap)bitmap.Clone();
                        similarities.Enqueue(new Tuple<Rectangle, Rectangle, float>(regions[i], regions[j],
                            Similarity(regions[i], regions[j], bitmapCopy)));
                    }
        });

        return similarities;
    }

    
    private static float Similarity(Rectangle firstRectangle, Rectangle secondRectangle, Bitmap bitmap) => 
        ColorSimilarity(bitmap, firstRectangle, secondRectangle) * 
        TextureSimilarity(firstRectangle, secondRectangle) * 
        Similarity(firstRectangle, secondRectangle); 
    
    private static float ColorSimilarity(Bitmap bitmap, Rectangle firstRectangle, Rectangle secondRectangle) =>
        ColorDistance(AverageColor(bitmap.Clone(firstRectangle, bitmap.PixelFormat)), 
            AverageColor(bitmap.Clone(secondRectangle, bitmap.PixelFormat))); 
    
    private static float ColorDistance(Color firstColor, Color secondColor) =>
         (float)Math.Sqrt(Math.Pow(firstColor.R - secondColor.R, 2) + Math.Pow(firstColor.G - secondColor.G, 2) +
                                Math.Pow(firstColor.B - secondColor.B, 2));

    private static float TextureSimilarity(Rectangle firstRectangle, Rectangle secondRectangle) {
        unsafe {
            var firstTexture  = Texture(firstRectangle);
            var secondTexture = Texture(secondRectangle);

            var sumOfSquaredDifferences = stackalloc float[Environment.ProcessorCount];
            for (var i = 0; i < Environment.ProcessorCount; i++) 
                sumOfSquaredDifferences[i] = 0.0f;
            
            Parallel.For(0, Math.Min(firstRectangle.Width, secondRectangle.Width), 
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, x => {
                    for (var y = 0; y < Math.Min(firstRectangle.Height, secondRectangle.Height); y++) 
                        sumOfSquaredDifferences[Environment.CurrentManagedThreadId % Environment.ProcessorCount] 
                            += (float)Math.Pow(firstTexture[x, y] - secondTexture[x, y], 2.0f);
                });
                
            var meanSquaredDifference = 0.0f;
            for (var i = 0; i < Environment.ProcessorCount; i++)
                meanSquaredDifference += sumOfSquaredDifferences[i];
            
            return 1.0f / (1.0f + meanSquaredDifference / firstRectangle.Width * firstRectangle.Height);
        }
    }
    
    private static float[,] Texture(Rectangle rectangle) {
        var texture = new float[rectangle.Width, rectangle.Height]; 
        
        for (var x = 0; x < rectangle.Width; x++) 
            for (var y = 0; y < rectangle.Height; y++) 
                texture[x, y] = _secondBlurred.GetPixel(x + rectangle.X, y + rectangle.Y).R 
                                - _firstBlurred.GetPixel(x + rectangle.X, y + rectangle.Y).R; 
        
        return texture; 
    } 

    private static float Similarity(Rectangle firstRectangle, Rectangle secondRectangle) { 
        var firstArea = firstRectangle.Width * firstRectangle.Height; 
        var secondArea = secondRectangle.Width * secondRectangle.Height; 
        
        var ratio = Math.Min(firstArea, secondArea) / Math.Max(firstArea, secondArea);
        return ratio; 
    } 

    private static Rectangle MergeRegions(Rectangle firstRectangle, Rectangle secondRectangle) {
        var x = Math.Min(firstRectangle.X, secondRectangle.X); 
        var y = Math.Min(firstRectangle.Y, secondRectangle.Y); 
        
        var width  = Math.Max(firstRectangle.X + firstRectangle.Width, secondRectangle.X + secondRectangle.Width) - x; 
        var height = Math.Max(firstRectangle.Y + firstRectangle.Height, secondRectangle.Y + secondRectangle.Height) - y; 
        
        return new Rectangle(x, y, width, height); 
    } 

    private static Color AverageColor(Bitmap bitmap) {
        var width  = bitmap.Width;
        var height = bitmap.Height;

        const int minDiversion = 15; 
        var dropped = 0; 
        
        long[] totals = { 0, 0, 0 };
        var bppModifier = bitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

        var srcData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        var stride = srcData.Stride;
        var scan = srcData.Scan0;

        unsafe {
            var scanned = (byte*)(void*)scan;

            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    var idx = (y * stride) + x * bppModifier;
                    
                    var red   = scanned[idx + 2];
                    var green = scanned[idx + 1];
                    var blue  = scanned[idx];
                    
                    if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion) {
                        totals[2] += red;
                        totals[1] += green;
                        totals[0] += blue;
                    }
                    else dropped++;
                }
            }
        }

        var count = width * height - dropped;
        return Color.FromArgb((int)(totals[2] / count), (int)(totals[1] / count), (int)(totals[0] / count)); 
    } 

    private static Bitmap Grayscale(Bitmap bitmap) { 
        var grayImage = new Bitmap(bitmap.Width, bitmap.Height);
        
        for (var x = 0; x < bitmap.Width; x++)  
            for (var y = 0; y < bitmap.Height; y++) { 
                var color = bitmap.GetPixel(x, y); 
                var gray = (byte)(.299f * color.R + .587f * color.G + .114f * color.B); 
                grayImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray)); 
            } 
         
        return grayImage; 
    } 

    private static Bitmap GaussianBlur(Bitmap image, int kernelSizeX, int kernelSizeY, float sigma) { 
        var radiusX = kernelSizeX / 2; 
        var radiusY = kernelSizeY / 2; 
        
        var kernel = GaussianKernel(kernelSizeX, kernelSizeY, sigma); 
        
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

    private static float[,] GaussianKernel(int sizeX, int sizeY, float sigma) {
        var kernel = new float[sizeX, sizeY]; 
        
        var radiusX = sizeX / 2; 
        var radiusY = sizeY / 2; 
        
        var twoSigmaSquared = 2 * Math.Pow(sigma, 2); 
        
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