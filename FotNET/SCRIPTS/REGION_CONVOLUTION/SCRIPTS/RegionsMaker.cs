using System.Drawing;
using System.Drawing.Imaging;

namespace FotNET.SCRIPTS.REGION_CONVOLUTION.SCRIPTS;

public static class RegionsMaker {
    public static List<Rectangle> GetRegions(Bitmap bitmap, int defaultRectangleSize, int stepsCount) {
        var regions = RectanglesFromBitmap(bitmap, defaultRectangleSize);
        
        for (var i = 0; i < stepsCount; i++) {
            var similar = Similarities(regions, bitmap);
                regions.Clear();
                
            for (var j = 0; j < similar.Count; j++) {
                if (similar[j].Count <= 0) continue;
                var region = similar[j][0].Item1;
                
                foreach (var neighbor in similar[j]) {
                    Console.WriteLine(neighbor.Item3);
                    if (neighbor.Item3 <= 50) continue;
                    region = MergeRegions(region, neighbor.Item2);
                }
                
                regions.Add(region);
            }
        }

        for (var i = 0; i < regions.Count; i++) 
            for (var j = i + 1; j < regions.Count; j++) 
                if (regions[i] == regions[j]) regions.RemoveAt(j);
        
        return regions; 
    } 

    private static List<Rectangle> RectanglesFromBitmap(Image image, int minSize) {
        var width  = image.Width; 
        var height = image.Height; 

        var regions = new List<Rectangle>(); 
        for (var y = 0; y < height; y += minSize) 
            for (var x = 0; x < width; x += minSize) 
                regions.Add(new Rectangle(x, y, Math.Min(minSize, width - x), Math.Min(minSize, height - y))); 
        
        return regions; 
    } 

    private static List<List<Tuple<Rectangle, Rectangle, float>>> Similarities(IReadOnlyList<Rectangle> regions, Bitmap bitmap) {
        var similarities = new List<List<Tuple<Rectangle, Rectangle, float>>>();
        
        foreach (var region in regions) {
            var neighbors = GetNeighbors(region, regions);
            var regionsList = new List<Tuple<Rectangle, Rectangle, float>>();
            
            if (!IsRectangleInsideImage(bitmap, region)) continue;

            foreach (var neighbor in neighbors) {
                if (!IsRectangleInsideImage(bitmap, neighbor)) continue;
                regionsList.Add(new Tuple<Rectangle, Rectangle, float>(region, neighbor,
                    Similarity(region, neighbor, bitmap)));
            }

            similarities.Add(regionsList);
        }

        return similarities;
    }
    
    private static bool IsRectangleInsideImage(Bitmap image, Rectangle rect) =>
         rect is { Left: >= 0, Top: >= 0 } &&
         rect.Right <= image.Width && rect.Bottom <= image.Height;
    
    private static List<Rectangle> GetNeighbors(Rectangle rect, IReadOnlyCollection<Rectangle> rects) {
        var offsets = new List<Point>();
        for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
                offsets.Add(new Point(i, j));

        var neighbors = new List<Rectangle>();
        foreach (var offset in offsets) {
            var neighbor = rect with { X = rect.X + offset.X * rect.Width, Y = rect.Y + offset.Y * rect.Height };
            if (neighbor != rect && rects.Any(rectangle => rectangle != rect && rectangle.IntersectsWith(neighbor)))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private static float Similarity(Rectangle firstRectangle, Rectangle secondRectangle, Bitmap bitmap) =>
        ColorSimilarity(bitmap, firstRectangle, secondRectangle) *
        Similarity(firstRectangle, secondRectangle) *
        FitSimilarity(firstRectangle, secondRectangle);

    private static float ColorSimilarity(Bitmap bitmap, Rectangle firstRectangle, Rectangle secondRectangle) =>
         ColorDistance(AverageColor(bitmap.Clone(firstRectangle, bitmap.PixelFormat)),
            AverageColor(bitmap.Clone(secondRectangle, bitmap.PixelFormat)));
    
    private static float ColorDistance(Color firstColor, Color secondColor) =>
         (float)Math.Sqrt(Math.Pow(firstColor.R - secondColor.R, 2) + Math.Pow(firstColor.G - secondColor.G, 2) +
                                Math.Pow(firstColor.B - secondColor.B, 2));
    
    private static float Similarity(Rectangle firstRectangle, Rectangle secondRectangle) { 
        var firstArea = firstRectangle.Width * firstRectangle.Height; 
        var secondArea = secondRectangle.Width * secondRectangle.Height; 
        
        var ratio = Math.Min(firstArea, secondArea) / Math.Max(firstArea, secondArea);
        return ratio; 
    }

    private static float FitSimilarity(Rectangle firstRectangle, Rectangle secondRectangle) {
        if (firstRectangle.Left >= secondRectangle.Left &&
            firstRectangle.Right <= secondRectangle.Right &&
            firstRectangle.Top >= secondRectangle.Top &&
            firstRectangle.Bottom <= secondRectangle.Bottom) return 1000f;
        return 1f;
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
        return Color.FromArgb((int)(totals[2] / (count == 0 ? 1 : count)), 
            (int)(totals[1] / (count == 0 ? 1 : count)), (int)(totals[0] / (count == 0 ? 1 : count))); 
    }
}