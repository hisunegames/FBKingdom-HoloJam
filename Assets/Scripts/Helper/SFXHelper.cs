using UnityEngine;

public static class SFXHelper
{
    private static readonly int[] pentatonicSemitones = { 0, 2, 4, 7, 9 };

    public static float GetRandomPitch()
    {
        int rand = Random.Range(0, pentatonicSemitones.Length);
        int semitone = pentatonicSemitones[rand];

        return Mathf.Pow(1.059463f, semitone);
    }
}
