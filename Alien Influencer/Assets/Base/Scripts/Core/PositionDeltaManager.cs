using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set up Minion placement under UFO, forming a scalable star shape
/// Give swat a uniqgue target around the UFO to help avoid them walking through each other
/// </summary>
public static class PositionDeltaManager
{
    public static int minionCount = 0;
    public static int swatCount = 0;

    static Vector2[] minion_modulators = new Vector2[] {
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(-1, 1),
        new Vector2(1, -1),
        new Vector2(1, 1),
        new Vector2(-1, -1)
    };
    static Vector2[] swat_modulators = new Vector2[] {
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(1, 1),
        new Vector2(-1, -1),
        new Vector2(1, -1),
        new Vector2(-1, 1)
    };

    public static Vector2 MinonPlacementDelta()
    {
        if(minionCount == 0)
        {
            minionCount++;
            return new Vector2(0, 0);
        }
        int mag = 1 + (minionCount - 1) / 8;
        Vector2 mod = minion_modulators[minionCount % 8];
        Vector2 delta = new Vector2(mag * mod[0], mag * mod[1]);
        //Debug.LogFormat("Minion: {0}, Delta: {1}", minionCount, delta);
        minionCount++;
        return delta;
    }

    public static Vector2 SwatTargetDelta()
    {
        int mag = (swatCount / 8 + 1) * 10;
        Vector2 mod = swat_modulators[swatCount % 8];
        int radius = swatCount / 8;
        Vector2 delta = new Vector2(mag * mod[0] + radius, mag * mod[1] + radius);
        //Debug.LogFormat("Swat: {0}, delta: {1}", swatCount, delta);
        swatCount++;
        return delta;
    }

    public static void Reset()
    {
        minionCount = 0;
        swatCount = 0;
    }
}
