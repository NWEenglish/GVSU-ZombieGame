using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using UnityEngine;

namespace Assets.Scripts.NPC
{
    public class NpcAim
    {
        private Transform StartPoint;

        public NpcAim(Transform startPoint)
        {
            StartPoint = startPoint;
        }

        public bool IsVisible(GameObject otherGameObject)
        {
            bool retWasHit = false;

#if UNITY_EDITOR
            Debug.DrawLine(StartPoint.position, otherGameObject.transform.position);
#endif

            if (Physics2D.Linecast(StartPoint.position, otherGameObject.transform.position))
            {
                List<RaycastHit2D> hits = Physics2D.LinecastAll(StartPoint.position, otherGameObject.transform.position).ToList();

                // If we see a wall, we must not of seen the target
                if (hits.Any(hit => hit.collider?.gameObject?.tag == TagNames.Wall))
                {
                    retWasHit = false;
                }
                else
                {
                    RaycastHit2D firstApplicableHit = hits.FirstOrDefault(hit => hit.collider?.gameObject == otherGameObject);
                    retWasHit = firstApplicableHit.collider?.gameObject == otherGameObject;
                }
            }

            return retWasHit;
        }
    }
}
