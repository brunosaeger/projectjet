# Submódulo: Framework/Core

Ciclo de vida dos módulos e comunicação entre eles. ← [Framework](../context.md)

## Tipos
| Tipo | Papel |
|---|---|
| `IModule` | Contrato de um módulo: `Name` + `Initialize(ModContext)` |
| `ModuleHost` | Inicializa módulos na ordem de registro; loga e relança falhas indicando o módulo culpado |
| `ModContext` | `PluginDirectory`, `Config` (BepInEx ConfigFile) e um registro de serviços por tipo |
| `Log` | Fachada estática sobre o `ManualLogSource` do plugin |
| `ConfigRevision` | `ResetIfOutdated(config, revisão)`: quando a revisão sobe, todas as entradas do `.cfg` voltam ao padrão do código |

## Config e valores padrão
O BepInEx grava os valores no `.cfg`; mudar um padrão no código **não** muda o valor de quem já tem o arquivo.
Por isso, cada conjunto de configs chama `ConfigRevision.ResetIfOutdated` depois dos `Bind`, e a revisão é
incrementada sempre que um padrão muda (ex.: `JetConfig.DefaultsRevision`).

## Padrão de serviços
```csharp
// módulo provedor
context.Register(new MeuServico());
// módulo consumidor (registrado DEPOIS no JetPlugin)
var svc = context.Resolve<MeuServico>();
```
- Um tipo, uma instância. `Register` duplicado lança exceção.
- `Resolve` de algo inexistente lança exceção com dica de ordem de registro.
- Não use singletons estáticos para serviços. A exceção é `Log`, por ser transversal e sem estado de jogo.

## Não pertence aqui
Qualquer coisa que dependa de RoR2 (content, idiomas, assets). Core depende só de BepInEx.
