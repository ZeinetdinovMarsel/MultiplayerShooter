using UnityEngine;

public static class MMRSystem
{
    private const int K = 32;

    public static int Calculate(int myMMR, int enemyMMR, bool win)
    {
        float expected = 1f / (1f + Mathf.Pow(10f, (enemyMMR - myMMR) / 400f));
        int score = win ? 1 : 0;

        return myMMR + Mathf.RoundToInt(K * (score - expected));
    }
}
