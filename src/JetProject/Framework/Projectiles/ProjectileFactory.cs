using JetProject.Framework.Assets;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JetProject.Framework.Projectiles
{
    /// <summary>
    /// Criação e adaptação de prefabs de projétil a partir de projéteis do jogo.
    /// O chamador registra o resultado com <c>ContentRegistry.AddProjectile</c> (obrigatório para rede).
    /// </summary>
    internal static class ProjectileFactory
    {
        /// <summary>Clona um projétil do jogo com registro de rede.</summary>
        public static GameObject Clone(string addressableKey, string newName)
        {
            var original = GameAssets.Load<GameObject>(addressableKey);
            return original.InstantiateClone(newName, registerNetwork: true);
        }

        /// <summary>
        /// Projétil em arco: sai com <paramref name="speed"/> na direção do disparo e cai com a gravidade.
        /// A velocidade só é definida no lançamento (sem updateAfterFiring), daí em diante a física manda.
        /// </summary>
        public static void MakeArcing(GameObject projectile, float speed, float lifetime)
        {
            var body = projectile.GetComponent<Rigidbody>();
            if (body)
                body.useGravity = true;

            var simple = projectile.GetComponent<ProjectileSimple>();
            if (simple)
            {
                simple.desiredForwardSpeed = speed;
                simple.updateAfterFiring = false;
                simple.lifetime = lifetime;
            }
        }

        /// <summary>
        /// Aumenta (ou diminui) o projétil: visual (ghost clonado e escalado) e colisão (SphereCollider).
        /// O ghost original vem por endereço Addressable; o clone é atribuído como referência direta e o endereço é limpo
        /// (ProjectileController só usa a referência direta quando o endereço é inválido).
        /// </summary>
        public static void Scale(GameObject projectile, float visualScale, float colliderScale)
        {
            // ghostPrefab é "obsoleto" (o jogo prefere endereços Addressable), mas um endereço não aponta para um clone
            // criado em runtime: a referência direta é o único caminho para um ghost modificado. Uso intencional.
#pragma warning disable CS0618
            var controller = projectile.GetComponent<ProjectileController>();
            if (controller && !Mathf.Approximately(visualScale, 1f))
            {
                var originalGhost = controller.ghostPrefabAddress != null && controller.ghostPrefabAddress.RuntimeKeyIsValid()
                    ? GameAssets.Load<GameObject>(controller.ghostPrefabAddress.AssetGUID)
                    : controller.ghostPrefab;

                if (originalGhost)
                {
                    var ghost = originalGhost.InstantiateClone(projectile.name + "Ghost", registerNetwork: false);
                    ghost.transform.localScale *= visualScale;
                    controller.ghostPrefab = ghost;
                    controller.ghostPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceT<GameObject>("");
                }
            }
#pragma warning restore CS0618

            if (!Mathf.Approximately(colliderScale, 1f))
            {
                foreach (var sphere in projectile.GetComponents<SphereCollider>())
                    sphere.radius *= colliderScale;
            }
        }

        /// <summary>
        /// Configura uma zona de dano contínuo (ProjectileDotZone, ex.: poça do Molotov).
        /// <paramref name="damagePerSecondMultiplier"/> é sobre o dano do projétil da zona: 1 = o dano do projétil por segundo.
        /// <paramref name="scale"/> escala a área (hitboxes) e o visual.
        /// </summary>
        public static ProjectileDotZone ConfigureDamageZone(GameObject zone, float damagePerSecondMultiplier, float procCoefficient,
            float lifetime, float scale, DamageTypeCombo damageType)
        {
            var dotZone = zone.GetComponent<ProjectileDotZone>();
            if (dotZone)
            {
                float ticksPerSecond = Mathf.Max(0.01f, dotZone.fireFrequency);
                dotZone.damageCoefficient = damagePerSecondMultiplier / ticksPerSecond;
                dotZone.overlapProcCoefficient = procCoefficient;
                dotZone.lifetime = lifetime;
            }

            var damage = zone.GetComponent<ProjectileDamage>();
            if (damage)
                damage.damageType = damageType;

            if (!Mathf.Approximately(scale, 1f))
            {
                zone.transform.localScale *= scale;
                Scale(zone, scale, 1f);
            }

            return dotZone;
        }

        /// <summary>
        /// Troca o comportamento de impacto original por uma explosão no impacto (inimigo ou cenário), com dano separado
        /// para o alvo direto e para a área (ver <see cref="DirectHitImpactExplosion"/>). Multiplicadores são sobre o dano
        /// do projétil (o coeficiente fica no estado que dispara).
        /// Padrão: explosão em área simples (sem acerto direto extra; a área inclui o alvo direto).
        /// </summary>
        public static DirectHitImpactExplosion MakeExploding(GameObject projectile, float radius, GameObject explosionEffect, float lifetime,
            int directHitCount = 0, float directDamageMultiplier = 1f, float areaDamageMultiplier = 1f, bool areaHitsDirectTarget = true)
        {
            foreach (var behavior in projectile.GetComponents<IProjectileImpactBehavior>())
                Object.DestroyImmediate((Component)behavior);
            foreach (var oldExplosion in projectile.GetComponents<ProjectileExplosion>())
                Object.DestroyImmediate(oldExplosion);

            projectile.AddComponent<DirectHitTracker>();
            var explosion = projectile.AddComponent<DirectHitImpactExplosion>();
            explosion.DirectHitCount = Mathf.Max(0, directHitCount);
            explosion.DirectDamageMultiplier = directDamageMultiplier;
            explosion.AreaDamageMultiplier = areaDamageMultiplier;
            explosion.AreaHitsDirectTarget = areaHitsDirectTarget;
            explosion.blastRadius = radius;
            explosion.blastDamageCoefficient = 1f;
            explosion.blastProcCoefficient = 1f;
            explosion.falloffModel = BlastAttack.FalloffModel.None;
            explosion.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            explosion.explosionEffect = explosionEffect;
            explosion.impactEffect = null;
            explosion.destroyOnEnemy = true;
            explosion.destroyOnWorld = true;
            explosion.impactOnWorld = true;
            explosion.lifetime = lifetime;
            explosion.timerAfterImpact = false;
            return explosion;
        }
    }
}
