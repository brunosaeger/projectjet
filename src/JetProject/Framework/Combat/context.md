# Submódulo: Framework/Combat

Helpers de combate reutilizáveis (estáticos, sem estado). ← [Framework](../context.md)

| Tipo | Papel |
|---|---|
| `SelfKnockback` | `Apply(body, aimDirection, force)`: empurra o atirador para trás (mecanismo do tiro sem mira da Railgunner, via `CharacterMotor.ApplyForce`) |
| `BlastHelper` | `Explode(atacante, posição, raio, coef, damageType, efeito)`: explosão em área padronizada (sem queda por distância), pode rodar na autoridade do cliente |
| `BurnHelper` | `Apply(...)`: Burn idêntico ao IgniteOnHit (50% do dano do acerto, upgrade por Ignition Tank), para Burn com chance; `IsBurn(dotIndex)` |
| `GroundFinder` | `TryFindBelow(ponto, distância)`: chão abaixo de um ponto (raycast no world) |
| `ProcThresholds` | `ClampBelowBandProc(coef)`: teto de 399% por acerto, para nunca ativar Kjaro's/Runald's Band (ativam em ≥ 400%) |

## Regras
- Chamar só na **autoridade** do body (o movimento de jogador é simulado no cliente dono).
- A força é dividida pela massa do body: 1000 em massa 100 ≈ 10 m/s.
- Candidatos futuros aqui: aplicação de Burn, explosões (BlastAttack) padronizadas, DoTs.
