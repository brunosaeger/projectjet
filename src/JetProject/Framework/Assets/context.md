# Submódulo: Framework/Assets

Carregamento dos AssetBundles produzidos pelo projeto [Unity](../../../../unity/context.md). ← [Framework](../context.md)

## Tipos
| Tipo | Papel |
|---|---|
| `AssetBundleService` | Serviço. `GetBundle(nome)` com cache e `Load<T>(bundle, asset)` com log de erro claro |
| `AssetsModule` | Registra o serviço |

## Convenções
- Bundles ficam em `Resources/AssetBundles/<nome>` (ver [Resources](../../Resources/context.md)).
- Um bundle por survivor (ex.: `<survivor>assets`). Recursos compartilhados podem ganhar um bundle `shared`.
- Consumidores pedem por **nome de asset**, nunca por caminho de arquivo.
- Assets do jogo base (materiais, shaders, efeitos vanilla) **não** vêm daqui. Use `Addressables`
  (`Addressables.LoadAssetAsync<T>("RoR2/Base/...")`). Quando isso for necessário, crie um helper aqui mesmo.

## Shaders
O RoR2 usa shaders próprios (Hopoo, `HGStandard`). Materiais feitos no Unity com shader stub precisam ser trocados
pelo shader real em runtime. Quando chegarmos lá, essa troca entra como responsabilidade deste submódulo.
