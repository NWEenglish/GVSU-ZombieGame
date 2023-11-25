using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class NpcAim : MonoBehaviour
    {
        public LineRenderer LineRenderer;
        public Transform StartPoint;

        [SerializeField]
        private float AimDistance = 100f;

        private void Start()
        {
            if (StartPoint == null)
            {
                StartPoint = gameObject.GetComponent<Transform>();
            }
        }

        public bool IsVisible(GameObject gameObject)
        {
            bool wasHit = false;

            if (Physics2D.Raycast(StartPoint.position, gameObject.transform.position))
            {
                RaycastHit2D hit = Physics2D.Raycast(StartPoint.position, StartPoint.right);

                if (hit.collider != null)
                {
                    wasHit = true;
                }
            }

            return wasHit;
        }
    }
}
