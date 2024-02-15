using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Yarn.Unity.Addons.SpeechBubbles.Editor
{

    public class TMP_SDFJitterShaderGUI : TMP_SDFShaderGUI
    {
        protected override void DoGUI()
        {
            base.DoGUI();
            DoSlider("_NoiseScale", "Noise Scale");
            DoFloat("_NoiseSnap", "Noise Time Snap");
        }
    }

}