using FotNET.NETWORK.MATH.OBJECTS;
using NAudio.Wave;

namespace FotNET.DATA.SOUND;

public static class Parser {
    /// <summary>
    /// Convert array of values into .mp3 file
    /// </summary>
    /// <param name="input"> Input array </param>
    /// <param name="outputPath"> Path for saving </param>
    /// <param name="sampleRate"> Sample rate for output .mp3 </param>
    /// <param name="channels"> Count of channels of output .mp3 file </param>
    public static void ConvertArrayToSound(Vector input, string outputPath, int sampleRate = 44100, int channels = 1) {
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);

        using var writer = new WaveFileWriter(outputPath, waveFormat);
        var buffer = new WaveBuffer(input.Size);

        for (var i = 0; i < input.Size; i++)
            buffer.FloatBuffer[i] = (float)input[i];
            

        writer.WriteSamples(buffer.FloatBuffer, 0, buffer.ByteBuffer.Length);
    }

    /// <summary>
    /// Convert .mp3 file into Matrix of values
    /// </summary>
    /// <param name="path"> Path of .mp3 file </param>
    /// <param name="sampleRate"> Sample rate for input .mp3 </param>
    /// <param name="frameSize"> Frame size of input .mp3 file </param>
    /// <param name="hopSize"> Frame size of input .mp3 file </param>
    /// <param name="numCoefficients"> Num Coefficients size of input .mp3 file </param>
    /// <param name="preEmphasis"> Pre Emphasis size of input .mp3 file </param>
    /// <returns></returns>
    public static Matrix ConvertSoundToSpectrogram(string path, int sampleRate = 44100, int frameSize = 1024,
        int hopSize = 512,
        int numCoefficients = 13, double preEmphasis = .97d) =>
        SoundConverter.Convert(path, sampleRate, frameSize, hopSize, numCoefficients, preEmphasis);
}