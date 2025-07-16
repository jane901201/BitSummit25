using UnityEngine;

namespace Controller
{
    public abstract class IInputDevice: MonoBehaviour
    {
        public abstract void moveUpdate();
    }
}