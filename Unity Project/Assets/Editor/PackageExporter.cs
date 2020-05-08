using System;
using System.Collections.Generic;
using System.IO;
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
		/// <param name="assetPath">Asset path. Asset path to package to be exported example: Packages/ProBuilder</param>
		/// <param name="resolvedPath">Resolved path. Full path to package to be exported example: C:/UnityProject/.../Library/PackageCache/ProBuilde@4.0.1</param>
		public static void ExportPackage(string assetPath, string resolvedPath)
		{
			if (assetPath.Contains("StansAssets"))
			{
				AssetDatabase.ExportPackage(assetPath, "D:/test.unitypackage", ExportPackageOptions.Recurse);
			}
			else
			{
				CopyAssetAndExport(resolvedPath);
			}
		}
		static void CopyAssetAndExport(string resolvedPath)
		{
			List<string> paths = new List<string>();

			var files = AssetDatabase.GetAllAssetPaths();
			string pathToPackageFolder = resolvedPath.Replace("\\", "/"); 
			string folderName = Path.GetFileName(pathToPackageFolder);

			// Application.dataPath = projectPath + "/Assets"  Example: C:/ProjectUnity/Exporter/Assets
			string pathDestination = Application.dataPath + "/StansAssets/Plugins/" + folderName;

			var pathDestinationPackage = EditorUtility.SaveFilePanel("Save unitypackage", "", folderName.Replace(".", "_"), "unitypackage");
			if (!string.IsNullOrEmpty(pathDestinationPackage))
			{
				Copy(pathToPackageFolder, pathDestination);
				AssetDatabase.Refresh();
				AssetDatabase.ExportPackage("Assets/StansAssets/Plugins/" + folderName, pathDestinationPackage, ExportPackageOptions.Recurse);
				AssetDatabase.DeleteAsset("Assets/StansAssets/Plugins/" + folderName);
			}
			else
			{
				return;
			}
		}
		static void Copy(string sourceDir, string targetDir)
		{
			if (!Directory.Exists(targetDir))
				Directory.CreateDirectory(targetDir);

			string destFileName;
			foreach (var file in Directory.GetFiles(sourceDir))
			{
				destFileName = Path.Combine(targetDir, Path.GetFileName(file));
				//if (!File.Exists(destFileName) && !destFileName.Contains(".asmdef"))
					File.Copy(file, destFileName);

			}

			foreach (var directory in Directory.GetDirectories(sourceDir))
				Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
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

