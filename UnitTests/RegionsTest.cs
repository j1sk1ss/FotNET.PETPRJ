using System.Drawing;
using System.Drawing.Imaging;
using FotNET.SCRIPTS.REGION_CONVOLUTION.SCRIPTS;

namespace UnitTests;

public class RegionsTest {
    [Test]
    public void RegionsCreation() {
        var bitmap = (Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//RCNN_TEST//test1.jpg");
        var subBitmaps = RegionsMaker.GetRegions(bitmap, 64, 9);

        var graphics = Graphics.FromImage(bitmap);
        foreach (var image in subBitmaps) {
            graphics.DrawRectangle(Pens.Black, image);
        }
        
        bitmap.Save(@$"D:\загрузки\{Guid.NewGuid()}.png", ImageFormat.Png);
    }
}