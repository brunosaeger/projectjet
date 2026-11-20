# Camada: Framework

Infraestrutura genérica do mod. ← [Plugin](../context.md) · [CONTEXT.md](../../../CONTEXT.md)

## Regra de ouro
O Framework **não sabe que existe um survivor**. Se o código menciona algo de um personagem específico
(nome, skill, número de balanceamento), ele pertence a `Characters/`, não aqui.

O teste: *"esse código serviria igualzinho para um segundo survivor totalmente diferente?"* Se sim, é Framework.

## Submódulos
| Submódulo | Serviço exposto | Contexto |
|---|---|---|
| Core | `ModContext`, `IModule`, `ModuleHost`, `Log` | [Core/context.md](Core/context.md) |
| Assets | `AssetBundleService` | [Assets/context.md](Assets/context.md) |
| Localization | *(nenhum, só registra a pasta de idiomas)* | [Localization/context.md](Localization/context.md) |
| Content | `ContentRegistry` | [Content/context.md](Content/context.md) |
| Bodies | `BodyFactory`, `StateMachineSetup` (estáticos) | [Bodies/context.md](Bodies/context.md) |
| Skills | `SkillFactory` (estático) | [Skills/context.md](Skills/context.md) |
| Survivors | `SurvivorFactory` (estático) | [Survivors/context.md](Survivors/context.md) |
| UI | `IResourceMeterSource`, `ResourceMeterHud` | [UI/context.md](UI/context.md) |
| Combat | `SelfKnockback` (estático) | [Combat/context.md](Combat/context.md) |
| Audio | *(nenhum, registra os soundbanks Wwise)* | [Audio/context.md](Audio/context.md) |
| Projectiles | `ProjectileFactory` (estático) | [Projectiles/context.md](Projectiles/context.md) |
| Interactables | `InteractableSpawning` (estático) | [Interactables/context.md](Interactables/context.md) |
| Commands | `ChatCommandRegistry` (serviço) | [Commands/context.md](Commands/context.md) |

## Serviço ou helper estático?
- **Tem estado compartilhado** (cache, registro, ciclo de vida)? → **serviço** num `IModule`, acessado por `ModContext.Resolve`.
- **É uma função pura de montagem** (cria e devolve objetos, sem estado)? → **classe estática**, sem módulo.
  Ex.: `SkillFactory`, `BodyFactory`.

## Quando criar um novo submódulo
Quando surgir infraestrutura reutilizável que não cabe nos atuais. Exemplos prováveis no futuro:
- **Models/**: troca do modelo de um body clonado pelo modelo do AssetBundle (CharacterModel, hurtboxes, ModelLocator).
- **Dots/**: DoTs customizados (Fogo Azul) via R2API.Dot.
- **Networking/**: mensagens de rede próprias.

Para criar: pasta + `XModule : IModule` + `context.md`, registrar em `JetPlugin` e na tabela do CONTEXT.md raiz.
