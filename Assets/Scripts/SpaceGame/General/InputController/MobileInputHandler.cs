using UnityEngine;

namespace SpaceGame.General
{
    public class MobileInputHandler : IInputHandler
    {
        public float GetInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    return touch.deltaPosition.x;
                }
            }

            return 0f;
        }
    }
}
