using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtil
{
    public static System.Random random = new System.Random();
    public static List<T> Shuffle<T>(List<T> list)
    {
        return list.OrderBy(x => RandomUtil.random.Next()).ToList();
    }
    /// <summary>
    /// Method quantitatively tested and working as intended
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<IRarityObject> ShuffleHostList(List<IRarityObject> list)
    {
        return list.OrderBy(x => x.GetRarity() * RandomUtil.random.Next()).ToList();
    }
}
public interface IRarityObject {
    float GetRarity();
}
