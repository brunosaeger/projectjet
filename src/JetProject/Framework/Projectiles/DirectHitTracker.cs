using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JetProject.Framework.Projectiles
{
    /// <summary>Guarda o inimigo atingido em cheio pelo projétil (o primeiro HealthComponent que não seja o dono).</summary>
    public class DirectHitTracker : MonoBehaviour, IProjectileImpactBehavior
    {
        public HealthComponent DirectTarget { get; private set; }
        public Vector3 ImpactPoint { get; private set; }

        private ProjectileController _controller;

        private void Awake() => _controller = GetComponent<ProjectileController>();

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (DirectTarget || !impactInfo.collider)
                return;

            var hurtBox = impactInfo.collider.GetComponent<HurtBox>();
            var health = hurtBox ? hurtBox.healthComponent : null;
            if (!health || (_controller && health.gameObject == _controller.owner))
                return;

            DirectTarget = health;
            ImpactPoint = impactInfo.estimatedPointOfImpact;
        }
    }
}
