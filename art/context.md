# Módulo: Art

Fontes de arte editáveis. ← [CONTEXT.md](../CONTEXT.md)
**Divisão de papéis:** o usuário é dono da direção artística (forma, silhueta, estilo, cor). O Claude cuida da parte técnica
(topologia, rig, pesos, UV, export, limites de performance) e guia cada passo.

## Estrutura
```
art/
  concept/      desenhos do usuário (versionados)
    refs/       referências de terceiros: só locais, gitignored (não redistribuir no repo público)
  blender/      arquivos .blend (modelo, rig, animações)
  textures/     texturas fonte (psd, kra, substance) e exportadas (png)
  audio/source/ áudios originais (mp3/wav); entram no jogo via wwise/sounds.json + tools/audio/build-soundbank.ps1
```

## Autoria
Tudo em `art/` (exceto `concept/refs/`, que não é versionado) é **produção original do usuário**, incluindo
os desenhos de conceito e os áudios (`audio/source/normaldeagle.mp3`, `audio/source/newdeagle.mp3`).
Ao adicionar um asset de terceiros, registre aqui a fonte e a licença antes de versionar.

## Convenções técnicas (alvo RoR2)
- **Escala:** 1 unidade Blender = 1 metro. Altura de survivor ≈ 1,5 a 2,5 m (referência: Commando ≈ 1,8 m).
- **Orientação:** personagem olhando para **-Y** no Blender, de pé no eixo Z, origem nos pés.
- **Antes de exportar:** aplicar escala e rotação (Ctrl+A). Rodar `inspect_scene.py` para conferir.
- **Orçamento:** ~8k a 15k triângulos para o survivor. Texturas 1024 a 2048 px.
- **Rig:** um armature, ossos de deformação com nomes claros (`spine`, `upperArm.L`...). Sem leaf bones.
- **Nomes:** inglês, PascalCase para objetos (`SurvivorBody`, `SurvivorWeapon`), sem espaços nem acentos.
- **Animações:** uma Action por animação, nomes no padrão `Idle`, `Run`, `Sprint`, `Jump`, `Primary1`...

## Pipeline
`art/blender/*.blend` → [tools/blender](../tools/blender/context.md) `export_fbx.py` → `unity/Assets/JetProject/Models/` → [unity](../unity/context.md)
