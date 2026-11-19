# Módulo: Unity

Projeto Unity que transforma arte em prefabs e **AssetBundles** para o mod. ← [CONTEXT.md](../CONTEXT.md)

## Estado
**Ainda não criado.** Será criado quando o primeiro modelo (mesmo provisório) for para o jogo.
Pré-requisito: login no Unity Hub com licença Personal ativa.

## Versão
**Unity 2021.3.45f1** (`D:\Unity\2021.3.45f1\Editor\Unity.exe`). O jogo roda 2021.3.45f2. Não trocar de versão.

## Estrutura planejada
```
unity/
  Assets/JetProject/
    Models/        FBX exportados do Blender (via tools/blender/export_fbx.py)
    Materials/     materiais (shader stub, trocado pelo real em runtime, ver Framework/Assets)
    Textures/
    Animations/    AnimatorControllers
    Prefabs/       prefab do modelo (mdl<Survivor>), efeitos, projéteis
    Icons/         ícones de skills e do survivor (sprites)
    Editor/        script de build dos AssetBundles
  AssetBundles/    saída (gitignored) → copiada para src/JetProject/Resources/AssetBundles
```

## Regras
- O Unity monta **apenas o visual** (modelo, animator, materiais, VFX, ícones). Lógica de gameplay vive no C# (`src/`).
- O nome do bundle segue o padrão de [Framework/Assets](../src/JetProject/Framework/Assets/context.md).
- Decisão pendente: usar ThunderKit/RoR2EditorKit (importa os scripts do jogo para o editor) ou um projeto limpo.
  Será decidido na criação, conforme a necessidade de componentes do RoR2 dentro do editor.
