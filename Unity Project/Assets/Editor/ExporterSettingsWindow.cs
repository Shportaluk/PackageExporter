using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

namespace StansAssets.PackageExport.Editor
{
    public class ExporterSettingsWindow : EditorWindow
    {
        [MenuItem("Stan's Assets/ExporterSettingsWindow")]
        public static void OpenSettingsTest()
        {
            GetWindow<ExporterSettingsWindow>().Show();
        }
        static ListRequest _listRequest;
        
        static List<UnityEditor.PackageManager.PackageInfo> packageInfos = new List<UnityEditor.PackageManager.PackageInfo>();
        static void Progress()
        {
            packageInfos = new List<UnityEditor.PackageManager.PackageInfo>();
            if (_listRequest.IsCompleted)
            {
                if (_listRequest.Status == StatusCode.Success)
                {
                    foreach (UnityEditor.PackageManager.PackageInfo packageInfo in _listRequest.Result)
                    {
                        packageInfos.Add(packageInfo);
                    }

                }
                else if (_listRequest.Status >= StatusCode.Failure)
                {
                    Debug.Log(_listRequest.Error.message);
                }

                EditorApplication.update -= Progress;
            }
        }
        private void OnEnable()
        {
            _listRequest = Client.List();
            EditorApplication.update += Progress;

            //AssetDatabase.ExportPackage("Assets/StansAssets/Plugins/com.unity.probuilder@4.0.5", "D:/test.unitypackage", ExportPackageOptions.Recurse);
            //AssetDatabase.DeleteAsset("Assets/StansAssets/Plugins/com.unity.probuilder@4.0.5");
            //PackageExporter.ExportPackage("Assets/StansAssets/Plugins/" + "com.unity.probuilder@4.0.5");
            //AssetDatabase.DeleteAsset("Assets/StansAssets/Plugins/" + "com.unity.probuilder@4.0.5");
            var c = packageInfos;
            string s = "";
        }
        Vector2 scrollPos;
        void OnGUI()
        {
            EditorGUILayout.LabelField("Exporter Packages", EditorStyles.toolbarButton);
            if (packageInfos != null && packageInfos.Count > 0)
            {
                Color[] colors = new Color[] { Color.white, Color.black };
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(340), GUILayout.Height(470));
                for (int i = 0; i < packageInfos.Count; i++)
                {
                    //GUI.backgroundColor = colors[i % 2];
                    GUI.contentColor = Color.white;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(packageInfos[i].name, EditorStyles.largeLabel);

                    if( GUILayout.Button("Export") )
                    {
                        PackageExporter.ExportPackage(packageInfos[i].assetPath, packageInfos[i].resolvedPath);
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                this.Repaint();
            }
            else
            {
                EditorGUILayout.LabelField("Loading ...", EditorStyles.centeredGreyMiniLabel);
            }
        }
    }
}
