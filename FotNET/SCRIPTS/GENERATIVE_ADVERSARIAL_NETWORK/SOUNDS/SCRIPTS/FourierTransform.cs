using System.Numerics;

namespace FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK.SOUNDS.SCRIPTS;

public static class FourierTransform {
    public static Complex[] ForwardFourierTransform(Complex[] complexes) {
        var complexesLength = complexes.Length;

        if (complexesLength == 1)
            return new[] { complexes[0] };

        var even = new Complex[complexesLength / 2];
        var odd = new Complex[complexesLength / 2];
        
        for (var i = 0; i < complexesLength / 2; i++) {
            even[i] = complexes[2 * i];
            odd[i] = complexes[2 * i + 1];
        }
        
        var fftEven = ForwardFourierTransform(even);
        var fftOdd = ForwardFourierTransform(odd);

        var fft = new Complex[complexesLength];
        for (var k = 0; k < complexesLength / 2; k++) {
            /*
            var complex = fftOdd[k] * Complex.Exp(-2.0 * Math.PI * Complex.ImaginaryOne * k / complexesLength);
            fft[k] = fftEven[k] + complex;
            fft[k + complexesLength / 2] = fftEven[k] - complex;
            */ // TODO: FIX COMPLEX
        }
        
        return fft;
    }
    
    public static Complex[] BackwardFourierTransform(Complex[] spectrum) {
        Array.Reverse(spectrum);
        var result = ForwardFourierTransform(spectrum);

        for (var i = 0; i < result.Length; i++)
            result[i] = Complex.Conjugate(result[i]) / result.Length;
        
        return result;
    }
}