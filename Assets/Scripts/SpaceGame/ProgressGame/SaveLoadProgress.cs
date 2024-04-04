using UnityEngine;

namespace SpaceGame.ProgressGame
{
    public class SaveLoadProgress
    {
        private string _keyPoint = "PointsPlayer";
        private string _keyCrystal = "CrystalPlayer";

        public void SaveProgress(int poins, int coins)
        {
            PlayerPrefs.SetInt(_keyPoint, poins);
            PlayerPrefs.SetInt(_keyCrystal, coins);
        }

        public int LoadCoinsProgress()
        {
            return RetrieveByKey(_keyCrystal);
        }

        public int LoadPointProgress()
        {
            return RetrieveByKey(_keyPoint);
        }

        private int RetrieveByKey(string key)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key);
            else
                return 0;
        }
    }
}