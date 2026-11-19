# Módulo: Build

Infraestrutura MSBuild compartilhada por todos os projetos C#. ← [CONTEXT.md](../CONTEXT.md)

## Arquivos
| Arquivo | Papel |
|---|---|
| `../Directory.Build.props` | Importado automaticamente por todo .csproj: importa os caminhos e define LangVersion, nullable etc. |
| `../Directory.Build.targets` | Importa `Deploy.targets` só em projetos com `<IsRoR2Plugin>true</IsRoR2Plugin>` |
| `Paths.props` | Caminhos da máquina: `RoR2Dir`, `RoR2Managed`, `R2ProfileName`, `R2ProfileDir`, `DeployToProfile` |
| `Paths.user.props` | *(opcional, gitignored)* sobrescreve os caminhos em outra máquina |
| `Deploy.targets` | Após o build, copia DLL, PDB e `Resources/` para `<perfil>\BepInEx\plugins\<AssemblyName>\` |
| `../nuget.config` | Fontes NuGet: nuget.org + nuget.bepinex.dev |

## Regras
- Nunca coloque caminhos absolutos em `.csproj`; use as propriedades de `Paths.props`.
- Assemblies do jogo vêm **da instalação local** (`$(RoR2Managed)`), com `Private=false` (nunca são copiados).
- `RoR2.dll` é publicizado (`Publicize="true"`, via BepInEx.AssemblyPublicizer) para acessar membros privados sem reflection.
- Para compilar sem deploy: `dotnet build -p:DeployToProfile=false`.
- Se o deploy avisar que o perfil não existe, rode `tools/setup-profile.ps1`.
