using System.Drawing;
using System.Drawing.Imaging;
using FotNET.NETWORK.LAYERS.REGIONS;
using FotNET.NETWORK.LAYERS.REGIONS.SCRIPTS;

namespace UnitTests;

public class RegionsTest {
    [Test]
    public void RegionsCreation() {
        var bitmap = (Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//fight.jpg");
        
        var subBitmaps = RegionsMaker.GetRegions(bitmap, 50, 2);
        var images = subBitmaps.Select(rec => bitmap.Clone(rec, bitmap.PixelFormat)).ToList();

        foreach (var image in images) {
            image.Save(@$"D:\загрузки\{Guid.NewGuid()}.png", ImageFormat.Png);
        }
    }
}