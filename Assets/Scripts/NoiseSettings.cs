using Generation;
using UnityEngine;

public class NoiseSettings : MonoBehaviour
{
    public EasingType easingType;
    public int lowerBound;
    public int upperBound;
    public float noiseCaveThreshold;
    public bool updateVFX;
    public FastNoise.NoiseType noiseType;
    public int seed = 1337;
    public float frequency = 0.01f;
    public FastNoise.Interp interp = FastNoise.Interp.Quintic;
    public float cellularJitter = 0.45f;
    public float gain = 0.5f;
    public float lacunarity = 2.0f;
    public int octaves = 3;
    public FastNoise.FractalType fractalType = FastNoise.FractalType.FBM;
    public int cellularDistanceIndex0 = 0;
    public int cellularDistanceIndex1 = 1;
    public FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean;
    public float gradientPerturbAmp = 1.0f;
    public FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue;

    private void Awake()
    {
        Noise.SetSettings(noiseType, seed, interp, frequency, gain, lacunarity, octaves, fractalType, cellularJitter, 
            cellularDistanceIndex0, cellularDistanceIndex1, 
            cellularDistanceFunction,cellularReturnType,gradientPerturbAmp);

        ChunkGenerator.easingType = easingType;
        ChunkGenerator.lowerBound = lowerBound;
        ChunkGenerator.upperBound = upperBound;
        ChunkGenerator.noiseCaveThreshold = noiseCaveThreshold;
    }
    
    public FastNoise CreateFastNoise()
    {
        FastNoise fn = new FastNoise();

        fn.SetNoiseType(noiseType);
        fn.SetSeed(seed);
        fn.SetFrequency(frequency);
        fn.SetInterp(interp);
        
        //Fractal Settings
        fn.SetFractalGain(gain);
        fn.SetFractalLacunarity(lacunarity);
        fn.SetFractalOctaves(octaves);
        fn.SetFractalType(fractalType);
        
        //Cellular Settings
        fn.SetCellularDistance2Indicies(cellularDistanceIndex0, cellularDistanceIndex1);
        fn.SetCellularDistanceFunction(cellularDistanceFunction);
        fn.SetCellularJitter(cellularJitter);
        fn.SetCellularReturnType(cellularReturnType);
        
        fn.SetGradientPerturbAmp(gradientPerturbAmp);

        return fn;
    }
}
