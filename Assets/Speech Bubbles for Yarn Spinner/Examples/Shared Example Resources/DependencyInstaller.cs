
using UnityEngine;

// This script exists to make sure that both Cinemachine and the Unity Input
// System, which the samples need, are installed and configured correctly. It
// detects if certain packages are loaded, and if they're not, shows a window
// that offers an easy way to install them.
//
// You can safely delete this script from your game.

namespace Yarn.Unity.Addons.SpeechBubbles.Example
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEditor.PackageManager.Requests;

    [ExecuteAlways]
    public class DependencyInstaller : MonoBehaviour
    {
        public void OnEnable()
        {
            if (!Application.isEditor)
            {
                return;
            }

            if (DependenciesInstallerTool.AreDependenciesAvailable == false)
            {
                DependenciesInstallerTool.Install();
            }
        }
    }

    public class DependenciesInstallerTool : EditorWindow
    {
        public static bool AreDependenciesAvailable => IsCinemachineInstalled && IsInputSystemInstalled && IsInputSystemEnabled;

        public static bool CheckAssemblyLoaded(string name)
        {
            var allLoadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allLoadedAssemblies)
            {
                if (assembly.FullName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCinemachineInstalled => CheckAssemblyLoaded("Cinemachine");

        public static bool IsInputSystemInstalled
        {
            get
            {
                bool isInputSystemDefined;
#if USE_INPUTSYSTEM
                isInputSystemDefined = true;
#else
                isInputSystemDefined = false;
                #endif
                return isInputSystemDefined && CheckAssemblyLoaded("Unity.InputSystem");
            }
        }

        public static bool IsInputSystemEnabled
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return true;
#else
                return false;
#endif
            }
        }

        public static void Install()
        {
            DependenciesInstallerTool window = EditorWindow.GetWindow<DependenciesInstallerTool>();
            window.titleContent = new GUIContent("Install Sample Dependencies");
            window.ShowUtility();
        }

        private Vector2 scrollViewPosition = Vector2.zero;

        void OnGUI()
        {
            scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);
            DrawDependencyInstallerGUI(isWindow: true);
            EditorGUILayout.EndScrollView();
        }

        internal static void DrawDependencyInstallerGUI(bool isWindow)
        {
            var wrap = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                richText = true
            };

            EditorGUILayout.LabelField("This sample requires <b>Cinemachine</b> and the <b>Unity Input System</b> to work. (The add-on doesn't need these packages - they're only needed by the samples.)", wrap);

            if (AreDependenciesAvailable)
            {
                string message;

                if (isWindow)
                {
                    message = "All dependencies for this sample are installed and enabled. You can close this window.";
                }
                else
                {
                    message = "All dependencies for this sample are installed and enabled. You can delete this object.";
                }
                EditorGUILayout.LabelField(message, wrap);
                return;
            }

            EditorGUILayout.LabelField("This sample requires some additional packages, and won't work correctly without them.", wrap);
            EditorGUILayout.Space();

            if (IsCinemachineInstalled == false)
            {
                EditorGUILayout.LabelField("This sample requires Cinemachine.", wrap);

                using (new EditorGUI.DisabledGroupScope(PackageInstaller.IsInstallationInProgress))
                {
                    if (GUILayout.Button("Install Cinemachine"))
                    {
                        PackageInstaller.Add("com.unity.cinemachine");
                    }
                }

                EditorGUILayout.Space();
            }
            if (IsInputSystemInstalled == false)
            {
                EditorGUILayout.LabelField("This sample requires the Unity Input System.", wrap);
                using (new EditorGUI.DisabledGroupScope(PackageInstaller.IsInstallationInProgress))
                {
                    if (GUILayout.Button("Install Input System"))
                    {
                        PackageInstaller.Add("com.unity.inputsystem");
                    }
                }
                EditorGUILayout.Space();
            }
            else if (IsInputSystemEnabled == false)
            {
                EditorGUILayout.LabelField("The Unity Input System is installed, but is not currently enabled.", wrap);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("To address this, open Project Settings, choose Player, and set Active Input Handling to 'Input System Package'.", wrap);

                using (new EditorGUI.DisabledGroupScope(PackageInstaller.IsInstallationInProgress))
                {
                    if (GUILayout.Button("Open Project Settings"))
                    {
                        EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
                    }
                }
                EditorGUILayout.Space();
            }

            if (PackageInstaller.IsInstallationInProgress)
            {
                EditorGUILayout.LabelField("Installation in progress...");
            }
        }
    }

    [CustomEditor(typeof(DependencyInstaller))]
    public class DependencyInstallerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DependenciesInstallerTool.DrawDependencyInstallerGUI(isWindow: false);
        }
    }

    public static class PackageInstaller
    {
        static AddRequest CurrentRequest;

        public static bool IsInstallationInProgress { get; private set; }

        public static void Add(string identifier)
        {
            CurrentRequest = Client.Add(identifier);
            EditorApplication.update += Progress;
            IsInstallationInProgress = true;
        }

        static void Progress()
        {
            if (CurrentRequest.IsCompleted)
            {
                IsInstallationInProgress = false;
                if (CurrentRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError($"Unable to install cinemachine: {CurrentRequest.Error.message}");
                }
                EditorApplication.update -= Progress;
                CurrentRequest = null;
            }
        }
    }

#else
    // A placeholder version of DependencyInstaller, to prevent Unity from
    // complaining about missing scripts.
    public class DependencyInstaller : MonoBehaviour { }
#endif
}
