"""Relatório técnico da cena: objetos, malhas (tris, modificadores, transformações não aplicadas),
armatures (ossos) e materiais. Serve para revisar se o modelo está pronto para exportar ao Unity."""
import bpy


def mesh_tris(obj):
    mesh = obj.evaluated_get(bpy.context.evaluated_depsgraph_get()).to_mesh()
    tris = sum(len(p.vertices) - 2 for p in mesh.polygons)
    obj.evaluated_get(bpy.context.evaluated_depsgraph_get()).to_mesh_clear()
    return tris


def main():
    print("=" * 60)
    print(f"Blender {bpy.app.version_string} | arquivo: {bpy.data.filepath or '(sem arquivo)'}")
    print(f"Unidade: {bpy.context.scene.unit_settings.system}, escala {bpy.context.scene.unit_settings.scale_length}")
    total_tris = 0
    for obj in bpy.context.scene.objects:
        issues = []
        if obj.type not in {"MESH", "ARMATURE"}:
            print(f"- {obj.name} [{obj.type}]")
            continue
        if tuple(round(s, 4) for s in obj.scale) != (1.0, 1.0, 1.0):
            issues.append(f"escala não aplicada {tuple(round(s, 3) for s in obj.scale)}")
        if any(abs(r) > 1e-4 for r in obj.rotation_euler):
            issues.append("rotação não aplicada")
        line = f"- {obj.name} [{obj.type}]"
        if obj.type == "MESH":
            tris = mesh_tris(obj)
            total_tris += tris
            mods = ", ".join(m.type for m in obj.modifiers) or "nenhum"
            mats = ", ".join(s.material.name for s in obj.material_slots if s.material) or "nenhum"
            line += f" tris={tris} mods=({mods}) mats=({mats})"
        elif obj.type == "ARMATURE":
            line += f" ossos={len(obj.data.bones)}"
        if issues:
            line += "  ⚠ " + "; ".join(issues)
        print(line)
    print(f"Total de triângulos: {total_tris}")
    print("=" * 60)


main()
