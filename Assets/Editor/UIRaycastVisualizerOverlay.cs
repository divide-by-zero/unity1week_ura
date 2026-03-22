using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace TohokuEpco.Editor
{
    [Overlay(typeof(SceneView), "ui-raycast-visualizer", "UI Raycast Visualizer")]
    public class UIRaycastVisualizerOverlay : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            root.style.minWidth = 200;
            root.style.paddingTop = 4;
            root.style.paddingBottom = 4;

            var toggle = new Toggle("Enable")
            {
                value = UIRaycastVisualizerBehaviour.IsEnabled,
            };
            toggle.RegisterValueChangedCallback(evt =>
            {
                UIRaycastVisualizerBehaviour.IsEnabled = evt.newValue;
                SceneView.RepaintAll();
            });

            var slider = new Slider("Opacity", 0f, 1f)
            {
                value = UIRaycastVisualizerBehaviour.Opacity,
            };
            slider.RegisterValueChangedCallback(evt =>
            {
                UIRaycastVisualizerBehaviour.Opacity = evt.newValue;
                SceneView.RepaintAll();
            });

            var depthSlider = new Slider("DepthBand", 0f, 1f)
            {
                value = UIRaycastVisualizerBehaviour.Opacity,
            };
            depthSlider.RegisterValueChangedCallback(evt =>
            {
                UIRaycastVisualizerBehaviour.DepthRange = evt.newValue * 100f;
                SceneView.RepaintAll();
            });

            root.Add(toggle);
            root.Add(slider);
            root.Add(depthSlider);
            return root;
        }
    }
}
