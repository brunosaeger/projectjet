# Submódulo: Framework/Content

Ponte entre o mod e o sistema de ContentPacks do RoR2. ← [Framework](../context.md)

## Tipos
| Tipo | Papel |
|---|---|
| `ContentRegistry` | Serviço. API `AddBody`, `AddSurvivor`, `AddSkill`, `AddEntityState<T>`, `AddBuff`, `AddProjectile`… |
| `JetContentPackProvider` | `IContentPackProvider`: copia o registro para o `ContentPack` em `LoadStaticContentAsync` |
| `ContentModule` | Cria o registry, registra como serviço e assina `ContentManager.collectContentPackProviders` |

## Ciclo de vida
1. `Awake` do plugin: módulos de conteúdo chamam `ContentRegistry.Add*`.
2. O jogo carrega conteúdo e chama `LoadStaticContentAsync`. O registro é copiado e **selado**.
3. Qualquer `Add*` depois disso lança exceção, porque o conteúdo do RoR2 é imutável após o carregamento.

## Regras
- **Todo** conteúdo do mod passa por aqui. Não chame `ContentPack` diretamente de outros módulos.
- Todo `EntityState` usado por skills precisa ser registrado com `AddEntityState<T>()`, senão quebra em rede.
- Para um tipo de conteúdo ainda não suportado (ex.: `itemDefs`, `unlockableDefs`), adicione a lista + método
  `Add*` + linha em `PopulateAndSeal`, e atualize esta tabela.
