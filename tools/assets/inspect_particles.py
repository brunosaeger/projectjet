"""Resumo dos ParticleSystems de um prefab num asset bundle do RoR2 (somente leitura):
nome do objeto, duração, tempo de vida, tamanho, velocidade, cor inicial e material."""
import sys
import UnityPy

bundle_path, prefab_name = sys.argv[1], sys.argv[2]
env = UnityPy.load(bundle_path)
objs = {o.path_id: o for o in env.objects}


def read(pptr):
    o = objs.get(pptr["m_PathID"]) if pptr and pptr.get("m_FileID", 1) == 0 else None
    return o.read_typetree() if o else None


def curve_value(c):
    if not isinstance(c, dict):
        return c
    mode = c.get("minMaxState", 0)
    scalar = c.get("scalar")
    if mode == 3:  # dois constantes
        return f"{c.get('minScalar')}..{scalar}"
    return scalar


def color_value(g):
    if not isinstance(g, dict):
        return g
    col = g.get("maxColor") or {}
    return "rgba({:.2f},{:.2f},{:.2f},{:.2f})".format(col.get("r", 0), col.get("g", 0), col.get("b", 0), col.get("a", 0))


def walk(go_obj, depth=0):
    go = go_obj.read_typetree()
    pad = "  " * depth
    ps_info, renderer_info, children = None, None, []
    for comp in go.get("m_Component", []):
        o = objs.get(comp["component"]["m_PathID"])
        if not o:
            continue
        if o.type.name == "ParticleSystem":
            d = o.read_typetree()
            init = d.get("InitialModule", {})
            ps_info = (f"duração={d.get('lengthInSec')} loop={d.get('looping')} "
                       f"vida={curve_value(init.get('startLifetime'))} tamanho={curve_value(init.get('startSize'))} "
                       f"vel={curve_value(init.get('startSpeed'))} cor={color_value(init.get('startColor'))}")
        elif o.type.name == "ParticleSystemRenderer":
            d = o.read_typetree()
            mats = [read(m) for m in d.get("m_Materials", [])]
            renderer_info = ", ".join((m or {}).get("m_Name", "?") for m in mats)
        elif o.type.name == "Transform":
            children = o.read_typetree().get("m_Children", [])
    line = f"{pad}{go.get('m_Name')}"
    if ps_info:
        line += f"  [{ps_info}]  material: {renderer_info}"
    print(line)
    for ch in children:
        t = objs.get(ch["m_PathID"])
        if t:
            child_go = objs.get(t.read_typetree()["m_GameObject"]["m_PathID"])
            if child_go:
                walk(child_go, depth + 1)


for obj in env.objects:
    if obj.type.name == "GameObject":
        try:
            if obj.read_typetree().get("m_Name") == prefab_name:
                walk(obj)
        except Exception:
            pass
