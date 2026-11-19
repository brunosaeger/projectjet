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

## Quando criar um novo submódulo
Quando surgir infraestrutura reutilizável que não cabe nos atuais. Exemplos prováveis no futuro:
- **Skills/**: helpers para criar `SkillDef`/`SkillFamily` e montar `GenericSkill` num body.
- **Bodies/**: builder de CharacterBody/prefab a partir de um modelo (hurtboxes, ModelLocator, CharacterModel).
- **Networking/**: mensagens de rede próprias.
- **Config/**: binding tipado de opções do BepInEx.

Para criar: pasta + `XModule : IModule` + `context.md`, registrar em `JetPlugin` e na tabela do CONTEXT.md raiz.
