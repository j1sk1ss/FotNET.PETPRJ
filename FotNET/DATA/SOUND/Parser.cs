using FotNET.NETWORK.MATH.OBJECTS;
using FotNET.SCRIPTS.GENERATIVE_ADVERSARIAL_NETWORK.SOUNDS.SCRIPTS;
using NAudio.Wave;

namespace FotNET.DATA.SOUND;

public static class Parser {
    public static void ConvertArrayToSound(Vector input, string outputPath, int sampleRate = 44100, int channels = 1) {
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);

        using var writer = new WaveFileWriter(outputPath, waveFormat);
        var buffer = new WaveBuffer(input.Size);

        for (var i = 0; i < input.Size; i++)
            buffer.FloatBuffer[i] = (float)input[i];
            

        writer.WriteSamples(buffer.FloatBuffer, 0, buffer.ByteBuffer.Length);
    }

    public static Matrix ConvertSoundToSpectrogram(string path, int sampleRate = 44100, int frameSize = 1024,
        int hopSize = 512,
        int numCoefficients = 13, double preEmphasis = .97d) =>
        SoundConverter.Convert(path, sampleRate, frameSize, hopSize, numCoefficients, preEmphasis);
}