"""Exporta a cena (malhas + armature) para FBX com as convenções que o Unity espera.
Uso: run.ps1 export_fbx.py -Blend <arquivo.blend> -ScriptArgs "--out","<caminho.fbx>" """
import argparse
import os
import sys

import bpy


def parse_args():
    argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", required=True, help="Caminho do .fbx de saída")
    parser.add_argument("--no-anim", action="store_true", help="Não exportar animações")
    return parser.parse_args(argv)


def main():
    args = parse_args()
    out = os.path.abspath(args.out)
    os.makedirs(os.path.dirname(out), exist_ok=True)
    bpy.ops.export_scene.fbx(
        filepath=out,
        object_types={"ARMATURE", "MESH", "EMPTY"},
        apply_unit_scale=True,
        apply_scale_options="FBX_SCALE_UNITS",
        axis_forward="-Z",
        axis_up="Y",
        use_mesh_modifiers=True,
        mesh_smooth_type="FACE",
        add_leaf_bones=False,
        use_armature_deform_only=True,
        bake_anim=not args.no_anim,
        bake_anim_use_all_actions=True,
        bake_anim_use_nla_strips=False,
        path_mode="AUTO",
    )
    print(f"FBX exportado: {out}")


main()
