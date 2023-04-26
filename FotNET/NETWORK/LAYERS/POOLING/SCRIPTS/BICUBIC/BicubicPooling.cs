using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.BICUBIC;

/// <summary>
/// Takes bicubic value from area and puts in new matrix
/// </summary>
public class BicubicPooling : Pooling {
    protected override Matrix BackPool(Matrix matrix, Matrix referenceMatrix, int poolSize) {
        var inputWidth  = matrix.Columns;
        var inputHeight = matrix.Rows;

        var xScale = inputWidth / (inputWidth / (double)poolSize);
        var yScale = inputHeight / (inputHeight / (double)poolSize);

        var output = new Matrix(inputWidth / poolSize, inputHeight / poolSize);

        Parallel.For(0, output.Columns, y => {
            for (var x = 0; x < output.Rows; x++) {
                var secondX = x * xScale;
                var secondY = y * yScale;

                var floorX = (int)Math.Floor(secondX - 1);
                var floorY = (int)Math.Floor(secondY - 1);

                var samples = new Matrix(4, 4);

                for (var j = 0; j < 4; j++) 
                for (var i = 0; i < 4; i++) {
                    var neighborX = floorX + i;
                    var neighborY = floorY + j;

                    if (neighborX < 0 || neighborX >= inputWidth || neighborY < 0 || neighborY >= inputHeight)
                        samples.Body[j, i] = 0;
                    else
                        samples.Body[j, i] = matrix.Body[neighborY, neighborX];
                }
                
                var fractionalX = secondX - floorX - 1;
                var fractionalY = secondY - floorY - 1;

                var weights = new Matrix(4, 4);

                for (var j = 0; j < 4; j++)
                for (var i = 0; i < 4; i++)
                    weights.Body[j, i] = CubicWeight(i - fractionalX) * CubicWeight(j - fractionalY);
                
                double sum = 0;
                for (var j = 0; j < 4; j++)
                for (var i = 0; i < 4; i++)
                    sum += samples.Body[j, i] * weights.Body[j, i];
                
                output.Body[y, x] = sum;
            }
        });
        
        return output;
    }

    private static double CubicWeight(double value) {
        const double a = -0.5;

        return Math.Abs(value) switch {
            >= 2 => 0,
            >= 1 => ((a + 2) * Math.Abs(value) - (a + 3)) * Math.Pow(Math.Abs(value), 2) + 1,
            _ => (a + 2) * Math.Pow(Math.Abs(value), 3) - (a + 3) * Math.Pow(Math.Abs(value), 2) + 1
        };
    }
    
    protected override Matrix Pool(Matrix matrix, int poolSize) {
        var inHeight = matrix.Rows / poolSize;
        var inWidth = matrix.Columns / poolSize;
        var output = new Matrix(inHeight * poolSize, inWidth * poolSize);

        Parallel.For(0, inHeight, i => {
            for (var j = 0; j < inWidth; j++) 
                for (var k = 0; k < poolSize; k++) 
                    for (var l = 0; l < poolSize; l++) {
                        var row = i * poolSize + k;
                        var col = j * poolSize + l;

                        var xCoordinate = j / (double)inWidth;
                        var yCoordinate = i / (double)inHeight;

                        var xWeight = 0.0;
                        var yWeight = 0.0;
                        for (var m = -1; m <= 2; m++) {
                            var h = CubicSpline(Math.Abs(m) + (xCoordinate - j) * poolSize);
                            
                            xWeight += h * (m < 0 ? -1 : 1) * matrix.Body[row, col + m * poolSize];
                            h = CubicSpline(Math.Abs(m) + (yCoordinate - i) * poolSize);
                            yWeight += h * (m < 0 ? -1 : 1) * matrix.Body[row, col + m * poolSize * inWidth];
                        }

                        output.Body[row, col] = xWeight * yWeight;
                    }
        });

        return output;
    }
    
    private static double CubicSpline(double x) => Math.Abs(x) switch {
            <= 1d => 1d - 2d * Math.Pow(Math.Abs(x), 2) + Math.Pow(Math.Abs(x), 3),
            < 2d => Math.Pow(2d - Math.Abs(x), 3),
            _ => 0d
        };
}