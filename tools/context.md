# Módulo: Tools

Scripts de desenvolvimento. Não são distribuídos com o mod. ← [CONTEXT.md](../CONTEXT.md)

| Script | O que faz |
|---|---|
| `setup-profile.ps1` | Cria ou repara o perfil r2modman `JetDev` com o BepInExPack mais recente do Thunderstore (`-Force` reinstala) |
| `launch.ps1` | Abre o RoR2 modado com o perfil (via Steam + args do Doorstop 4). `-Build` compila antes |
| `blender/` | Automação do Blender → [blender/context.md](blender/context.md) |

## Notas
- `launch.ps1` exige que o jogo tenha sido aberto **uma vez** pelo r2modman (ele instala o `winhttp.dll` na pasta do jogo).
- Log do jogo modado: `%APPDATA%\r2modmanPlus-local\RiskOfRain2\profiles\JetDev\BepInEx\LogOutput.log`.
- Mods de apoio ao desenvolvimento (ex.: DebugToolkit, para spawnar itens e inimigos) se instalam pelo r2modman no perfil `JetDev`.
- Scripts novos: um propósito por script, parâmetros com default sensato, e entrada nesta tabela.
