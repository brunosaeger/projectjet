# Submódulo: Framework/Projectiles

Criação e adaptação de projéteis. ← [Framework](../context.md)

## API (`ProjectileFactory`, estático)
| Método | Uso |
|---|---|
| `Clone(key, nome)` | Clona um projétil do jogo **com rede** (R2API `InstantiateClone`) |
| `MakeArcing(prefab, velocidade, lifetime)` | Projétil em **arco**: gravidade ligada, velocidade só no lançamento (`updateAfterFiring = false`) |
| `ConfigureDamageZone(zona, dps, proc, duração, escala, damageType)` | Zona de dano contínuo (`ProjectileDotZone`, ex.: poça do Molotov): dano/s, proc coefficient, duração, tamanho |
| `Scale(prefab, visual, colisão)` | Projétil maior/menor: clona e escala o ghost (atribuído por referência direta, endereço limpo) e escala os `SphereCollider` |
| `MakeExploding(prefab, raio, efeito, lifetime, directHitCount, directMult, areaMult, areaHitsDirectTarget)` | Troca o impacto original por uma **explosão com dano direto + área**. Padrão = explosão em área simples |

## Explosão com acerto direto + área
- `DirectHitTracker` (IProjectileImpactBehavior): guarda o inimigo atingido em cheio.
- `DirectHitImpactExplosion` (subclasse de `ProjectileImpactExplosion`): ao detonar (servidor)
  1. aplica `DirectHitCount` acertos de `DirectDamageMultiplier` × dano no alvo direto;
  2. dispara a área com `AreaDamageMultiplier` × dano, **sem o alvo direto** (BlastAttack `CustomFilter`), salvo `AreaHitsDirectTarget`.
  Um único efeito visual e um único som.
- Evento `DetonatedServer(posição, normal)`: efeitos pós-impacto (ex.: criar uma zona). Inscrever no Awake de um componente do mesmo prefab.
- `IsApplyingDirectHit` fica true durante o dano direto. A morte é processada **de forma síncrona** dentro de
  `HealthComponent.TakeDamage` (→ `onCharacterDeathServer`), então quem escuta mortes sabe se veio do acerto direto
  (usado pelo baú da secundária do Jet).

**Armadilha do publicizer:** ao sobrescrever membros `protected` do jogo, declare o override como `public`
(o RoR2.dll é publicizado na compilação; ampliar o acesso é válido em runtime).

## Regras
- Todo projétil novo é registrado com `ContentRegistry.AddProjectile`, senão não funciona em rede.
- O **coeficiente de dano** fica no estado que dispara (`FireProjectileInfo.damage = coef × damageStat`), não no prefab.
- Tipo de dano (ex.: Burn) vai em `ProjectileDamage.damageType` no prefab, com a `DamageSource` correta
  (Primary/Secondary/Utility/Special): itens do jogo usam essa origem.
- O visual (ghost) vem do projétil clonado; trocar por ghost próprio quando houver VFX do mod.
