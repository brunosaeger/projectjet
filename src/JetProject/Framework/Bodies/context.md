# Submódulo: Framework/Bodies

Criação e configuração de prefabs de body. ← [Framework](../context.md)

## Tipos
| Tipo | Papel |
|---|---|
| `BodyFactory` | `CloneBody(key, nome)` clona um body do jogo **com rede** (via R2API `PrefabAPI.InstantiateClone`); `CloneDisplay` clona sem rede |
| `StateMachineSetup` | `SetMainState(body, máquina, tipo)` troca o estado principal; `Add(body, nome, initial, main)` cria uma máquina nova |

## Por que `StateMachineSetup.Add` existe
Uma EntityStateMachine nova num body precisa de três integrações que é fácil esquecer:
1. Entrar em `NetworkStateMachine.stateMachines`, senão o estado não sincroniza em multiplayer.
2. Entrar em `CharacterDeathBehavior.idleStateMachine`, para voltar a idle ao morrer.
3. Entrar em `SetStateOnHurt.idleStateMachine`, para ser interrompida por stun/freeze.

## Regras
- Bodies de jogador **sempre** com `registerNetwork: true`. Displays (tela de seleção) sem rede.
- Clonar um body vanilla é aceitável como **ponto de partida/provisório**. Quando o modelo próprio existir,
  o clone continua como base de componentes e só o modelo é trocado (submódulo futuro para troca de modelo).
