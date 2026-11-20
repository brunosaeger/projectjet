# Módulo: Wwise

Projeto Wwise que gera o soundbank do mod. ← [CONTEXT.md](../CONTEXT.md)

## Por que Wwise
O RoR2 **desativa o áudio do Unity** (confirmado: `AudioSettings.outputSampleRate == 0`). Todo som passa pelo Wwise,
então os sons do mod precisam estar num soundbank Wwise.

## Versão
**Wwise 2023.1.4.8496** em `D:\Audiokinetic\Wwise2023.1.4.8496` (instalado pelo Audiokinetic Launcher, conta do usuário).
O bank gerado precisa ter **bankVersion 150**, a mesma dos `.bnk` do jogo. O script valida isso.
Não trocar de versão sem conferir.

## Estrutura
```
wwise/
  sounds.json            MANIFESTO: lista de sons (nome, fonte em art/audio/source, volume em dB)
  JetProject/            projeto Wwise (estrutura versionada; Originals/ e GeneratedSoundBanks/ são gerados)
```

## Fluxo para adicionar/alterar um som
1. Coloque o áudio fonte em `art/audio/source/` (qualquer formato que o ffmpeg leia).
2. Adicione/edite a entrada em `sounds.json`: `{ "name": "Jet_<Skill>_<Evento>", "source": "...", "volumeDb": 0 }`.
3. Rode `./tools/audio/build-soundbank.ps1` (sem abrir o Wwise).
4. Adicione a constante `Play_<name>` em `Characters/<Survivor>/<Survivor>Sounds.cs` e toque com `Util.PlaySound`.

Sons removidos do manifesto são removidos do projeto pelo script.

**Normalização:** o script normaliza cada som para **pico em -1 dBFS** (após converter para mono) antes de importar.
Assim, fontes gravadas baixas ou altas entram no mesmo nível. O `volumeDb` do manifesto é só **mixagem relativa**
(ex.: -3 para um som que deve ficar atrás dos outros). Para subir tudo acima do pico, usar `volumeDb` positivo.

## Como o projeto está montado (feito pelo script, via WAAPI)
| Objeto | Configuração | Por quê |
|---|---|---|
| Bus `Master Audio Bus\SFX_BUS\HDR\Character` | Só existe para gerar o **mesmo ID** (436743010) do bus Character do jogo | Os sons herdam volume Master/SFX e a mixagem HDR do jogo |
| Actor-Mixer `JetProject` | Saída no bus Character, 3D (posição), atenuação ligada | Som posicional como os sons vanilla |
| Attenuation `JetProject_Default` | RadiusMax 100 m | Alcance parecido com tiros vanilla |
| Eventos `Play_<nome>` | Uma ação Play por som | Nome previsível para o código |
| SoundBank `JetProject` | Inclui todos os eventos (com mídia embutida) | Um único `.bnk` autossuficiente |

## Regras
- **Não editar o projeto à mão** para algo que o script define: a próxima execução sobrescreve. Se precisar de algo
  novo (randomização de pitch, containers, RTPCs), evolua o script e o manifesto.
- Nada de busses próprios com nomes novos: o Init.bnk do jogo não os conhece, e o som sairia sem a mixagem do jogo.
- Referência: [R2Wiki: WWise Getting Started](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Sounds/WWise/Getting-Started).
