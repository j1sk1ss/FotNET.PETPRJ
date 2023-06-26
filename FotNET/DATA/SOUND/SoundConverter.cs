using System.Numerics;
using FotNET.NETWORK.MATH.OBJECTS;
using FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK.SOUNDS.SCRIPTS;

namespace FotNET.DATA.SOUND;

public static class SoundConverter {
    public static Matrix Convert(string path, int sampleRate = 44100, int frameSize = 1024, int hopSize = 512,
        int numCoefficients = 13, double preEmphasis = .97d) {
        var audioBytes = File.ReadAllBytes(path);
        
        var audioSamples = new float[audioBytes.Length / 2];
        for (var i = 0; i < audioBytes.Length; i += 2) {
            var sample = BitConverter.ToInt16(audioBytes, i);
            audioSamples[i / 2] = sample / 32768.0f;
        }

        return new Matrix(CepstralCoefficients(audioSamples, sampleRate, frameSize, hopSize, numCoefficients, preEmphasis));
    }
    
    private static double[,] CepstralCoefficients(float[] audioSamples, int sampleRate, int frameSize, int hopSize, int numCoefficients, double preEmphasis) {
        for (var i = 1; i < audioSamples.Length; i++)
            audioSamples[i] = (float)(audioSamples[i] - preEmphasis * audioSamples[i - 1]);

        var numFrames = (int)Math.Floor((double)(audioSamples.Length - frameSize) / hopSize) + 1;
        var spectrogram = new double[numFrames, frameSize / 2 + 1];
        for (var i = 0; i < numFrames; i++) {
            var frame = new float[frameSize];
            Array.Copy(audioSamples, i * hopSize, frame, 0, frameSize);
            var spectrum = ComputeSpectrum(frame, frameSize);
            for (var j = 0; j < spectrum.Length; j++)
                spectrogram[i, j] = spectrum[j];
        }
        
        var numFilters = 40;
        var filterBank = ComputeMelFilterbank(sampleRate, frameSize, numFilters);
        var melSpectrogram = ApplyMelFilterBank(spectrogram,filterBank);
        var mock = new double[numFrames, numCoefficients];
        
        for (var i = 0; i < numFrames; i++) 
            for (var j = 0; j < numCoefficients; j++) {
                var sum = 0.0;
                for (var k = 0; k < numFilters; k++) 
                    sum += Math.Log(melSpectrogram[i, k]) * Math.Cos(Math.PI * (j + 0.5) / numFilters);
                
                mock[i, j] = sum;
            }
        
        return mock;
    }
    
    private static double[] ComputeSpectrum(IReadOnlyList<float> frame, int fftSize) {
        var spectrum = new double[fftSize / 2 + 1];
        var buffer = new Complex[fftSize];
        for (var i = 0; i < fftSize; i++) 
            buffer[i] = i < frame.Count ? new Complex(frame[i], 0.0) : new Complex(0.0, 0.0);
        
        buffer = FourierTransform.ForwardFourierTransform(buffer);
        for (var i = 0; i < spectrum.Length; i++)
            spectrum[i] = buffer[i].Magnitude;
        
        return spectrum;
    }

    private static double[,] ComputeMelFilterbank(int sampleRate, int fftSize, int numFilters) {
        var minFrequency = 0.0;
        var maxFrequency = sampleRate / 2.0;
        var minMel = FrequencyToMel(minFrequency);
        var maxMel = FrequencyToMel(maxFrequency);
        var filterEdges = new double[numFilters + 2];
        
        for (var i = 0; i < filterEdges.Length; i++)
            filterEdges[i] = MelToFrequency(minMel + (maxMel - minMel) / (numFilters + 1) * i); 
        
        var filterBank = new double[fftSize / 2 + 1, numFilters];
        for (var i = 0; i < numFilters; i++) {
            var weights = ComputeMelFilterWeights(filterEdges[i], filterEdges[i + 1], filterEdges[i + 2], fftSize);
            for (var j = 0; j < fftSize / 2 + 1; j++) 
                filterBank[j, i] = weights[j];
        }
        return filterBank;
    }
    
    private static double[] ComputeMelFilterWeights(double left, double center, double right, int fftSize) {
        var weights = new double[fftSize / 2 + 1];
        for (var i = 0; i < fftSize / 2 + 1; i++) {
            var frequency = i * 1.0 / fftSize * 2 * Math.PI;
            if (frequency < left || frequency > right)
                weights[i] = 0.0;
            else if (frequency < center)
                weights[i] = (frequency - left) / (center - left);
            else
                weights[i] = (right - frequency) / (right - center);
        }
        
        return weights;
    }
    
    private static double[,] ApplyMelFilterBank(double[,] spectrogram, double[,] filterBank) {
        var numFrames = spectrogram.GetLength(0);
        var numFilters = filterBank.GetLength(1);
        
        var melSpectrogram = new double[numFrames, numFilters];
        for (var i = 0; i < numFrames; i++)
            for (var j = 0; j < numFilters; j++) {
                    var sum = 0.0;
                    for (var k = 0; k < spectrogram.GetLength(1); k++)
                        sum += spectrogram[i, k] * filterBank[k, j];
                    
                    melSpectrogram[i, j] = sum;
            }
        
        return melSpectrogram;
    }
    
    private static double FrequencyToMel(double frequency) => 1127.01048 * Math.Log(1 + frequency / 700.0);

    private static double MelToFrequency(double mel) => 700.0 * (Math.Exp(mel / 1127.01048) - 1);
    
    public static double[] SpectrogramToWaveform(Matrix spectrogram, int windowSize, int hopSize) {
        var numFrames = spectrogram.Columns;
        var numSamples = (numFrames - 1) * hopSize + windowSize;

        var waveform = new double[numSamples];
        
        var window = new double[windowSize];
        for (var i = 0; i < windowSize; i++)
            window[i] = 0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (windowSize - 1));
        
        for (var i = 0; i < numFrames; i++) {
            var frame = new Complex[windowSize];
            for (var j = 0; j < windowSize; j++)
                frame[j] = new Complex(spectrogram.Body[i,j], 0);
            frame = FourierTransform.BackwardFourierTransform(frame);

            for (var j = 0; j < windowSize; j++)
                waveform[i * hopSize + j] += frame[j].Real * window[j];
        }

        for (var i = 0; i < numFrames; i++) 
            for (var j = 0; j < windowSize; j++) {
                var sampleIndex = i * hopSize + j;
                waveform[sampleIndex] /= window[j];
            }
        
        return waveform;
    }
}