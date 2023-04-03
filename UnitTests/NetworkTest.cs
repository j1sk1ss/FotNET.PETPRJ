﻿using System.Drawing;
using System.Drawing.Imaging;
using FotNET.MODELS.NUMBER_CLASSIFICATION;
using FotNET.MODELS.SCRIPTS.COMPUTER_VISION;

namespace UnitTests;

public class NetworkTest {
    [Test]
    public void RCnnTest() {
        var bitmap = (Bitmap)Bitmap.FromFile(@"C://Users//j1sk1ss//Desktop//fight.png");
        ComputerVision.Calculate(bitmap, CnnClassification.SimpleConvolutionNetwork, .3, 28, 28)
            .Save(@$"D:\загрузки\{Guid.NewGuid()}.png", ImageFormat.Png);
    }
}