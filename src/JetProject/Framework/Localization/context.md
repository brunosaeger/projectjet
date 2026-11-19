# Submódulo: Framework/Localization

Integra os arquivos de idioma do mod ao sistema nativo do RoR2. ← [Framework](../context.md)

## Como funciona
`LocalizationModule` adiciona `<plugin>/Resources/Language` em `Language.collectLanguageRootFolders`.
O RoR2 então lê todos os `.json` de cada subpasta de idioma.

## Formato
```
Resources/Language/
  en/JetProject.json       ← idioma base (obrigatório: é o fallback)
  pt-BR/JetProject.json
```
```json
{ "strings": { "JETPROJECT_MOD_NAME": "JetProject" } }
```

## Regras
- Todo token começa com `JETPROJECT_` (constante `PluginInfo.TokenPrefix`).
- Todo token novo entra em **en e pt-BR** ao mesmo tempo.
- Conforme crescer, divida por assunto (`Survivor.json`, `Skills.json`…). Todos os arquivos da pasta são lidos.
- O conteúdo dos textos (lore, descrições) é decidido em [design/](../../../../design/context.md).
  Este módulo só cuida da mecânica.
