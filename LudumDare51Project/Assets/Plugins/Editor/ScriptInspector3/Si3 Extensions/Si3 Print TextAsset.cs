/* SCRIPT INSPECTOR 3
 * version 3.0.33, May 2022
 * Copyright © 2012-2022, Flipbook Games
 * 
 * Script Inspector 3 - World's Fastest IDE for Unity
 * 
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

#if UNITY_EDITOR_WIN
namespace ScriptInspector
{	
	using UnityEngine;
	using UnityEditor;
	using System.Diagnostics;
	
	internal static class PrintTextAsset
	{
		[MenuItem("Assets/Print with Notepad++", true, 19)]
		private static bool ValidatePrint()
		{
			return
				Selection.activeObject &&
				Selection.activeObject is TextAsset &&
				Selection.objects.Length == 1;
		}
	
		[MenuItem("Assets/Print with Notepad++", false, 19)]
		private static void Print(MenuCommand command)
		{
			var asset = Selection.activeObject as TextAsset;
			if (asset == null)
				return;
			
			var path = AssetDatabase.GetAssetPath(asset);
			if (string.IsNullOrEmpty(path))
				return;
				
			path = System.IO.Path.GetFullPath(path);
			Process.Start("CMD.exe", "/C start notepad++ -quickPrint \"" + path + "\"");
		}
	}
}
#endif
