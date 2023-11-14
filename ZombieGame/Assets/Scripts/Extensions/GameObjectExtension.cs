using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class GameObjectExtension
    {
        public static bool HasComponent<T>(this GameObject gameObject)
        {
            bool hasComponent = gameObject.TryGetComponent(out T gameComponent);
            return hasComponent;
        }
    }
}
