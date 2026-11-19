# Módulo: Characters

Conteúdo específico: os survivors do mod. ← [Plugin](../context.md) · [CONTEXT.md](../../../CONTEXT.md)

## Estado
Vazio. O primeiro survivor entra quando o conceito estiver fechado em [design/](../../../design/context.md).

## Estrutura planejada por survivor
```
Characters/
  CharactersModule.cs          registra cada survivor no ContentRegistry
  <Nome>/
    context.md                 contexto do survivor (kit, decisões, pendências)
    <Nome>Definition.cs        monta body, master, SurvivorDef (usa helpers do Framework)
    <Nome>Config.cs            números de balanceamento (dano, cooldown) em um só lugar
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
