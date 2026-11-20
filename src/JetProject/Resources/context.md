# Módulo: Resources

Dados (não código) distribuídos junto da DLL. ← [Plugin](../context.md)

Tudo aqui é copiado para a saída do build (`CopyToOutputDirectory`) e para o perfil de teste pelo deploy.

| Pasta | Conteúdo | Quem consome |
|---|---|---|
| `Language/<idioma>/*.json` | Tokens de texto | [Localization](../Framework/Localization/context.md) |
| `SoundBanks/*.bnk` | Soundbanks Wwise (gerados por `tools/audio/build-soundbank.ps1`) | [Audio](../Framework/Audio/context.md) |
| `AssetBundles/` *(futuro)* | Bundles gerados pelo [Unity](../../../unity/context.md) | [Assets](../Framework/Assets/context.md) |

## Regras
- Nada de arquivos-fonte aqui (.blend, .psd, cenas Unity). Fontes vivem em `art/` e `unity/`.
- Bundles são artefatos gerados. Copie-os do output do Unity, não edite à mão.
