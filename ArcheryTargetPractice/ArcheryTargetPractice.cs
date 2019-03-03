using Harmony;
using UnityEngine;

namespace ArcheryTargetPractice
{
    [HarmonyPatch(typeof(ArrowItem))]
    [HarmonyPatch("HandleCollisionWithObject")]
    internal class ArcheryTargetPractice
    {
        private static void Prefix(float ___m_ReleaseTime, GameObject collider, Vector3 collisionPoint)
        {
            // Must hit the target
            if (collider.name != "OBJ_BullseyeTarget_Prefab")
            {
                return;
            }

            // must be level 0 or 1 (1 or 2 in game)
            var archerylevel = (float)GameManager.GetSkillArchery().GetCurrentTierNumber();
            if (archerylevel > 1)
            {
                return;
            }

            // Must be standing a decent distance away
            var timeInAir = Time.time - ___m_ReleaseTime;
            if (timeInAir < 0.25)
            {
                return;
            }

            // Must hit the paper bullseye (not the outer rim)
            if (collisionPoint.x < 1646.7 || collisionPoint.x > 1647.2)
            {
                return;
            }
            if (collisionPoint.y < 43.9 || collisionPoint.y > 44.7)
            {
                return;
            }
            if (collisionPoint.z < 1827.9 || collisionPoint.z > 1828.6)
            {
                return;
            }

            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Archery, 1, SkillsManager.PointAssignmentMode.AssignInAnyMode);
        }
    }
}