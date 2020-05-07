using System;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace StansAssets.PackageExport.Editor
{
	/// <summary>
	/// Use to export package as <c>.unitypackage</c>
	/// </summary>
	public static class PackageExporter
	{
		static SearchRequest s_ActiveSearchRequest;
		static string s_ExportDestination;

		/// <summary>
		/// Export package as <c>.unitypackage</c>
		/// </summary>
		/// <param name="packageInfo">Package info. The package to be exported</param>
		public static void ExportPackage(UnityEditor.PackageManager.PackageInfo packageInfo)
		{
			Debug.Log("Package name: " + packageInfo.name);
			var pathDestinationPackage = EditorUtility.SaveFilePanel("Save unitypackage", "", packageInfo.name.Replace(".", "_"), "unitypackage");
			//string pathDestinationPackage = "D:\\" + packageInfo.name.Replace(".", "_") + ".unitypackage";
			AssetDatabase.ExportPackage(packageInfo.assetPath, pathDestinationPackage, ExportPackageOptions.Recurse);
		}

		static void OnEditorApplication()
		{
			 if (s_ActiveSearchRequest.IsCompleted)
			 {
				 if (s_ActiveSearchRequest.Status == StatusCode.Success)
					 Export();
				 else if (s_ActiveSearchRequest.Status >= StatusCode.Failure)
					 Debug.LogError($"Export Failed: {s_ActiveSearchRequest.Error.message} Code: {s_ActiveSearchRequest.Error.errorCode}");
				 else
					 Debug.LogError($"Unsupported progress state {s_ActiveSearchRequest.Status}");
				 OnFinalize();
			 }
		}

		static void OnFinalize()
		{
			s_ExportDestination = null;
			EditorApplication.update -= OnEditorApplication;
		}

		static void Export()
		{
			var packageInfo = s_ActiveSearchRequest.Result.First();
			Debug.Log(packageInfo.assetPath);
		}
	}
}

