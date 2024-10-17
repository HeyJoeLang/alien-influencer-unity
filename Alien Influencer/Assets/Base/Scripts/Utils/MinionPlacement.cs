using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set up Minion placement under UFO, forming a scalable star shape
/// </summary>
public static class MinionPlacement
{
    public static int minionCount = 0;
    public static int helicopterCount = 0;

    static Vector2[] modulators = new Vector2[] {
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(-1, 1),
        new Vector2(1, -1),
        new Vector2(1, 1),
        new Vector2(-1, -1)
    };

    public static Vector2 MinonPlacementDelta()
    {
        if(minionCount == 0)
        {
            minionCount++;
            return new Vector2(0, 0);
        }
        int mag = 1 + (minionCount - 1) / 8;
        Vector2 mod = modulators[minionCount % 8];
        Vector2 delta = new Vector2(mag * mod[0], mag * mod[1]);
        Debug.LogFormat("Minion: {0}, Delta: {1}", minionCount, delta);
        minionCount++;
        return delta;
    }
    public static void Reset()
    {
        minionCount = 0;
        helicopterCount = 0;
    }
    public static float HelicopterPlacementDeltaX()
    {
        float xMagnitude = 5f;
        if (helicopterCount == 0)
        {
            helicopterCount++;
            return 0;
        }

        int x = helicopterCount % 2 == 0 ? (helicopterCount + 1) / 2 : -1 * (helicopterCount + 1) / 2;
        helicopterCount++;
        return x * xMagnitude;
    }
}
