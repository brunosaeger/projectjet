# Submódulo: Framework/Effects

Efeitos visuais derivados dos do jogo. ← [Framework](../context.md)

## API (`EffectFactory`, estático)
| Método | Uso |
|---|---|
| `Clone(key, nome)` | Clona um efeito do jogo (só visual, sem rede) |
| `RemoveScreenShake(efeito)` | Tira o `ShakeEmitter` |
| `ScaleScreenShake(efeito, fator)` | Multiplica a amplitude do tremor (efeitos em loop: use fator baixo; ≤ 0 remove) |
| `Mute(efeito)` | Zera `EffectComponent.soundName` |
| `ScaleHierarchy(efeito, escala)` | Escala o efeito inteiro, passando as partículas para o modo Hierarchy (para efeitos com `applyScale = 0`, que ignoram `EffectData.scale`) |
| `RemoveChildren(efeito, nomes...)` / `KeepOnlyChildren(efeito, nomes...)` | Separa partes de um efeito (ex.: chama da jetpack com ritmo próprio) |
| `TintParticles(efeito, cor)` | Muda a cor inicial das partículas (e luzes), ex.: chama azul |
| `RemoveLights(efeito)` | Tira `Light` + `LightIntensityCurve` |
| `UseHierarchyScaling(efeito)` | Partículas seguem `transform.localScale` (para variar o tamanho de cada instância em runtime) |
| `Register(content, efeito)` | Registra como `EffectDef` (necessário para `EffectManager.SpawnEffect`) |

## Regras
- Efeitos só **visuais e locais** (ex.: fumaça sob a jetpack) usam `EffectManager.SpawnEffect(..., transmit: false)`
  em código que roda em **todos** os clientes. Assim não há tráfego de rede por efeito.
- Antes de copiar um efeito, inspecione-o com `tools/assets/inspect-bundle.ps1` (som embutido, tremor, `applyScale`, duração).
