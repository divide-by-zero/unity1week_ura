using UnityEditor;
using UnityEngine;

public class TMP_HologramShaderGUI : TMPro.EditorUtilities.TMP_SDFShaderGUI
{
    private static readonly string[] _hologramProps = {
        "_ScanlineCount",
        "_ScanlineSpeed",
        "_ScanlineIntensity",
        "_FlickerSpeed",
        "_FlickerIntensity",
        "_GlitchIntensity",
        "_GlitchFrequency",
    };

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hologram", EditorStyles.boldLabel);

        foreach (var name in _hologramProps)
        {
            var prop = FindProperty(name, properties, false);
            if (prop != null)
            {
                materialEditor.ShaderProperty(prop, prop.displayName);
            }
        }
    }
}
