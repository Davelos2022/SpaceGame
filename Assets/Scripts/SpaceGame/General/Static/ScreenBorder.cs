
namespace SpaceGame.General
{
    public static class ScreenBorder
    {
        private static int _heightScreen = 1920;
        private static int _widthScreen = 1080;
        private static float _offSetScreen = 150f;

        private static float _heightBorder = (_heightScreen / 2) - _offSetScreen;
        private static float _widthBorder = (_widthScreen / 2) - _offSetScreen;

        public static int HeightScreen => _heightScreen;
        public static int WidthScreen => _widthScreen;
        public static float HeightOptimimaze => _heightBorder;
        public static float WidthOptimaze => _widthBorder;
    }
}