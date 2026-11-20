# Submódulo: Framework/Survivors

Criação de `SurvivorDef` (entrada na tela de seleção). ← [Framework](../context.md)

`SurvivorFactory.Create(SurvivorInfo)`: preenche nome interno (`cachedName`), body, display, tokens, cor e posição na lista.
O chamador registra o resultado com `ContentRegistry.AddSurvivor`.

Futuro: desbloqueio (UnlockableDef) e requisito de expansão, se algum dia forem necessários.
