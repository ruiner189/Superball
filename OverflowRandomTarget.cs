using Battle.Attacks;
using Battle.Enemies;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Superball
{
    [HarmonyPatch]
    [RequireComponent(typeof(ProjectileAttack))]
    public class OverflowRandomTarget : MonoBehaviour
    {
        public Enemy CurrentTarget;
        private EnemyManager _enemyManager;


        public void Awake()
        {
            _enemyManager = Resources.FindObjectsOfTypeAll<EnemyManager>().FirstOrDefault();
        }

        public Enemy GetRandomTarget()
        {
            return _enemyManager.Enemies
                .Where(enemy => enemy.CurrentHealth > 0)
                .OrderBy(enemy => Random.Range(0f, 1f))
                .FirstOrDefault();
        }

        public void ChangeDirections(ShotBehavior behavior)
        {
            if (CurrentTarget == null) return;

            TargetingUI target = CurrentTarget.GetComponentInChildren<TargetingUI>();

            if (target == null) return;

            Rigidbody2D rigid = behavior._rigid;

            float velocity = rigid.velocity.magnitude;

            Vector2 currentPosition = behavior.transform.position;
            Vector2 endPosition = target.transform.position;

            Vector2 direction = (endPosition - currentPosition).normalized;

            rigid.velocity = direction * velocity;
        }


        [HarmonyPatch(typeof(ProjectileAttack), nameof(ProjectileAttack.Fire))]
        [HarmonyPrefix]
        public static void PatchFire(ProjectileAttack __instance, ref Enemy target)
        {
            OverflowRandomTarget overflow = __instance.GetComponent<OverflowRandomTarget>();
            if (overflow != null)
            {
                target = overflow.GetRandomTarget();
                overflow.CurrentTarget = target;
            }
        }

        [HarmonyPatch(typeof(ProjectileAttack), nameof(ProjectileAttack.OnEnemyHit))]
        [HarmonyPrefix]
        public static void PatchOnTriggerEnter(ProjectileAttack __instance, Enemy enemy, ShotBehavior shot)
        {
            OverflowRandomTarget overflow = __instance.GetComponent<OverflowRandomTarget>();

            if (overflow == null || overflow.CurrentTarget != enemy) return;

            overflow.CurrentTarget = overflow.GetRandomTarget();
            overflow.ChangeDirections(shot);
            
        }
    }
}
