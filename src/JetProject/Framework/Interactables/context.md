# Submódulo: Framework/Interactables

Spawn de interagíveis (baús, barris, santuários...). ← [Framework](../context.md)

## API (`InteractableSpawning`, estático)
| Método | Uso |
|---|---|
| `TrySpawnNear(card, posição, rng, maxDistance)` | Coloca o interagível no ponto válido mais próximo, via `DirectorCore` (modo Approximate). Só no servidor |
| `TrySpawnAt(card, posição, rng)` | Coloca EXATAMENTE na posição (modo Direct). A posição deve estar no chão (`GroundFinder`) |
| `IsAvailable(card)` | Falso se o prefab exige uma expansão (DLC) desligada na partida |

## Regras
- Só no **servidor**; retorna null em clientes.
- RNG: use `RoR2Application.rng` (fora da simulação) para não consumir `stageRng`/`runRNG` e não alterar runs com seed.
- Spawn cards: carregue pelas constantes `GameAssetPathsBetter` (`isc*_asset`).
- Nome evita conflito com `RoR2.InteractableSpawner` (classe do jogo).
