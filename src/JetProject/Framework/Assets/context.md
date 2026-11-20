# Submódulo: Framework/Assets

Carregamento dos AssetBundles produzidos pelo projeto [Unity](../../../../unity/context.md). ← [Framework](../context.md)

## Tipos
| Tipo | Papel |
|---|---|
| `AssetBundleService` | Serviço. `GetBundle(nome)` com cache e `Load<T>(bundle, asset)` com log de erro claro |
| `AssetsModule` | Registra o serviço |
| `GameAssets` | Estático. `Load<T>(chave)` carrega assets **do jogo base** via Addressables (síncrono) |

## Chaves Addressables: use GUIDs, nunca caminhos
O jogo carrega assets **por GUID**. Passar o caminho (`"RoR2/Base/.../CommandoBody.prefab"`) dá `InvalidKeyException`,
mesmo com o caminho aparecendo no catálogo.

Use as constantes do RoR2BepInExPack (namespace `RoR2BepInExPack.GameAssetPathsBetter`, uma classe por pasta do jogo):
```csharp
using RoR2BepInExPack.GameAssetPathsBetter;
GameAssets.Load<GameObject>(RoR2_Base_Commando.CommandoBody_prefab);   // = "64ee0a44..."
```
Os nomes das classes seguem o caminho **lógico** antigo (`RoR2_Base_Commando`), não a pasta física atual.
Para descobrir uma constante: procurar no `RoR2BepInExPack.xml` do perfil ou `ilspycmd -l c` na DLL.

## Convenções
- Bundles ficam em `Resources/AssetBundles/<nome>` (ver [Resources](../../Resources/context.md)).
- Um bundle por survivor (ex.: `<survivor>assets`). Recursos compartilhados podem ganhar um bundle `shared`.
- Consumidores pedem por **nome de asset**, nunca por caminho de arquivo.
- Assets do jogo base (materiais, shaders, efeitos vanilla) vêm de `GameAssets`, nunca do `AssetBundleService`.

## Shaders
O RoR2 usa shaders próprios (Hopoo, `HGStandard`). Materiais feitos no Unity com shader stub precisam ser trocados
pelo shader real em runtime. Quando chegarmos lá, essa troca entra como responsabilidade deste submódulo.
