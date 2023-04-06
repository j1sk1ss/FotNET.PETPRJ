using System.Drawing;
using System.Drawing.Imaging;
using FotNET.SCRIPTS.REGION_CONVOLUTION.SCRIPTS;

namespace UnitTests;

public class RegionsTest {
    [Test]
    public void RegionsCreation() {
        var bitmap = (Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//RCNN_TEST//test.jpg");
        var subBitmaps = RegionsMaker.GetRegions(bitmap, 31, 20, .4);

        var graphics = Graphics.FromImage(bitmap);
        foreach (var image in subBitmaps) 
            graphics.DrawRectangle(new Pen(Color.Black), image);
        
        bitmap.Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
        foreach (var image in subBitmaps) 
            bitmap.Clone(image, bitmap.PixelFormat).Save(@$"C://Users//j1sk1ss//Desktop//RCNN_TEST//answers//{Guid.NewGuid()}.png", ImageFormat.Png);
    }
}