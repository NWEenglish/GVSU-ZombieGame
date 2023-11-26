using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class NpcAim
    {
        private Transform StartPoint;
        private LayerMask IgnoredLayer;

        public NpcAim(Transform startPoint, LayerMask ignoredLayer)
        {
            StartPoint = startPoint;
            IgnoredLayer = ignoredLayer;
        }

        public bool IsVisible(GameObject otherGameObject)
        {
            bool retWasHit = false;

            // Dones't work
            //if (Physics2D.Linecast(StartPoint.position, otherGameObject.transform.position, ~IgnoredLayer))
            //{
            //    List<RaycastHit2D> hits = Physics2D.LinecastAll(StartPoint.position, otherGameObject.transform.position, ~IgnoredLayer, -10f).ToList();
            //    retWasHit = hits.Any(hit => hit.collider.TryGetComponent<TilemapCollider2D>(out _));
            //}

            if (Physics2D.Raycast(StartPoint.position, otherGameObject.transform.position))
            {
                RaycastHit2D hit = Physics2D.Raycast(StartPoint.position, StartPoint.right, Mathf.Infinity, ~IgnoredLayer);
                retWasHit = hit.collider != null;
            }

            return retWasHit;
        }
    }
}
