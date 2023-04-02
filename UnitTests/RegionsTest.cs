using System.Drawing;
using System.Drawing.Imaging;
using FotNET.NETWORK.LAYERS.REGIONS;

namespace UnitTests;

public class RegionsTest {
    [Test]
    public void RegionsCreation() {
        var bitmap = (Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//fight.jpg");
        
        var subBitmaps = new RegionsMaker().GetBitmapRegions(bitmap, 50, 1);

        foreach (var image in subBitmaps) {
            image.Save(@$"D:\загрузки\{Guid.NewGuid()}.png", ImageFormat.Png);
        }
    }
}