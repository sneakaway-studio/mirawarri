

using UnityEngine;

public static class UM_RewardManager
{
    private const string k_CoinsKey = "um_coins";
    private const string k_GoldStatusKey = "um_gold_status";

    public static void AddCoins()
    {
        PlayerPrefs.SetInt(k_CoinsKey, Coins + 100);
    }

    public static void ActivateGold()
    {
        PlayerPrefs.SetInt(k_GoldStatusKey, 1);
    }

    public static int Coins
    {
        get
        {
            if (PlayerPrefs.HasKey(k_CoinsKey))
            {
                return PlayerPrefs.GetInt(k_CoinsKey);
            }
            return 0;
        }
    }

    public static bool HasGoldStatus
    {
        get { return PlayerPrefs.HasKey(k_GoldStatusKey); }
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(k_CoinsKey);
        PlayerPrefs.DeleteKey(k_GoldStatusKey);
    }
    
}