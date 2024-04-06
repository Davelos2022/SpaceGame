namespace SpaceGame.ProgressGame
{
    public class AccountPlayer
    {
        private int _countPoints;
        private int _countCrystals;
        private SaveLoadProgress _saveLoad;

        public int CountPoints => _countPoints;
        public int CountCrystal => _countCrystals;

        private bool _isInit = false;

        public void InitAccount()
        {
            _isInit = true;

            _saveLoad = new SaveLoadProgress();
            _countPoints = _saveLoad.LoadPointsProgress();
            _countCrystals = _saveLoad.LoadCrystalsProgress();
        }

        public void Set(int points, int crystals)
        {
            if (!_isInit) return;

            _countPoints = points;
            _countCrystals = crystals;
            _saveLoad.SaveProgress(_countPoints, _countCrystals);
        }
    }
}