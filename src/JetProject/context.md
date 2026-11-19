# Módulo: Plugin (src/JetProject)

Projeto C# que vira `JetProject.dll`, o plugin BepInEx. ← [CONTEXT.md](../../CONTEXT.md)

## Estrutura
```
JetPlugin.cs        composition root: cria o ModContext, registra módulos em ordem
PluginInfo.cs       GUID, nome, versão, prefixo de tokens
Framework/          camada genérica (não conhece survivors)   → Framework/context.md
Characters/         conteúdo: survivors do mod                → Characters/context.md
Resources/          dados copiados junto da DLL               → Resources/context.md
```

## Composition root (`JetPlugin`)
- É o **único** lugar que instancia módulos e decide a ordem: serviços do Framework primeiro, conteúdo depois.
- Não contém lógica de jogo. Se algo além de `_host.Add(...)` aparecer no `Awake`, está no lugar errado.

## Dependências
- Compilação: BepInEx.Core 5.4.21 (NuGet), assemblies do jogo (instalação local, RoR2 publicizado).
- Runtime: apenas BepInExPack. **R2API ainda não é usado.** Adicione um submódulo do R2API somente quando resolver
  um problema real (ex.: `R2API.Prefab` para clonar prefabs em rede) e documente aqui o motivo e a dependência no manifest.

## Convenções
- Namespace = caminho da pasta (`JetProject.Framework.Content`, `JetProject.Characters.<Nome>.Skills`…).
- Classes `internal` por padrão. `public` só quando o RoR2/BepInEx exigir (plugin, EntityStates).
- Tokens: `JETPROJECT_<ESCOPO>_<NOME>` (ex.: `JETPROJECT_<SURVIVOR>_PRIMARY_NAME`).
