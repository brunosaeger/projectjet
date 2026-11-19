# Submódulo: Framework/Core

Ciclo de vida dos módulos e comunicação entre eles. ← [Framework](../context.md)

## Tipos
| Tipo | Papel |
|---|---|
| `IModule` | Contrato de um módulo: `Name` + `Initialize(ModContext)` |
| `ModuleHost` | Inicializa módulos na ordem de registro; loga e relança falhas indicando o módulo culpado |
| `ModContext` | `PluginDirectory`, `Config` (BepInEx ConfigFile) e um registro de serviços por tipo |
| `Log` | Fachada estática sobre o `ManualLogSource` do plugin |

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
