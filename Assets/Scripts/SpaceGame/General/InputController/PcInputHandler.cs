using UnityEngine;

namespace SpaceGame.General
{
    public class PcInputHandler : IInputHandler
    {
        private const float SENSITIVY_FACTOR = 80f;  // Sensitivity factor to adjust the effect of mouse movement

        public float GetInput()
        {
            if (Input.GetMouseButton(0))
            {
                return Input.GetAxis("Mouse X") * SENSITIVY_FACTOR;
            }

            return 0f;
        }
    }
}
