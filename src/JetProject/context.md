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
- Runtime: BepInExPack + os submódulos do R2API abaixo. Só adicione um submódulo do R2API quando ele resolver
  um problema real, e registre aqui o motivo.

| Dependência | Motivo |
|---|---|
| `R2API.Prefab` 1.1.1 | Clonar bodies com registro de rede (`InstantiateClone`), gerando um NetworkIdentity próprio |
| `R2API.Sound` 1.0.3 | Registrar o soundbank Wwise do mod (`SoundAPI.SoundBanks.Add`) |
| `RoR2BepInExPack` 1.50.0 | Constantes de GUID dos assets do jogo (`GameAssetPathsBetter`); base exigida pelo R2API |

Ao adicionar: `PackageReference` no .csproj, `[BepInDependency]` no `JetPlugin`, linha em `tools/profile-mods.txt`.

**Atenção:** pacotes do R2API trazem transitivamente `RiskOfRain2.GameLibs`, `UnityEngine.Modules` e `MMHOOK.RoR2`
(versões antigas). Eles ficam excluídos (`ExcludeAssets="all"`) no .csproj para compilar contra a instalação local.
Se um pacote novo trouxer outra versão deles, atualize as versões excluídas.

## Convenções
- Namespace = caminho da pasta (`JetProject.Framework.Content`, `JetProject.Characters.<Nome>.Skills`…).
- Classes `internal` por padrão. `public` só quando o RoR2/BepInEx exigir (plugin, EntityStates).
- Tokens: `JETPROJECT_<ESCOPO>_<NOME>` (ex.: `JETPROJECT_<SURVIVOR>_PRIMARY_NAME`).
