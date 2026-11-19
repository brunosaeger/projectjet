# Módulo: Tools/Blender

Integração do Claude com o Blender. ← [Tools](../context.md) · [Art](../../art/context.md)

## Dois modos de integração

### 1. Headless (pronto)
O Claude roda scripts bpy sem abrir a interface:
```powershell
./tools/blender/run.ps1 <script.py> [-Blend arquivo.blend] [-ScriptArgs "--a","b"]
```
| Script (`scripts/`) | Função |
|---|---|
| `inspect_scene.py` | Relatório técnico: tris, modificadores, materiais, ossos, transformações não aplicadas |
| `export_fbx.py` | Exporta FBX com convenções do Unity (`--out <fbx>`, `--no-anim`) |

Regras: scripts são idempotentes, recebem parâmetros após `--` e **nunca salvam por cima do .blend** sem flag explícita.

### 2. Ao vivo: MCP for Blender (pendente)
Addon + servidor MCP que deixam o Claude ver e editar a cena **aberta** em tempo real (ler objetos, rodar código, screenshot da viewport).
Requer aprovação do usuário. Passos:
```powershell
uvx mcp-for-blender install-addon        # instala o addon no Blender
claude mcp add blender uvx mcp-for-blender
```
Depois, no Blender: Preferências → Add-ons → ativar **MCP for Blender**. Na viewport, `N` → aba MCP → **Start MCP Server**.

Segurança: o servidor escuta em `localhost:9876` sem autenticação e executa Python arbitrário.
Manter só local e salvar o arquivo antes de pedir mudanças grandes.

## Quando usar cada um
- **Ao vivo:** modelagem guiada, revisão do que o usuário está fazendo, ajustes rápidos na cena aberta.
- **Headless:** tarefas repetíveis do pipeline (export, validação, batch), sempre reproduzíveis por script versionado.
