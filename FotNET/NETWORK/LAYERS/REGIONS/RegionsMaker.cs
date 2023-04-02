using System.Drawing;

namespace FotNET.NETWORK.LAYERS.REGIONS;

public class RegionsMaker {
    
    private readonly Bitmap _image; 
    private List<Rectangle> _regions; 
    private readonly int _minSize; 
    private readonly int _count; 

    public RegionsMaker(Bitmap image, int minSize = 50, int count = 2000) { 
        _image = image; 
        _minSize = minSize; 
        _count = count; 
    } 

    public List<Rectangle> GetRegions() { 
        _regions = DivideImageIntoRegions(_image, _minSize); 

        for (int i = 0; i < _count; i++) { 
            var similarities = ComputeSimilarities(_regions); 
            var mostSimilar = similarities.OrderByDescending(s => s!.Item3).FirstOrDefault();

            if (mostSimilar == null) continue;
            var mergedRegion = MergeRegions(mostSimilar.Item1, mostSimilar.Item2); 
            _regions.Remove(mostSimilar.Item1); 
            _regions.Remove(mostSimilar.Item2); 
            _regions.Add(mergedRegion);
        } 

        return _regions; 
    } 

    private List<Rectangle> DivideImageIntoRegions(Bitmap image, int minSize) { 
        var regions = new List<Rectangle>(); 
        var width = image.Width; 
        var height = image.Height; 

        for (var y = 0; y < height; y += minSize) { 
            for (var x = 0; x < width; x += minSize) { 
                var w = Math.Min(minSize, width - x); 
                var h = Math.Min(minSize, height - y); 
                regions.Add(new Rectangle(x, y, w, h)); 
            } 
        } 

        return regions; 
    } 

    private List<Tuple<Rectangle, Rectangle, float>?> ComputeSimilarities(List<Rectangle> regions) { 
        List<Tuple<Rectangle, Rectangle, float>?> similarities = new List<Tuple<Rectangle, Rectangle, float>?>(); 

        // Compute the similarities between adjacent regions. 
        for (int i = 0; i < regions.Count; i++) { 
            for (int j = i + 1; j < regions.Count; j++) { 
                float similarity = ComputeSimilarity(regions[i], regions[j]); 
                similarities.Add(new Tuple<Rectangle, Rectangle, float>(regions[i], regions[j], similarity)); 
            } 
        } 

        return similarities; 
    } 

    private float ComputeSimilarity(Rectangle r1, Rectangle r2) { 
        // Compute the similarity between two regions based on color, texture, and size. 
        float colorSimilarity = ComputeColorSimilarity(_image, r1, r2); 
        float textureSimilarity = ComputeTextureSimilarity(_image, r1, r2); 
        float sizeSimilarity = ComputeSizeSimilarity(r1, r2); 
        return colorSimilarity * textureSimilarity * sizeSimilarity; 
    } 

    private float ComputeColorSimilarity(Bitmap image, Rectangle r1, Rectangle r2) { 
        // Compute the color similarity between two regions using the average color of each region. 
        Color c1 = ComputeAverageColor((Bitmap)image.Clone(r1, image.PixelFormat)); 
        Color c2 = ComputeAverageColor((Bitmap)image.Clone(r2, image.PixelFormat)); 
        return ComputeColorDistance(c1, c2); 
    }

    private float ComputeTextureSimilarity(Bitmap image, Rectangle r1, Rectangle r2) { 
        // Compute the texture similarity between two regions using the difference of Gaussian (DoG) filter. 
        float[,] texture1 = ComputeTexture(image, r1); 
        float[,] texture2 = ComputeTexture(image, r2); 
        float textureDifference = ComputeTextureDifference(texture1, texture2); 
        return 1.0f / (1.0f + textureDifference); 
    } 

    private float[,] ComputeTexture(Bitmap image, Rectangle r) { 
        // Compute the texture of a region using the difference of Gaussian (DoG) filter. 
        float[,] texture = new float[r.Width, r.Height]; 
        Bitmap grayImage = Grayscale(image); 
        Bitmap blurredImage = GaussianBlur(grayImage, 5, 5, 1.0f); 
        Bitmap blurredImage2 = GaussianBlur(grayImage, 5, 5, 2.0f); 
        for (int x = 0; x < r.Width; x++) { 
            for (int y = 0; y < r.Height; y++) { 
                float pixelValue = blurredImage2.GetPixel(x + r.X, y + r.Y).R - blurredImage.GetPixel(x + r.X, y + r.Y).R; 
                texture[x, y] = pixelValue; 
            } 
        } 
        return texture; 
    } 

    private float ComputeTextureDifference(float[,] texture1, float[,] texture2) {
        float sumOfSquaredDifferences = 0.0f; 
        for (int x = 0; x < texture1.GetLength(0); x++) { 
            for (int y = 0; y < texture1.GetLength(1); y++) { 
                float diff = texture1[x, y] - texture2[x, y]; 
                sumOfSquaredDifferences += diff * diff; 
            } 
        } 
        return sumOfSquaredDifferences; 
    } 

    private float ComputeSizeSimilarity(Rectangle r1, Rectangle r2) { 
        // Compute the size similarity between two regions using the ratio of their areas. 
        float area1 = r1.Width * r1.Height; 
        float area2 = r2.Width * r2.Height; 
        float ratio = Math.Min(area1, area2) / Math.Max(area1, area2); 
        return ratio; 
    } 

    private Rectangle MergeRegions(Rectangle r1, Rectangle r2) { 
        // Merge two regions into a single region. 
        int x = Math.Min(r1.X, r2.X); 
        int y = Math.Min(r1.Y, r2.Y); 
        int width = Math.Max(r1.X + r1.Width, r2.X + r2.Width) - x; 
        int height = Math.Max(r1.Y + r1.Height, r2.Y + r2.Height) - y; 
        return new Rectangle(x, y, width, height); 
    } 

    private Color ComputeAverageColor(Bitmap image) { 
        // Compute the average color of an image. 
        long red = 0, green = 0, blue = 0; 
        for (int x = 0; x < image.Width; x++) { 
            for (int y = 0; y < image.Height; y++) { 
                Color color = image.GetPixel(x, y); 
                red +=color.R; 
                green += color.G; 
                blue += color.B; 
            } 
        }
        long numPixels = image.Width * image.Height; 
        byte avgRed = (byte)(red / numPixels); 
        byte avgGreen = (byte)(green / numPixels); 
        byte avgBlue = (byte)(blue / numPixels); 
        return Color.FromArgb(avgRed, avgGreen, avgBlue); 
    } 

    private Bitmap Grayscale(Bitmap image) { 
        // Convert an image to grayscale. 
        Bitmap grayImage = new Bitmap(image.Width, image.Height); 
        for (int x = 0; x < image.Width; x++) { 
            for (int y = 0; y < image.Height; y++) { 
                Color color = image.GetPixel(x, y); 
                byte gray = (byte)(0.299f * color.R + 0.587f * color.G + 0.114f * color.B); 
                grayImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray)); 
            } 
        } 
        return grayImage; 
    } 

    private Bitmap GaussianBlur(Bitmap image, int kernelSizeX, int kernelSizeY, float sigma) 
    { 
        // Apply a Gaussian blur to an image. 
        int radiusX = kernelSizeX / 2; 
        int radiusY = kernelSizeY / 2; 
        float[,] kernel = ComputeGaussianKernel(kernelSizeX, kernelSizeY, sigma); 
        Bitmap blurredImage = new Bitmap(image.Width, image.Height); 
        for (int x = 0; x < image.Width; x++) { 
            for (int y = 0; y < image.Height; y++) { 
                float red = 0, green = 0, blue = 0; 
                float weight = 0; 
                for (int i = -radiusX; i <= radiusX; i++) { 
                    for (int j = -radiusY; j <= radiusY; j++) { 
                        if (x + i >= 0 && x + i < image.Width && y + j >= 0 && y + j < image.Height) { 
                            Color color = image.GetPixel(x + i, y + j); 
                            float kernelValue = kernel[i + radiusX, j + radiusY]; 
                            red += kernelValue * color.R; 
                            green += kernelValue * color.G; 
                            blue += kernelValue * color.B; 
                            weight += kernelValue; 
                        } 
                    } 
                } 
                if (weight > 0) 
                { 
                red /= weight; 
                green /= weight; 
                blue /= weight; 
                } 
                red = Math.Min(Math.Max(red, 0), 255); 
                green = Math.Min(Math.Max(green, 0), 255); 
                blue = Math.Min(Math.Max(blue, 0), 255); 
                blurredImage.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue)); 
            } 
        } 
        return blurredImage; 
    } 

    private float[,] ComputeGaussianKernel(int sizeX, int sizeY, float sigma) { 
        // Compute a Gaussian kernel. 
        float[,] kernel = new float[sizeX, sizeY]; 
        int radiusX = sizeX / 2; 
        int radiusY = sizeY / 2; 
        float twoSigmaSquared = 2 * sigma * sigma; 
        float sum = 0; 
        for (int i = -radiusX; i <= radiusX; i++) { 
            for (int j = -radiusY; j <= radiusY; j++) { 
                float distanceSquared = i * i + j * j; 
                float kernelValue = (float)Math.Exp(-distanceSquared / twoSigmaSquared) / (float)(Math.PI * twoSigmaSquared); 
                kernel[i + radiusX, j + radiusY] = kernelValue; 
                sum += kernelValue; 
            } 
        } 
        for (int i = 0; i < sizeX; i++) { 
            for (int j = 0; j < sizeY; j++) { 
                kernel[i, j] /= sum; 
            } 
        } 
        return kernel; 
    }
    
    private float[,] ComputeSobelKernelX() { 
        // Compute the Sobel kernel for edge detection in the x direction. 
        float[,] kernel = new
        float[3, 3]; 
        kernel[0, 0] = -1; kernel[0, 1] = 0; kernel[0, 2] = 1; 
        kernel[1, 0] = -2; kernel[1, 1] = 0; kernel[1, 2] = 2; 
        kernel[2, 0] = -1; kernel[2, 1] = 0; kernel[2, 2] = 1; 
        return kernel; 
    } 

    private float[,] ComputeSobelKernelY() { 
        // Compute the Sobel kernel for edge detection in the y direction. 
        float[,] kernel = new float[3, 3]; 
        kernel[0, 0] = -1; kernel[0, 1] = -2; kernel[0, 2] = -1; 
        kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0; 
        kernel[2, 0] = 1; kernel[2, 1] = 2; kernel[2, 2] = 1; 
        return kernel; 
    } 

    private Bitmap ComputeGradientMagnitude(Bitmap image, float[,] kernelX, float[,] kernelY) { 
        // Compute the gradient magnitude of an image using the Sobel operator. 
        Bitmap gradientImage = new Bitmap(image.Width, image.Height); 
        for (int x = 0; x < image.Width; x++) { 
            for (int y = 0; y < image.Height; y++) { 
                float dx = 0, dy = 0; 
                for (int i = -1; i <= 1; i++) { 
                    for (int j = -1; j <= 1; j++) { 
                        if (x + i >= 0 && x + i < image.Width && y + j >= 0 && y + j < image.Height) { 
                            Color color = image.GetPixel(x + i, y + j); 
                            float kernelXValue = kernelX[i + 1, j + 1]; 
                            float kernelYValue = kernelY[i + 1, j + 1]; 
                            dx += kernelXValue * color.R; 
                            dy += kernelYValue * color.R; 
                        } 
                    } 
                } 
                float magnitude = (float)Math.Sqrt(dx * dx + dy * dy); 
                gradientImage.SetPixel(x, y, Color.FromArgb((int)magnitude, (int)magnitude, (int)magnitude)); 
            } 
        } 
        return gradientImage; 
    }
    
    private void ApplyNonMaximaSuppression(Bitmap gradientImage, Bitmap edgeImage) { 
    // Apply non-maxima suppression to thin out the edges. 
        for (int x = 1; x < gradientImage.Width - 1; x++) 
        { 
            for (int y = 1; y < gradientImage.Height - 1; y++) 
            { 
                float gradientX = GetPixelBrightness(gradientImage, x + 1, y) - GetPixelBrightness(gradientImage, x - 1, y); 
                float gradientY = GetPixelBrightness(gradientImage, x, y + 1) - GetPixelBrightness(gradientImage, x, y - 1); 
                float gradientMagnitude = (float)Math.Sqrt(gradientX * gradientX + gradientY * gradientY); 
                float edgePixel = GetPixelBrightness(edgeImage, x, y); 
                if (gradientMagnitude == 0) 
                { 
                    edgeImage.SetPixel(x, y, Color.FromArgb(0, 0, 0)); 
                } 
                else 
                { 
                    float dotProduct = (gradientX * (-gradientY)) / gradientMagnitude; 
                    float angle = (float)Math.Acos(dotProduct); 
                    if (angle < Math.PI / 4) 
                    { 
                        if (edgePixel < GetPixelBrightness(edgeImage, x - 1, y - 1) || 
                        edgePixel < GetPixelBrightness(edgeImage, x + 1, y + 1)) 
                        { 
                            edgeImage.SetPixel(x, y, Color.FromArgb(0, 0, 0)); 
                        } 
                    } 
                    else if (angle < Math.PI / 2) 
                    { 
                        if (edgePixel < GetPixelBrightness(edgeImage, x - 1, y) || 
                        edgePixel < GetPixelBrightness(edgeImage, x + 1, y)) 
                        { 
                            edgeImage.SetPixel(x, y, Color.FromArgb(0, 0, 0)); 
                        } 
                    } 
                    else if (angle < 3 * Math.PI / 4) 
                    { 
                        if (edgePixel < GetPixelBrightness(edgeImage, x - 1, y + 1) || 
                        edgePixel < GetPixelBrightness(edgeImage, x + 1, y - 1)) 
                        { 
                            edgeImage.SetPixel(x, y, Color.FromArgb(0, 0, 0)); 
                        } 
                    } 
                    else 
                    { 
                        if (edgePixel < GetPixelBrightness(edgeImage, x, y - 1) || 
                        edgePixel < GetPixelBrightness(edgeImage, x, y + 1)) 
                        { 
                            edgeImage.SetPixel(x, y, Color.FromArgb(0, 0, 0)); 
                        } 
                    } 
                } 
            } 
        } 
    } 

    private float GetPixelBrightness(Bitmap image, int x, int y) { 
        // Get the brightness of a pixel in an image. 
        if (x < 0 || x >= image.Width || y < 0 || y >= image.Height) { 
            return 0; 
        } 
        Color color = image.GetPixel(x, y); 
        return color.R + color.G + color.B; 
    }
}