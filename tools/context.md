# Módulo: Tools

Scripts de desenvolvimento. Não são distribuídos com o mod. ← [CONTEXT.md](../CONTEXT.md)

| Script | O que faz |
|---|---|
| `setup-profile.ps1` | Cria ou repara o perfil r2modman `JetDev` com o BepInExPack mais recente (`-Force` reinstala) e chama o `install-mods.ps1` |
| `install-mods.ps1` | Instala no perfil os mods de `profile-mods.txt` (última versão, dependências resolvidas pelo Thunderstore) |
| `profile-mods.txt` | Lista de mods do perfil: dependências de runtime (R2API) e ferramentas de dev (DebugToolkit) |
| `launch.ps1` | Abre o RoR2 modado com o perfil (via Steam + args do Doorstop 4). `-Build` compila antes |
| `blender/` | Automação do Blender → [blender/context.md](blender/context.md) |
| `assets/inspect-particles.ps1` | Resume os ParticleSystems de um prefab (vida, tamanho, cor) para identificar qual peça de um efeito é qual |
| `assets/inspect-bundle.ps1` | Inspeciona prefabs dentro dos asset bundles do jogo (componentes, campos, filhos) via UnityPy/uv. Use quando o código decompilado não mostra os VALORES (efeitos, sons, tremor, raios) |
| `audio/build-soundbank.ps1` | Lê `wwise/sounds.json`, monta o projeto Wwise via WAAPI, gera e valida o `.bnk` e copia para `Resources/SoundBanks`. Requer Wwise 2023.1.4 + ffmpeg. Ver [wwise/context.md](../wwise/context.md) |

## Notas
- `launch.ps1` exige que o jogo tenha sido aberto **uma vez** pelo r2modman (ele instala o `winhttp.dll` na pasta do jogo).
- Log do jogo modado: `%APPDATA%\r2modmanPlus-local\RiskOfRain2\profiles\JetDev\BepInEx\LogOutput.log`.
- **RoR2BepInExPack é obrigatório** e fica explícito em `profile-mods.txt`: o R2API depende dele (e do `Newtonsoft.Json`
  que ele traz, já que o jogo não inclui mais essa DLL), mas isso não está declarado nas dependências do Thunderstore.
  Sintoma quando falta: `Could not load file or assembly 'Newtonsoft.Json'` no log e nenhum mod do R2API carrega.
- Mods instalados pelo `install-mods.ps1` não aparecem na lista do r2modman (ele não escreve o `mods.yml`), mas carregam normalmente.
- DebugToolkit: no console do jogo (tecla `~`), `give_item Feather 5` e `spawn_ai <nome>` ajudam nos testes.
- Comandos próprios no **chat** (`.help` lista todos): ver `src/JetProject/Framework/Commands/context.md`.
- Scripts novos: um propósito por script, parâmetros com default sensato, e entrada nesta tabela.
