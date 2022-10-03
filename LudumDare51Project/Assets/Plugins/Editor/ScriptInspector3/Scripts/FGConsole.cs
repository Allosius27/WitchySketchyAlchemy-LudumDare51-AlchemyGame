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

namespace ScriptInspector
{

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

public class FGConsole : EditorWindow
#if !UNITY_3_5 && !UNITY_4_0
	, IHasCustomMenu
#endif
{
	const BindingFlags staticMemberFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	const BindingFlags instanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	
	private static System.Type consoleConstantsType = System.Type.GetType("UnityEditor.ConsoleWindow+Constants,UnityEditor");
	private static FieldInfo errorStyleField;
	private static FieldInfo warningStyleField;
	private static FieldInfo logStyleField;
	private static FieldInfo errorSmallStyleField;
	private static FieldInfo warningSmallStyleField;
	private static FieldInfo logSmallStyleField;
	private static FieldInfo messageStyleField;
	
	public readonly static System.Type consoleWindowType;
	private static FieldInfo consoleWindowField;
	private static FieldInfo consoleListViewField;
	private static FieldInfo consoleLVHeightField;
	private static FieldInfo consoleActiveTextField;
	private static MethodInfo consoleOnGUIMethod;
	private static MethodInfo consoleAddItemsToMenuMethod;
	private static System.Type listViewStateType;
	private static FieldInfo listViewStateRowField;
	private static FieldInfo editorWindowPosField;
	private static System.Type logEntriesType;
	private static MethodInfo getEntryMethod;
	private static MethodInfo startGettingEntriesMethod;
	private static MethodInfo endGettingEntriesMethod;
	private static System.Type logEntryType;
	private static FieldInfo logEntryLineField;
	private static FieldInfo logEntryFileField;
	private static FieldInfo logEntryInstanceIDField;
	private static object logEntry;
	
	public static int repaintOnUpdateCounter = 0;
	
	private static Font font;
	private static GUIContent monospacedContentButtonStyle = new GUIContent("Monospaced Font");
	
	private static bool openLogEntriesInSi2
	{
		get { return EditorPrefs.GetBool("ScriptInspector.OpenLogEntriesInSi2", true); }
		set { EditorPrefs.SetBool("ScriptInspector.OpenLogEntriesInSi2", value); }
	}
	
	public void AddItemsToMenu(GenericMenu menu)
	{
		if (consoleAddItemsToMenuMethod != null)
		{
			var console = consoleWindowField.GetValue(null) as EditorWindow;
			if (console)
			{
				try
				{
					consoleAddItemsToMenuMethod.Invoke(console, new [] {menu});
					return;
				}
				catch { }
			}
		}
		if (Application.platform == RuntimePlatform.OSXEditor)
			menu.AddItem(new GUIContent("Open Player Log"), false, new GenericMenu.MenuFunction( UnityEditorInternal.InternalEditorUtility.OpenPlayerConsole));
		menu.AddItem(new GUIContent("Open Editor Log"), false, new GenericMenu.MenuFunction(UnityEditorInternal.InternalEditorUtility.OpenEditorConsole));
	}
	
	static FGConsole()
	{
		consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
		if (consoleWindowType != null)
		{
			consoleWindowField = consoleWindowType.GetField("ms_ConsoleWindow", staticMemberFlags);
			consoleListViewField = consoleWindowType.GetField("m_ListView", instanceMemberFlags);
			consoleLVHeightField = consoleWindowType.GetField("ms_LVHeight", instanceMemberFlags);
			consoleActiveTextField = consoleWindowType.GetField("m_ActiveText", instanceMemberFlags);
			consoleOnGUIMethod = consoleWindowType.GetMethod("OnGUI", instanceMemberFlags);
			consoleAddItemsToMenuMethod = consoleWindowType.GetMethod("AddItemsToMenu", instanceMemberFlags);

			if (consoleConstantsType != null)
			{
				errorStyleField = consoleConstantsType.GetField("ErrorStyle", staticMemberFlags);
				errorSmallStyleField = consoleConstantsType.GetField("ErrorSmallStyle", staticMemberFlags);
				warningStyleField = consoleConstantsType.GetField("WarningStyle", staticMemberFlags);
				warningSmallStyleField = consoleConstantsType.GetField("WarningSmallStyle", staticMemberFlags);
				logStyleField = consoleConstantsType.GetField("LogStyle", staticMemberFlags);
				logSmallStyleField = consoleConstantsType.GetField("LogSmallStyle", staticMemberFlags);
				messageStyleField = consoleConstantsType.GetField("MessageStyle", staticMemberFlags);
			}
		}
		listViewStateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
		if (listViewStateType != null)
		{
			listViewStateRowField = listViewStateType.GetField("row", instanceMemberFlags);
		}
		editorWindowPosField = typeof(EditorWindow).GetField("m_Pos", instanceMemberFlags);
		logEntriesType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries")
			?? typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.LogEntries");
		if (logEntriesType != null)
		{
			getEntryMethod = logEntriesType.GetMethod("GetEntryInternal", staticMemberFlags);
			startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", staticMemberFlags);
			endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", staticMemberFlags);
		}
		logEntryType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntry")
			?? typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.LogEntry");
		if (logEntryType != null)
		{
			logEntry = System.Activator.CreateInstance(logEntryType);
			logEntryFileField = logEntryType.GetField("file", instanceMemberFlags);
			logEntryLineField = logEntryType.GetField("line", instanceMemberFlags);
			logEntryInstanceIDField = logEntryType.GetField("instanceID", instanceMemberFlags);
		}
	}

	[MenuItem("Window/Script Inspector 3/SI Console", false, 800)]
	public static void ShowConsole()
	{
		GetWindow(consoleWindowType).Show();
		var siConsole = GetWindow<FGConsole>("SI Console", consoleWindowType);
		siConsole.Show();
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		siConsole.title = "SI Console";
#else
		siConsole.titleContent.text = "SI Console";
#endif
	}
	
	public static FGConsole FindInstance()
	{
		UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(FGConsole));
		return (objArray.Length <= 0) ? null : (objArray[0] as EditorWindow) as FGConsole;
	}

	public static void OpenIfConsoleIsOpen()
	{
		EditorWindow console = FindInstance();
		if (console != null)
			return;

		UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(consoleWindowType);
		console = (objArray.Length <= 0) ? null : (objArray[0] as EditorWindow);
		if (console != null)
		{
			EditorWindow wnd = EditorWindow.focusedWindow;
			FGConsole.ShowConsole();
			if (wnd != null)
				wnd.Focus();
		}
	}

	private static void OnLog(string logString, string stackTrace, LogType type)
	{
		if (repaintOnUpdateCounter == 0)
			repaintOnUpdateCounter = 10;
	}

	protected void OnEnable()
	{
		EditorApplication.update -= OnApplicationUpdate;
		EditorApplication.update += OnApplicationUpdate;
		EditorApplication.update -= OnFirstAppUpdate;
		EditorApplication.update += OnFirstAppUpdate;
	}

	protected void OnDisable()
	{
		EditorApplication.update -= OnApplicationUpdate;
		EditorApplication.update -= OnFirstAppUpdate;
#if UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5
		Application.RegisterLogCallback(null);
#else
		Application.logMessageReceived -= OnLog;
#endif
	}

	protected static void OnFirstAppUpdate()
	{
		EditorApplication.update -= OnFirstAppUpdate;
#if UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5
		Application.RegisterLogCallback(OnLog);
#else
		Application.logMessageReceived += OnLog;
#endif
		repaintOnUpdateCounter = 1;
	}
	
	protected void OnApplicationUpdate()
	{
		if (repaintOnUpdateCounter > 0)
		{
			if (--repaintOnUpdateCounter == 0)
				Repaint();
		}
	}

	protected void OnGUI()
	{
		if (Event.current.isKey && TabSwitcher.OnGUIGlobal())
			return;
		
		EditorWindow console = consoleWindowField.GetValue(null) as EditorWindow;
		if (console == null)
		{
			EditorGUILayout.HelpBox(@"Script Inspector Console can only work when the Console tab is also open.

Click the button below to open the Console window...", MessageType.Info);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open Console Widnow"))
			{
				GetWindow(consoleWindowType);
				Focus();
				Repaint();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			return;
		}

		Rect oldPosition = console.position;
		editorWindowPosField.SetValue(console, position);

		try
		{
			bool contextClick = Event.current.type == EventType.ContextClick ||
				Event.current.type == EventType.MouseUp && Event.current.button == 1 && Application.platform == RuntimePlatform.OSXEditor;
			if (contextClick && GUIUtility.hotControl == 0 &&
				Event.current.mousePosition.y > EditorStyles.toolbar.fixedHeight)
			{
				int lvHeight = (int) consoleLVHeightField.GetValue(console);
				if (lvHeight > Event.current.mousePosition.y - EditorStyles.toolbar.fixedHeight)
				{
					Event.current.type = EventType.MouseDown;
					Event.current.button = 0;
					Event.current.clickCount = 1;
					try { consoleOnGUIMethod.Invoke(console, null); } catch { }
					GUIUtility.hotControl = 0;
					
					DoPopupMenu(console);
				}
			}
			else if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && Event.current.mousePosition.y > EditorStyles.toolbar.fixedHeight
				|| Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				OpenLogEntry(console);
				GUIUtility.hotControl = 0;
				GUIUtility.ExitGUI();
			}
			try { consoleOnGUIMethod.Invoke(console, null); } catch { }
			
#if UNITY_2019_3_OR_NEWER
			var rc = new Rect(413f, -1f, 160f, 18f);
#elif UNITY_2017_1_OR_NEWER
			var rc = new Rect(355f, -1f, 144f, 18f);
#else
			var rc = new Rect(254f, -1f, 144f, 18f);
#endif
			var autoFocusText = SISettings.autoFocusConsole == 0 ? "Auto-Focus: Never" :
				SISettings.autoFocusConsole == 1 ? "Auto-Focus: On Error" : "Auto-Focus: On Compile";
			if (GUI.Button(rc, autoFocusText, EditorStyles.toolbarDropDown))
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Never"), SISettings.autoFocusConsole == 0,
					() => { SISettings.autoFocusConsole.Value = 0; });
				menu.AddItem(new GUIContent("On Compile Error"), SISettings.autoFocusConsole == 1,
					() => { SISettings.autoFocusConsole.Value = 1; });
				menu.AddItem(new GUIContent("On Compile"), SISettings.autoFocusConsole == 2,
					() => { SISettings.autoFocusConsole.Value = 2; });
				menu.DropDown(rc);
			}

			if (font == null && SISettings.monospacedFontConsole)
			{
				font = FGTextEditor.LoadEditorResource<Font>("Smooth Fonts/DejaVu Sans Mono.ttf");
				SetConsoleFont(font);
			}

			rc.xMin = rc.xMax + 4f;
			rc.xMax = rc.xMin + EditorStyles.toolbarButton.CalcSize(monospacedContentButtonStyle).x;
			if (SISettings.monospacedFontConsole != GUI.Toggle(rc, SISettings.monospacedFontConsole, monospacedContentButtonStyle, EditorStyles.toolbarButton))
			{
				SISettings.monospacedFontConsole.Value = !SISettings.monospacedFontConsole;
				
				if (font == null && SISettings.monospacedFontConsole)
				{
					font = FGTextEditor.LoadEditorResource<Font>("Smooth Fonts/DejaVu Sans Mono.ttf");
				}

				SetConsoleFont(SISettings.monospacedFontConsole ? font : null);
			}
		}
		finally
		{
			editorWindowPosField.SetValue(console, oldPosition);
		}
	}
	
	private static void SetFontField(FieldInfo field, Font font)
	{
		if (field == null)
			return;
		GUIStyle style = field.GetValue(null) as GUIStyle;
		if (style == null)
			return;

		style.font = font;
		style.fontSize = font != null ? 11 : 0;
	}
	
	private static void SetConsoleFont(Font font)
	{
		SetFontField(errorStyleField, font);
		SetFontField(errorSmallStyleField, font);
		SetFontField(warningStyleField, font);
		SetFontField(warningSmallStyleField, font);
		SetFontField(logStyleField, font);
		SetFontField(logSmallStyleField, font);
		SetFontField(messageStyleField, font);
	}
	
	private struct CallStackTarget
	{
		public string guid;
		public int line;
		public string functionName;
	}
	
	private class CallStackFrames: CallStackFramesBase
	{
		public CallStackFrames(string stackTrace) : base(stackTrace) {}
	}
		
	private class CallStackFramesBase
	{
		string stackTrace;
		public CallStackFramesBase(string stackTrace) { this.stackTrace = stackTrace; }
		public CallStackFramesEnumerator GetEnumerator() { return new CallStackFramesEnumerator(stackTrace); }
    }
	
	private class CallStackFramesEnumerator : CallStackFramesEnumeratorBase
	{
		public CallStackFramesEnumerator(string stackTrace) : base(stackTrace) {}
	}
	
	private class CallStackFramesEnumeratorBase
	{
		int index;
		string[] lines;
		CallStackTarget current;

		public CallStackFramesEnumeratorBase(string stackTrace)
		{
			lines = stackTrace.Split('\n');
			index = -1;
			current = default(CallStackTarget);
		}
		public void Reset() { index = -1; }

		public CallStackTarget Current { get { return current; } }

		public bool MoveNext()
		{
			while (++index < lines.Length)
			{
				var textLine = lines[index];
				
				var atAssetsIndex = textLine.IndexOf("(at Assets/");
				if (atAssetsIndex >= 0)
				{
					var lineIndex = textLine.LastIndexOf(':');
					if (lineIndex <= atAssetsIndex)
						continue;
					
					current.line = 0;
					for (var i = lineIndex + 1; i < textLine.Length; ++i)
					{
						var c = textLine[i];
						if (c < '0' || c > '9')
							break;
						current.line = current.line * 10 + (c - '0');
					}
					
					atAssetsIndex += "(at ".Length;
					var assetPath = textLine.Substring(atAssetsIndex, lineIndex - atAssetsIndex);
					
					current.guid = AssetDatabase.AssetPathToGUID(assetPath);
					if (!string.IsNullOrEmpty(current.guid))
					{
						var functionNameEnd = textLine.IndexOf('(');
						if (functionNameEnd < 0)
							continue;
						current.functionName = textLine.Substring(0, functionNameEnd).TrimEnd(' ');

						return true;
					}
				}
				else
				{
					var indexFromFullStackLogging = textLine.IndexOf(" (Mono JIT Code) [");
					if (indexFromFullStackLogging < 0)
						continue;

					var fileNameStartIndex = indexFromFullStackLogging + " (Mono JIT Code) [".Length;
					var fileNameEndIndex = textLine.IndexOf(':', fileNameStartIndex);
					if (fileNameEndIndex <= fileNameStartIndex)
						continue;

					var lineStartIndex = fileNameEndIndex + 1;
					var lineEndIndex = textLine.IndexOf("] ", lineStartIndex);
					if (lineEndIndex <= lineStartIndex)
						continue;

					var fileName = textLine.Substring(fileNameStartIndex, fileNameEndIndex - fileNameStartIndex);
					if (!fileName.EndsWithCS())
						continue;

					var assets = AssetDatabase.FindAssets(fileName.Substring(0, fileName.Length - ".cs".Length));
					if (assets == null || assets.Length == 0)
						continue;

					var end1 = '/' + fileName;
					var end2 = '\\' + fileName;

					var paths = new List<string>(assets.Length);
					for (var i = assets.Length; i --> 0; )
					{
						var path = AssetDatabase.GUIDToAssetPath(assets[i]);
						if (path.EndsWith(end1, System.StringComparison.Ordinal) || path.EndsWith(end2, System.StringComparison.Ordinal))
						{
							current.guid = assets[i];
							paths.Add(path);
						}
					}

					if (paths.Count == 0)
						continue;
					
					var functionNameStart = lineStartIndex + 2;

					current.line = 0;
					for (var i = lineStartIndex; i < textLine.Length; ++i)
					{
						var c = textLine[i];
						if (c < '0' || c > '9')
							break;
						current.line = current.line * 10 + (c - '0');
						++functionNameStart;
					}
					
					var functionNameEnd = textLine.IndexOf('(', functionNameStart);
					if (functionNameEnd - 1 > functionNameStart)
					{
						var functionName = textLine.Substring(functionNameStart, functionNameEnd - 1 - functionNameStart);
						var start = functionName.LastIndexOf(':');
						if (start > 0)
							functionName = functionName.Substring(start + 1);
						current.functionName = functionName;
					}

					return true;
				}
			}
			
			return false;
		}
	}

	private static void OpenLogEntry(EditorWindow console)
	{
		var listView = consoleListViewField.GetValue(console);
		int listViewRow = listView != null ? (int)listViewStateRowField.GetValue(listView) : -1;
		if (listViewRow >= 0)
		{
			bool gotIt = false;
			startGettingEntriesMethod.Invoke(null, new object[0]);
			try {
				gotIt = (bool) getEntryMethod.Invoke(null, new object[] { listViewRow, logEntry });
			} finally {
				endGettingEntriesMethod.Invoke(null, new object[0]);
			}
			
			if (gotIt)
			{
				int line = (int)logEntryLineField.GetValue(logEntry);
				string file = (string)logEntryFileField.GetValue(logEntry);
				string guid = null;
				
				file = file.Replace('\\', '/');
				if (file.StartsWithIgnoreCase("assets/"))
				{
					guid = AssetDatabase.AssetPathToGUID(file);
					if (string.IsNullOrEmpty(guid))
					{
						int instanceID = (int)logEntryInstanceIDField.GetValue(logEntry);
						if (instanceID != 0)
						{
							file = AssetDatabase.GetAssetPath(instanceID);
							if (file.StartsWithIgnoreCase("assets/"))
								guid = AssetDatabase.AssetPathToGUID(file);
						}
					}
				}
				else
				{
					int instanceID = (int)logEntryInstanceIDField.GetValue(logEntry);
					if (instanceID != 0)
					{
						file = AssetDatabase.GetAssetPath(instanceID);
						if (file.StartsWithIgnoreCase("assets/"))
							guid = AssetDatabase.AssetPathToGUID(file);
					}
				}
				
				if (string.IsNullOrEmpty(guid))
				{
					string text = (string)consoleActiveTextField.GetValue(console);
					if (!string.IsNullOrEmpty(text))
					{
						var frames = new CallStackFramesEnumerator(text);
						if (frames.MoveNext())
						{
							line = frames.Current.line;
							guid = frames.Current.guid;
						}
					}
				}
				
				if (!string.IsNullOrEmpty(guid))
				{
					bool openInSI = (openLogEntriesInSi2 || SISettings.handleOpenAssets) && !SISettings.dontOpenAssets;
					if (EditorGUI.actionKey)
						openInSI = !openInSI;
					if(openInSI)
					{
						FGCodeWindow.addRecentLocationForNextAsset = true;
						FGCodeWindow.OpenAssetInTab(guid, line);
					}
					else
					{
						var assetPath = AssetDatabase.GUIDToAssetPath(guid);
						if (assetPath.StartsWithIgnoreCase("assets/"))
						{
							FGCodeWindow.openInExternalIDE = true;
							AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(assetPath), line);
						}
					}
				}
			}
		}
	}

	private void DoPopupMenu(EditorWindow console)
	{
		var listView = consoleListViewField.GetValue(console);
		int listViewRow = listView != null ? (int) listViewStateRowField.GetValue(listView) : -1;

		if (listViewRow < 0)
			return;

		string text = (string)consoleActiveTextField.GetValue(console);
		if (string.IsNullOrEmpty(text))
			return;
		
		GenericMenu codeViewPopupMenu = new GenericMenu();
		var i = 0;

		foreach (var stackFrame in new CallStackFrames(text))
		{
			if (!string.IsNullOrEmpty(stackFrame.guid))
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(stackFrame.guid);
				var scriptName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
				var functionName = stackFrame.functionName != null ? " - " + stackFrame.functionName.Replace(':', '.') : "";
				codeViewPopupMenu.AddItem(
					new GUIContent(i + ": " + scriptName + functionName + ", line " + stackFrame.line),
					false,
					() => {
						bool openInSI = (openLogEntriesInSi2 || SISettings.handleOpenAssets) && !SISettings.dontOpenAssets;
						if (EditorGUI.actionKey)
							openInSI = !openInSI;
						if (openInSI)
						{
							FGCodeWindow.addRecentLocationForNextAsset = true;
							FGCodeWindow.OpenAssetInTab(stackFrame.guid, stackFrame.line);
						}
						else
						{
							FGCodeWindow.openInExternalIDE = true;
							AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(assetPath), stackFrame.line);
						}
					});
				++i;
			}
		}
		if (codeViewPopupMenu.GetItemCount() > 0)
		{
			codeViewPopupMenu.AddSeparator(string.Empty);
			if (SISettings.handleOpenAssets || SISettings.dontOpenAssets)
				codeViewPopupMenu.AddDisabledItem(
					new GUIContent("Open Call-Stack Entries in Script Inspector")
				);
			else
				codeViewPopupMenu.AddItem(
					new GUIContent("Open Call-Stack Entries in Script Inspector"),
					openLogEntriesInSi2,
					() => { openLogEntriesInSi2 = !openLogEntriesInSi2; }
				);

			GUIUtility.hotControl = 0;
			codeViewPopupMenu.ShowAsContext();
			GUIUtility.ExitGUI();
		}
	}
}

}
