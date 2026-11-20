using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace JetProject.Framework.UI
{
    /// <summary>
    /// Barra vertical ao lado do crosshair, exibida apenas no HUD de quem controla/observa este body.
    /// Colocar no prefab do body junto de um componente que implemente <see cref="IResourceMeterSource"/>.
    /// Construída por código (sem assets), para funcionar antes de existir um AssetBundle.
    /// </summary>
    internal sealed class ResourceMeterHud : MonoBehaviour
    {
        private static readonly Vector2 Offset = new Vector2(48f, 0f);
        private static readonly Vector2 Size = new Vector2(6f, 64f);
        private static readonly Color BackgroundColor = new Color(0f, 0f, 0f, 0.55f);

        private IResourceMeterSource _source;
        private HUD _hud;
        private GameObject _root;
        private RectTransform _fill;
        private Image _fillImage;

        private void Awake() => _source = GetComponent<IResourceMeterSource>();

        private void Update()
        {
            if (_source == null)
                return;

            if (!_root && !TryBuild())
                return;

            _root.SetActive(_source.MeterVisible);
            _fill.anchorMax = new Vector2(1f, Mathf.Clamp01(_source.Fraction));
            _fillImage.color = _source.MeterColor;
        }

        private void OnDestroy()
        {
            if (_root)
                Destroy(_root);
        }

        private bool TryBuild()
        {
            _hud = FindHudTargetingThisBody();
            if (!_hud || !_hud.mainContainer)
                return false;

            _root = new GameObject("JetProject_ResourceMeter", typeof(RectTransform), typeof(Image));
            var rootRect = (RectTransform)_root.transform;
            rootRect.SetParent(_hud.mainContainer.transform, false);
            rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = Offset;
            rootRect.sizeDelta = Size;
            var background = _root.GetComponent<Image>();
            background.color = BackgroundColor;
            background.raycastTarget = false;

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            _fill = (RectTransform)fillObject.transform;
            _fill.SetParent(rootRect, false);
            _fill.anchorMin = Vector2.zero;
            _fill.anchorMax = Vector2.one;
            _fill.offsetMin = new Vector2(1f, 1f);
            _fill.offsetMax = new Vector2(-1f, -1f);
            _fillImage = fillObject.GetComponent<Image>();
            _fillImage.raycastTarget = false;
            return true;
        }

        private HUD FindHudTargetingThisBody()
        {
            foreach (var hud in HUD.readOnlyInstanceList)
            {
                if (hud && hud.targetBodyObject == gameObject)
                    return hud;
            }
            return null;
        }
    }
}
