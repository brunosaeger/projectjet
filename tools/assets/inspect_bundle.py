"""Lista componentes e campos relevantes de GameObjects num asset bundle do RoR2 (somente leitura)."""
import sys
import UnityPy

bundle_path = sys.argv[1]
targets = sys.argv[2].split(",")
env = UnityPy.load(bundle_path)

objs_by_pathid = {}
for obj in env.objects:
    objs_by_pathid[obj.path_id] = obj


def name_of_pptr(pptr):
    try:
        if pptr is None or (isinstance(pptr, dict) and pptr.get("m_PathID", 0) == 0):
            return None
        pid = pptr["m_PathID"] if isinstance(pptr, dict) else pptr.path_id
        fid = pptr["m_FileID"] if isinstance(pptr, dict) else pptr.file_id
        if fid != 0:
            return f"<externo file={fid} id={pid}>"
        o = objs_by_pathid.get(pid)
        if not o:
            return f"<id {pid}>"
        d = o.read_typetree()
        return d.get("m_Name") or f"<{o.type.name} {pid}>"
    except Exception as e:
        return f"<erro {e}>"


def script_name(mono):
    try:
        s = mono.get("m_Script")
        o = objs_by_pathid.get(s["m_PathID"]) if s and s.get("m_FileID", 1) == 0 else None
        if o:
            return o.read_typetree().get("m_ClassName")
        return f"<script externo id={s.get('m_PathID')}>"
    except Exception:
        return "?"


INTEREST = ("sound", "Sound", "effect", "Effect", "shake", "Shake", "amplitude", "radius", "Radius", "lifetime",
            "duration", "frequency", "ghost", "Ghost", "wwise", "Event", "event", "child", "Child", "prefab", "Prefab")


def dump_gameobject(go_obj, depth=0, max_depth=3):
    go = go_obj.read_typetree()
    pad = "  " * depth
    print(f"{pad}GameObject '{go.get('m_Name')}'")
    transform_children = []
    for comp in go.get("m_Component", []):
        c = comp["component"]
        o = objs_by_pathid.get(c["m_PathID"])
        if not o:
            continue
        tname = o.type.name
        try:
            data = o.read_typetree()
        except Exception as e:
            print(f"{pad}  [{tname}] (sem typetree: {e})")
            continue
        if tname == "MonoBehaviour":
            cls = script_name(data)
            keys = [k for k in data.keys() if not k.startswith("m_")]
            print(f"{pad}  [{cls}] campos: {', '.join(keys)[:300]}")
            for k, v in data.items():
                if k.startswith("m_") and k not in ("m_Enabled",):
                    continue
                if any(s in k for s in INTEREST) or isinstance(v, (str, float, int, bool)):
                    if isinstance(v, dict) and "m_PathID" in v:
                        v = name_of_pptr(v)
                    elif isinstance(v, dict) and "m_AssetGUID" in v:
                        v = f"Addressable {v.get('m_AssetGUID')}"
                    print(f"{pad}      {k} = {str(v)[:160]}")
        elif tname in ("Transform", "RectTransform"):
            transform_children = data.get("m_Children", [])
        else:
            print(f"{pad}  [{tname}]")
    if depth < max_depth:
        for ch in transform_children:
            t = objs_by_pathid.get(ch["m_PathID"])
            if not t:
                continue
            tgo = t.read_typetree().get("m_GameObject")
            go_child = objs_by_pathid.get(tgo["m_PathID"]) if tgo else None
            if go_child:
                dump_gameobject(go_child, depth + 1, max_depth)


for obj in env.objects:
    if obj.type.name != "GameObject":
        continue
    try:
        name = obj.read_typetree().get("m_Name")
    except Exception:
        continue
    if name in targets:
        print("=" * 70)
        dump_gameobject(obj)
