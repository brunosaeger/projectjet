# JetProject: CONTEXT orquestrador

Mod de **Risk of Rain 2** que adiciona um **survivor 100% original** (personagem, kit, arte, lore).
Nada pode ser copiado de personagens existentes, seja do RoR2 ou de outros jogos.

**Survivor: Jet**, piloto de jetpack com voo livre controlado pela mira, limitado por combustível.
Conceito em [design/concept.md](design/concept.md), habilidades em [design/kit.md](design/kit.md).

Este arquivo é o ponto de entrada. Ele traz as diretrizes globais e aponta para o `context.md` de cada módulo.
**Antes de mexer em um módulo, leia o context.md dele.**

---

## Diretrizes globais

1. **Arquitetura antes de código.** Antes de escrever algo novo, pergunte: *"isso pertence a este módulo, ou merece outro lugar?"*.
   Se a resposta não for óbvia, o código provavelmente pertence a um módulo novo ou ao Framework.
2. **Separação em camadas** (dependências só "para baixo"):
   ```
   JetPlugin (composition root)
        │
        ├── Characters/…      conteúdo específico (survivor, skills, states)
        │        ↓ usa
        └── Framework/…       infraestrutura genérica e reutilizável (não conhece nenhum survivor)
                 ↓ usa
           RoR2 / BepInEx / Unity
   ```
   O Framework **nunca** referencia `JetProject.Characters`.
3. **Módulos conversam por serviços**, nunca por estáticos globais. Use `ModContext.Register` / `Resolve` (ver [Core](src/JetProject/Framework/Core/context.md)).
4. **Um arquivo, uma responsabilidade.** Arquivos grandes viram pastas. Cada EntityState de skill fica no seu próprio arquivo.
5. **Todo módulo tem `context.md`.** Ao criar um módulo, crie o context.md dele e registre na tabela abaixo.
   Ao mudar o comportamento de um módulo, atualize o context.md **na mesma mudança**.
6. **Texto visível ao jogador nunca fica hardcoded.** Use sempre tokens de idioma com prefixo `JETPROJECT_` e mantenha `en` e `pt-BR`.
7. **Código em inglês, documentação em português.** Identificadores, namespaces e nomes de assets em inglês. Comentários e context.md em pt-BR.
8. **Multiplayer desde o início.** Lógica de gameplay respeita autoridade (`isAuthority`, `NetworkServer.active`). Nada pode funcionar só em single-player.

---

## Mapa de módulos

| Módulo | Caminho | Responsabilidade |
|---|---|---|
| Build | [build/context.md](build/context.md) | MSBuild compartilhado, caminhos da máquina, deploy automático no perfil de teste |
| Plugin | [src/JetProject/context.md](src/JetProject/context.md) | Projeto C# do mod: visão geral, composition root, camadas |
| Framework | [src/JetProject/Framework/context.md](src/JetProject/Framework/context.md) | Regras da camada genérica |
| ├ Core | [Framework/Core/context.md](src/JetProject/Framework/Core/context.md) | Ciclo de vida dos módulos, serviços, log |
| ├ Content | [Framework/Content/context.md](src/JetProject/Framework/Content/context.md) | Registro de conteúdo → ContentPack do RoR2 |
| ├ Assets | [Framework/Assets/context.md](src/JetProject/Framework/Assets/context.md) | AssetBundles do mod e assets do jogo (Addressables) |
| ├ Localization | [Framework/Localization/context.md](src/JetProject/Framework/Localization/context.md) | Integração com o sistema de idiomas do RoR2 |
| ├ Bodies | [Framework/Bodies/context.md](src/JetProject/Framework/Bodies/context.md) | Clonagem de bodies e configuração de máquinas de estado |
| ├ Skills | [Framework/Skills/context.md](src/JetProject/Framework/Skills/context.md) | Criação de SkillDefs e famílias |
| ├ Survivors | [Framework/Survivors/context.md](src/JetProject/Framework/Survivors/context.md) | Criação de SurvivorDefs |
| ├ UI | [Framework/UI/context.md](src/JetProject/Framework/UI/context.md) | Medidor de recurso no HUD |
| ├ Combat | [Framework/Combat/context.md](src/JetProject/Framework/Combat/context.md) | Helpers de combate (recuo de tiro) |
| ├ Audio | [Framework/Audio/context.md](src/JetProject/Framework/Audio/context.md) | Registro dos soundbanks Wwise do mod |
| ├ Projectiles | [Framework/Projectiles/context.md](src/JetProject/Framework/Projectiles/context.md) | Clonagem de projéteis, arco, explosão em área |
| ├ Interactables | [Framework/Interactables/context.md](src/JetProject/Framework/Interactables/context.md) | Spawn de baús e outros interagíveis |
| ├ Commands | [Framework/Commands/context.md](src/JetProject/Framework/Commands/context.md) | Comandos de teste no chat (`.help`) |
| └ Effects | [Framework/Effects/context.md](src/JetProject/Framework/Effects/context.md) | Cópias modificadas de efeitos visuais do jogo |
| Characters | [src/JetProject/Characters/context.md](src/JetProject/Characters/context.md) | Survivors do mod |
| └ Jet | [Characters/Jet/context.md](src/JetProject/Characters/Jet/context.md) | O survivor Jet: passiva, skills, states, config |
| Resources | [src/JetProject/Resources/context.md](src/JetProject/Resources/context.md) | Dados distribuídos com a DLL (idiomas, bundles) |
| Design | [design/context.md](design/context.md) | Conceito, kit de habilidades, balanceamento, lore |
| Art | [art/context.md](art/context.md) | Fontes de arte (Blender, texturas, concept art) |
| Unity | [unity/context.md](unity/context.md) | Projeto Unity que gera os AssetBundles |
| Wwise | [wwise/context.md](wwise/context.md) | Projeto Wwise + manifesto de sons que gera o soundbank |
| Tools | [tools/context.md](tools/context.md) | Scripts de desenvolvimento (perfil, launch) |
| └ Blender | [tools/blender/context.md](tools/blender/context.md) | Automação bpy headless + integração MCP |

---

## Pipeline de ponta a ponta

```
design/ (conceito) ─► art/ (Blender) ─► tools/blender (export FBX) ─► unity/ (prefabs, AssetBundle)
                                                                            │
                                    src/JetProject/Resources/AssetBundles ◄─┘
                                                     │
src/JetProject (C#) ─► dotnet build ─► deploy automático ─► perfil r2modman "JetDev" ─► tools/launch.ps1
```

## Referências externas
Consultar em caso de dúvida no desenvolvimento. Novos links indicados pelo usuário entram aqui.

| Link | Assunto |
|---|---|
| [R2Wiki: Character](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Character/) | Criação de personagens (modelo, rig, prefab, setup no Unity) |
| [R2Wiki: WWise Getting Started](https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Sounds/WWise/Getting-Started) | Sons próprios via Wwise (o áudio do Unity é desativado no RoR2) |

**Atenção:** guias da comunidade podem estar desatualizados (ex.: o jogo hoje carrega assets por GUID).
Na dúvida, a fonte da verdade é o código decompilado do jogo (`ilspycmd` no `RoR2.dll`).

## Ambiente

| Item | Valor |
|---|---|
| Jogo | `D:\SteamLibrary\steamapps\common\Risk of Rain 2` (Unity **2021.3.45f2**) |
| Unity Editor | `D:\Unity\2021.3.45f1\Editor\Unity.exe` (mesma linha de versão do jogo) |
| Blender | `D:\blender.exe` (5.1) |
| ffmpeg | winget `Gyan.FFmpeg` (conversão de áudio) |
| Wwise | `D:\Audiokinetic\Wwise2023.1.4.8496` (bankVersion 150, igual ao jogo) |
| .NET | SDK 8, alvo `netstandard2.1` |
| Mod loader | BepInExPack 5.4.2122, perfil r2modman `JetDev` |

## Comandos rápidos

```powershell
dotnet build                     # compila e copia para o perfil JetDev
./tools/launch.ps1 -Build        # compila e abre o jogo modado
./tools/setup-profile.ps1        # (re)cria o perfil de teste (BepInEx + mods de profile-mods.txt)
./tools/install-mods.ps1         # só (re)instala os mods do perfil
./tools/audio/build-soundbank.ps1  # sons: wwise/sounds.json → soundbank Wwise → Resources/SoundBanks
./tools/blender/run.ps1 inspect_scene.py -Blend <arquivo.blend>
```

## Estado atual

- [x] Ambiente instalado (.NET 8, Unity 2021.3.45f1, Unity Hub, r2modman, uv)
- [x] Framework base (Core, Content, Assets, Localization) compilando e com deploy automático
- [x] Ferramentas de Blender headless
- [ ] Integração Blender MCP (aguardando aprovação do usuário, ver tools/blender/context.md)
- [x] Repositório público: https://github.com/brunosaeger/projectjet
- [x] Conceito do survivor: visual, emblema, passiva e as 4 skills (detalhes finos em design/kit.md)
- [ ] Projeto Unity (unity/)
- [x] Protótipo do Jet compilando: passiva Jetpack completa + primária (modelo provisório do Commando)
- [x] Protótipo carrega no jogo: Jet aparece na seleção e entra na partida (2026-11-19)
- [ ] Feedback de gameplay do usuário (voo, combustível, primária) → ajustes em design/balance.md
- [x] Primária e passiva aprovadas pelo usuário (2026-11-20)
- [x] Secundária (UES Safe Travels Supplies): sinalizador em arco, mira no ar, poça de fogo, feixe e baús
- [x] Sons próprios via Wwise (pipeline automatizado) e comandos de teste no chat (`.help`)
- [x] Utilitária (DO A BARREL ROLL! // ACE THE SKIES!)
- [x] Visuais e sons da jetpack (jatos com chama piscando, fumaça de chão, ignição/loop/aviso de combustível)
- [ ] Especial
