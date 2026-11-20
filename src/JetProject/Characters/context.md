# Módulo: Characters

Conteúdo específico: os survivors do mod. ← [Plugin](../context.md) · [CONTEXT.md](../../../CONTEXT.md)

## Survivors
| Survivor | Contexto |
|---|---|
| Jet | [Jet/context.md](Jet/context.md) |

Cada survivor implementa `ICharacter` (`Name` + `Register(ModContext)`) e entra na lista `All` do `CharactersModule`.

## Estrutura padrão por survivor
```
Characters/
  CharactersModule.cs          registra cada survivor no ContentRegistry
  <Nome>/
    context.md                 contexto do survivor (kit, decisões, pendências)
    <Nome>Character.cs         ICharacter: monta body, SurvivorDef (usa helpers do Framework)
    <Nome>Config.cs            números de balanceamento (dano, cooldown) em um só lugar
    <Nome>Tokens.cs            constantes de tokens de idioma
    <Nome>StateMachines.cs     nomes das EntityStateMachines
    Skills/
      <Nome>Skills.cs          SkillDefs e SkillFamilies
    States/                    EntityStates, um arquivo por estado
      Primary/…  Secondary/…  Utility/…  Special/…
    Components/                MonoBehaviours próprios (passiva, trackers, etc.)
```

## Regras
- Números de balanceamento ficam **só** em `<Nome>Config.cs`, nunca espalhados nos States.
- Se um helper escrito aqui servir para outro survivor, ele **sobe** para o Framework.
- EntityStates são `public` (exigência do RoR2) e registrados via `ContentRegistry.AddEntityState<T>()`.
- Toda decisão de design relevante vai para o `context.md` do survivor.
