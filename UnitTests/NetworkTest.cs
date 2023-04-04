using System.Drawing;
using System.Drawing.Imaging;
using FotNET.MODELS.IMAGE_CLASSIFICATION;
using FotNET.SCRIPTS.REGION_CONVOLUTION;

namespace UnitTests;

public class NetworkTest {
    [Test]
    public void RCnnTest() {
        var bitmap = (Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//fight.jpg");
        RegionConvolution.ForwardFeed(bitmap, 50, 3, CnnClassification.DeepConvolutionNetwork, .2, 28, 28)
            .Save(@$"D:\загрузки\{Guid.NewGuid()}.png", ImageFormat.Png);
    }
}