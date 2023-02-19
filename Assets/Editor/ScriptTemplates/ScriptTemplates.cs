using System.IO;
using UnityEditor;

namespace WaynGroup.Mgm.Ability.Editor
{
    internal class ScriptTemplates
    {
        [MenuItem("Assets/Create/DOTS/System&Component&Authoring")]
        public static void CreateSystemAndComponentAndAuthoring()
        {
            string path = EditorUtility.SaveFilePanelInProject("Choose name", "", "cs", "Please select file name to save your script to:");
            if(!string.IsNullOrEmpty(path)) 
            {
                var fileName = Path.GetFileNameWithoutExtension(path);

                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                    $"Assets/Editor/ScriptTemplates/IComponentData.txt",
                    fileName + ".cs");
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                    $"Assets/Editor/ScriptTemplates/UnmanagedSystem.txt",
                    fileName + "System.cs");
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                    $"Assets/Editor/ScriptTemplates/AuthoringComponent.txt",
                    fileName + "Authoring.cs");
            }
        }

        [MenuItem("Assets/Create/DOTS/IAspect")]
        public static void CreateIAspect()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"Assets/Editor/ScriptTemplates/IAspect.txt",
                "IAspect.cs");
        }

        [MenuItem("Assets/Create/DOTS/Unmanaged System")]
        public static void CreateUnmanagedSystem()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"Assets/Editor/ScriptTemplates/UnmanagedSystem.txt",
                "UnmanagedSystem.cs");
        }

        [MenuItem("Assets/Create/DOTS/Authoring Component")]
        public static void CreateAuthoringComponent()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"Assets/Editor/ScriptTemplates/AuthoringComponent.txt",
                "AuthoringComponent.cs");
        }

        [MenuItem("Assets/Create/DOTS/IComponentData")]
        public static void CreateIComponentData()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"Assets/Editor/ScriptTemplates/IComponentData.txt",
                "IComponentData.cs");
        }
        [MenuItem("Assets/Create/DOTS/IBufferElementData")]
        public static void CreateIBufferElementData()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"Assets/Editor/ScriptTemplates/IBufferElementData.txt",
                "IBufferElementData.cs");
        }
        [MenuItem("Assets/Create/DOTS/Hybrid Component")]
        public static void CreateHybridComponent()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                $"Assets/Editor/ScriptTemplates/HybridComponent.txt",
                "HybridComponent.cs");
        }

    }
}