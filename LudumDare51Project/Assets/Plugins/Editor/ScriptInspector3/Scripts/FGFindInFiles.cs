﻿/* SCRIPT INSPECTOR 3
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
using System.Collections.Generic;
using System.IO;


public static class FGFindInFiles
{
	public static List<string> assets;
	
	//private static List<string> ignoreFileTypes = new List<string> { ".dll", ".a", ".so", ".dylib", ".exe" };
	
	public static List<SymbolDeclaration> FindDeclarations(SymbolDefinition symbol)
	{
		symbol = symbol.GetGenericSymbol();
		
		var candidates = FindDefinitionCandidates(symbol);
		foreach (var c in candidates)
		{
			var asset = AssetDatabase.LoadAssetAtPath(c, typeof(TextAsset)) as TextAsset;
			if (!asset)
				continue;
			var buffer = FGTextBufferManager.GetBuffer(asset);
			buffer.LoadImmediately();
		}
		
		var newSymbol = symbol.Rebind();
		var declarations = newSymbol == null ? null : newSymbol.declarations;
		return declarations ?? symbol.declarations;
	}
	
	static List<string> FindDefinitionCandidates(SymbolDefinition symbol)
	{
		var result = new List<string>();
		if (assets != null)
			assets.Clear();
		
		var symbolType = symbol;
		if (symbol.kind == SymbolKind.Namespace)
			return result;
		
		while (symbolType != null &&
			symbolType.kind != SymbolKind.Class && symbolType.kind != SymbolKind.Struct &&
			symbolType.kind != SymbolKind.Enum && symbolType.kind != SymbolKind.Interface &&
			symbolType.kind != SymbolKind.Delegate)
		{
			symbolType = symbolType.parentSymbol;
		}
		
		var assembly = symbolType.Assembly;
		FindAllAssemblyScripts(assembly);
		for (int i = assets.Count; i --> 0; )
			assets[i] = AssetDatabase.GUIDToAssetPath(assets[i]);
		
		string[] words;
		string typeName = symbolType.name;
		switch (symbolType.kind)
		{
		case SymbolKind.Class: words = new [] { "class", typeName }; break;
		case SymbolKind.Struct: words = new [] { "struct", typeName }; break;
		case SymbolKind.Interface: words = new [] { "interface", typeName }; break;
		case SymbolKind.Enum: words = new [] { "enum", typeName }; break;
		case SymbolKind.Delegate: words = new [] { typeName, "(" }; break;
			default: return result;
		}
		
		for (int i = assets.Count; i --> 0; )
			if (ContainsWordsSequence(assets[i], words))
				result.Add(assets[i]);
		
		return result;
	}
	
	public static void FindAllReferences(SymbolDefinition symbol, string localAssetPath)
	{
		if (symbol.kind == SymbolKind.Accessor || symbol.kind == SymbolKind.Constructor || symbol.kind == SymbolKind.Destructor)
		{
			symbol = symbol.parentSymbol;
			if (symbol != null && symbol.kind == SymbolKind.MethodGroup)
				symbol = symbol.parentSymbol;
		}
		if (symbol == null)
			return;
		
		symbol = symbol.GetGenericSymbol();
		
		var candidates = FindReferenceCandidates(symbol, localAssetPath);
		
		var searchOptions = new FindResultsWindow.SearchOptions {
			text = symbol.name,
			matchWord = true,
			matchCase = true,
		};
		
		var candidateGuids = new string[candidates.Count];
		for (int i = 0; i < candidates.Count; i++)
			candidateGuids[i] = AssetDatabase.AssetPathToGUID(candidates[i]);
		
		var searchForVarRefs = symbol is TypeDefinitionBase;
		if (searchForVarRefs)
		{
			searchOptions.altText1 = "var";
			
			var builtInTypesEnumerator = SymbolDefinition.builtInTypes.GetEnumerator();
			for (var i = 0; i < 16; i++)
			{
				builtInTypesEnumerator.MoveNext();
				var type = builtInTypesEnumerator.Current.Value;
				if (type == symbol)
				{
					searchOptions.altText2 = builtInTypesEnumerator.Current.Key;
					break;
				}
			}
		}
		
		var resultsWindow = FindResultsWindow.Create(
			"References to " + symbol.FullName,
			FindAllInSingleFile,
			candidateGuids,
			searchOptions,
			"References");
		resultsWindow.SetFilesValidator(ValidateFileForReferences);
		resultsWindow.SetResultsValidator(ValidateResultAsReference, symbol);
	}
	
	public static void RenameSymbol(SymbolDefinition symbol, string localAssetPath)
	{
		Debug.Log("here");

		if (symbol.kind == SymbolKind.Accessor || symbol.kind == SymbolKind.TypeAlias || symbol.kind == SymbolKind.TupleType)
			return;
		
		if (symbol.kind == SymbolKind.Constructor || symbol.kind == SymbolKind.Destructor)
			symbol = symbol.parentSymbol;
		if (symbol == null)
			return;
		
		symbol = symbol.GetGenericSymbol();
		
		var assembly = symbol.Assembly;
		if (assembly == null)
			return;
		if (!assembly.fromCsScripts)
		{
			// Only symbols defined in C# scripts can be renamed
			return;
		}
		
		var candidates = FindReferenceCandidates(symbol, localAssetPath);
		
		var searchOptions = new FindResultsWindow.SearchOptions {
			text = symbol.name,
			matchWord = true,
			matchCase = true,
		};
		
		var candidateGuids = new string[candidates.Count];
		for (int i = 0; i < candidates.Count; i++)
			candidateGuids[i] = AssetDatabase.AssetPathToGUID(candidates[i]);
		
		//var searchForVarRefs = symbol is TypeDefinitionBase && symbol.kind != SymbolKind.Delegate;
		//if (searchForVarRefs)
		//{
		//	searchOptions.altText1 = "var";
			
		//	var builtInTypesEnumerator = SymbolDefinition.builtInTypes.GetEnumerator();
		//	for (var i = 0; i < 16; i++)
		//	{
		//		builtInTypesEnumerator.MoveNext();
		//		var type = builtInTypesEnumerator.Current.Value;
		//		if (type == symbol)
		//		{
		//			searchOptions.altText2 = builtInTypesEnumerator.Current.Key;
		//			break;
		//		}
		//	}
		//}
		
		Debug.Log(symbol.FullName);
		
		FindResultsWindow resultsWindow = FindResultsWindow.Create(
			"Rename " + symbol.FullName,
			FindAllInSingleFile,
			candidateGuids,
			searchOptions,
			"Rename");
		resultsWindow.SetFilesValidator(ValidateFileForReferences);
		resultsWindow.SetResultsValidator(ValidateResultAsReference, symbol);
		resultsWindow.SetReplaceText(symbol.name);
	}
	
	static bool ValidateFileForReferences(string assetGuid, FindResultsWindow.FilteringOptions options)
	{
		try
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
			var isCsScript = assetPath.EndsWithCS();
			if (isCsScript)
				return true;
			var isJsScript = assetPath.EndsWithJS();
			if (isJsScript)
				return options.jsScripts;
			var isBooScript = assetPath.EndsWithBoo();
			if (isBooScript)
				return options.booScripts;
			if (FindReplaceWindow.shaderFileTypes.Contains(Path.GetExtension(assetPath).ToLowerInvariant()))
				return options.shaders;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
		
		return options.textFiles;
	}
	
	static FindResultsWindow.ResultType ValidateResultAsReference(string assetGuid, TextPosition location, int length, ref SymbolDefinition referencedSymbol)
	{
		try
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
			if (string.IsNullOrEmpty(assetPath))
				return FindResultsWindow.ResultType.RemoveResult;
		
			var isCsScript = assetPath.EndsWithCS();
		
			var buffer = FGTextBufferManager.GetBuffer(assetGuid);
			if (buffer == null)
				return FindResultsWindow.ResultType.RemoveResult;
		
			if (buffer.Parser == null)
			{
				buffer.LoadImmediately();
				referencedSymbol = referencedSymbol.Rebind();
			}
		
			var formatedLine = buffer.formatedLines[location.line];
		
			var textLine = buffer.lines[location.line];
			var isVarResult =
				length == 3 &&
				referencedSymbol is TypeDefinitionBase &&
				location.index + 3 < textLine.Length &&
				textLine[location.index] == 'v' &&
				textLine[location.index + 1] == 'a' &&
				textLine[location.index + 2] == 'r';
		
			if (isCsScript)
			{
				if (formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive)
				{
					if (isVarResult)
						return FindResultsWindow.ResultType.RemoveResult;
					return FindResultsWindow.ResultType.InactiveCode;
				}
			}
			else if (isVarResult)
			{
				return FindResultsWindow.ResultType.RemoveResult;
			}
		
			int tokenIndex;
			bool atTokenEnd;
			var token = buffer.GetTokenAt(new TextPosition(location.line, location.index + 1), out location.line, out tokenIndex, out atTokenEnd);
			switch (token.tokenKind)
			{
			case SyntaxToken.Kind.Preprocessor:
				return FindResultsWindow.ResultType.RemoveResult;
				
			case SyntaxToken.Kind.Comment:
			case SyntaxToken.Kind.PreprocessorArguments:
			case SyntaxToken.Kind.PreprocessorSymbol:
				if (isVarResult)
					return FindResultsWindow.ResultType.RemoveResult;
				return FindResultsWindow.ResultType.Comment;
			
			case SyntaxToken.Kind.StringLiteral:
			case SyntaxToken.Kind.VerbatimStringLiteral:
			case SyntaxToken.Kind.VerbatimStringBegin:
			case SyntaxToken.Kind.InterpolatedStringWholeLiteral:
			case SyntaxToken.Kind.InterpolatedStringStartLiteral:
			case SyntaxToken.Kind.InterpolatedStringMidLiteral:
			case SyntaxToken.Kind.InterpolatedStringEndLiteral:
			case SyntaxToken.Kind.InterpolatedStringFormatLiteral:
				if (isVarResult)
					return FindResultsWindow.ResultType.RemoveResult;
				return FindResultsWindow.ResultType.String;
			}
		
			if (!isCsScript || token.parent == null)
				return isVarResult ? FindResultsWindow.ResultType.UnresolvedVarSymbol : FindResultsWindow.ResultType.UnresolvedSymbol;
		
			var resolvedSymbol = token.parent.resolvedSymbol;
			if (resolvedSymbol == null || resolvedSymbol.kind == SymbolKind.Error)
				FGResolver.ResolveNode(token.parent.parent);
		
			if (resolvedSymbol != null && resolvedSymbol.kind == SymbolKind.MethodGroup && token.parent.parent != null)
			{
				var nextLeaf = token.parent.parent.FindNextLeaf();
				if (nextLeaf != null && nextLeaf.IsLit("("))
				{
					var nextNode = nextLeaf.parent;
					if (nextNode.RuleName == "arguments")
					{
						FGResolver.ResolveNode(nextNode);
						if (token.parent != null)
							if (token.parent.resolvedSymbol == null || token.parent.resolvedSymbol.kind == SymbolKind.Error)
								token.parent.resolvedSymbol = resolvedSymbol;
					}
				}
			}
		
			resolvedSymbol = token.parent != null ? token.parent.resolvedSymbol : null;
			if (resolvedSymbol == null || resolvedSymbol.kind == SymbolKind.Error)
				return isVarResult ? FindResultsWindow.ResultType.UnresolvedVarSymbol : FindResultsWindow.ResultType.UnresolvedSymbol;
		
			if (resolvedSymbol.kind == SymbolKind.Constructor || resolvedSymbol.kind == SymbolKind.Destructor)
			{
				resolvedSymbol = resolvedSymbol.parentSymbol;
				if (resolvedSymbol != null && resolvedSymbol.kind == SymbolKind.MethodGroup)
					resolvedSymbol = resolvedSymbol.parentSymbol;
			}
			if (resolvedSymbol == null || resolvedSymbol.kind == SymbolKind.Error)
				return isVarResult ? FindResultsWindow.ResultType.UnresolvedVarSymbol : FindResultsWindow.ResultType.UnresolvedSymbol;
		
			var constructedSymbol = resolvedSymbol;
			resolvedSymbol = resolvedSymbol.GetGenericSymbol();
		
			if (referencedSymbol.kind == SymbolKind.MethodGroup && resolvedSymbol.kind == SymbolKind.Method)
				resolvedSymbol = resolvedSymbol.parentSymbol;
		
			if (resolvedSymbol != referencedSymbol)
			{
				var typeArgument = referencedSymbol as TypeDefinitionBase;
				var constructedType = constructedSymbol as ConstructedTypeDefinition;
				if (isVarResult && typeArgument != null && constructedType != null)
					if (IsUsedAsTypeArgument(typeArgument.GetGenericSymbol() as TypeDefinitionBase, constructedType))
						return FindResultsWindow.ResultType.VarTemplateReference;
			
				if (resolvedSymbol.kind == SymbolKind.Property && referencedSymbol.kind == SymbolKind.Property ||
					resolvedSymbol.kind == SymbolKind.Event && referencedSymbol.kind == SymbolKind.Event ||
					resolvedSymbol.kind == SymbolKind.Indexer && referencedSymbol.kind == SymbolKind.Indexer)
				{
					var resolvedProperty = resolvedSymbol as InstanceDefinition;
					var referencedProperty = referencedSymbol as InstanceDefinition;
					if (resolvedProperty != null && referencedProperty != null)
					{
						var resolvedType = resolvedProperty.parentSymbol as TypeDefinitionBase
							?? resolvedProperty.parentSymbol.parentSymbol as TypeDefinitionBase;
						var referencedType = referencedProperty.parentSymbol as TypeDefinitionBase
							?? referencedProperty.parentSymbol.parentSymbol as TypeDefinitionBase;
					
						var isInterface = resolvedType != null && resolvedType.kind == SymbolKind.Interface ||
							referencedType != null && referencedType.kind == SymbolKind.Interface;
					
						var resolvedIsVirtual = isInterface || resolvedProperty.IsOverride || resolvedProperty.IsVirtual || resolvedProperty.IsAbstract;
						var referencedIsVirtual = isInterface || referencedProperty.IsOverride || referencedProperty.IsVirtual || referencedProperty.IsAbstract;
						if (resolvedIsVirtual && referencedIsVirtual)
						{
							if (resolvedSymbol.kind != SymbolKind.Indexer ||
								System.Linq.Enumerable.SequenceEqual(
								System.Linq.Enumerable.Select(resolvedProperty.GetParameters(), x => x.TypeOf()),
								System.Linq.Enumerable.Select(referencedProperty.GetParameters(), x => x.TypeOf()) ))
							{
								if (resolvedType != null && resolvedType.DerivesFrom(referencedType))
									return FindResultsWindow.ResultType.OverridingMethod;
								if (referencedType != null && referencedType.DerivesFrom(resolvedType))
									return FindResultsWindow.ResultType.OverriddenMethod;
							}
						}
					}
				}
			
				if (resolvedSymbol.kind == SymbolKind.Method && referencedSymbol.kind == SymbolKind.Method)
				{
					if (resolvedSymbol.parentSymbol == referencedSymbol.parentSymbol)
						return FindResultsWindow.ResultType.MethodOverload;
				
					var resolvedMethod = resolvedSymbol as MethodDefinition;
					var referencedMethod = referencedSymbol as MethodDefinition;
					if (resolvedMethod != null && referencedMethod != null)
					{
						var resolvedType = resolvedMethod.parentSymbol as TypeDefinitionBase
							?? resolvedMethod.parentSymbol.parentSymbol as TypeDefinitionBase;
						var referencedType = referencedMethod.parentSymbol as TypeDefinitionBase
							?? referencedMethod.parentSymbol.parentSymbol as TypeDefinitionBase;
					
						var isInterface = resolvedType != null && resolvedType.kind == SymbolKind.Interface || referencedType != null && referencedType.kind == SymbolKind.Interface;
					
						var resolvedIsVirtual = isInterface || resolvedMethod.IsOverride || resolvedMethod.IsVirtual || resolvedMethod.IsAbstract;
						var referencedIsVirtual = isInterface || referencedMethod.IsOverride || referencedMethod.IsVirtual || referencedMethod.IsAbstract;
						if (resolvedIsVirtual && referencedIsVirtual)
						{
							if (System.Linq.Enumerable.SequenceEqual(
								System.Linq.Enumerable.Select(resolvedMethod.GetParameters(), x => x.TypeOf()),
								System.Linq.Enumerable.Select(referencedMethod.GetParameters(), x => x.TypeOf()) ))
							{
								if (resolvedType != null && resolvedType.DerivesFrom(referencedType))
									return FindResultsWindow.ResultType.OverridingMethod;
								if (referencedType != null && referencedType.DerivesFrom(resolvedType))
									return FindResultsWindow.ResultType.OverriddenMethod;
							}
						}
					}
				}

				if (resolvedSymbol.kind != SymbolKind.MethodGroup || referencedSymbol.parentSymbol != resolvedSymbol)
					return FindResultsWindow.ResultType.RemoveResult;
			}
		
			if (isVarResult)
				return FindResultsWindow.ResultType.VarReference;
		
			if (FGResolver.IsWriteReference(token))
				return FindResultsWindow.ResultType.WriteReference;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
		
		return FindResultsWindow.ResultType.ReadReference;
	}
	
	static bool IsUsedAsTypeArgument(TypeDefinitionBase typeArgument, ConstructedTypeDefinition constructedType)
	{
		var arguments = constructedType.typeArguments;
		if (arguments == null)
			return false;
		
		for (int i = arguments.Length; i --> 0; )
		{
			var argRef = arguments[i];
			if (argRef == null)
				continue;
			var arg = argRef.definition;
			if (arg == null)
				continue;
			
			if (arg.GetGenericSymbol() == typeArgument)
				return true;
			
			var constructedArg = arg as ConstructedTypeDefinition;
			if (constructedArg != null && IsUsedAsTypeArgument(typeArgument, constructedArg))
				return true;
		}
		return false;
	}
	
	static List<string> FindReferenceCandidates(SymbolDefinition symbol, string localAssetPath)
	{
		var result = new List<string> { localAssetPath };
		if (assets != null)
			assets.Clear();
		else
			assets = new List<string>();
		
		if (symbol.kind == SymbolKind.CatchParameter ||
			symbol.kind == SymbolKind.Destructor ||
			symbol.kind == SymbolKind.CaseVariable ||
			symbol.kind == SymbolKind.ForEachVariable ||
			symbol.kind == SymbolKind.FromClauseVariable ||
			symbol.kind == SymbolKind.Label ||
			symbol.kind == SymbolKind.LambdaExpression ||
			symbol.kind == SymbolKind.LocalConstant ||
			symbol.kind == SymbolKind.Parameter ||
			symbol.kind == SymbolKind.TupleDeconstructVariable ||
			symbol.kind == SymbolKind.OutVariable ||
			symbol.kind == SymbolKind.IsVariable ||
			symbol.kind == SymbolKind.Variable)
		{
			// Local symbols cannot appear in any other file
			return result;
		}
		
		var localExtension = Path.GetExtension(localAssetPath.ToLowerInvariant());
		
		var allTextAssetGuids = FindAllTextAssets();
		for (int i = allTextAssetGuids.Count; i --> 0; )
		{
			var path = AssetDatabase.GUIDToAssetPath(allTextAssetGuids[i]);
			if (path != localAssetPath)
				//if (!ignoreFileTypes.Contains(Path.GetExtension(path.ToLowerInvariant())))
				if (localExtension == Path.GetExtension(path.ToLowerInvariant()))
					assets.Add(AssetDatabase.GUIDToAssetPath(allTextAssetGuids[i]));
		}
			
		for (int i = assets.Count; i --> 0; )
			result.Add(assets[i]);
		
		result.Sort((a, b) => {
			// Search .cs files first
			var aIsCs = a.EndsWithCS() ? 1 : 0;
			var bIsCs = b.EndsWithCS() ? 1 : 0;
			if (aIsCs != 0 && bIsCs != 0)
				return bIsCs - aIsCs;
			
			var aIsJsOrBoo = a.EndsWithJS() || a.EndsWithBoo();
			var bIsJsOrBoo = b.EndsWithJS() || b.EndsWithBoo();
			if (aIsJsOrBoo || bIsJsOrBoo)
				return (bIsJsOrBoo ? 1 : 0) - (aIsJsOrBoo ? 1 : 0);

			// And everything else at the end
			return 0;
		});
		
		return result;
	}
		
	public static void FindAllInSingleFile(
		System.Action<string, string, TextPosition, int> addResultAction,
		string assetGuid,
		FindResultsWindow.SearchOptions search)
	{
		var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
		var isCsFile = assetPath.EndsWithCS();
		
		var allLines = GetOrReadAllLines(assetGuid);
		if (!isCsFile || search.altText1 == null)
		{
			foreach (var textPosition in FindAll(allLines, search))
			{
				var line = allLines[textPosition.line];
				addResultAction(line, assetGuid, textPosition, search.text.Length);
			}
		}
		else
		{
			var results = FindAll(allLines, search).GetEnumerator();
			
			var altSearch = new FindResultsWindow.SearchOptions {
				text = search.altText1,
				matchCase = search.matchCase,
				matchWord = search.matchWord,
			};
			var altResults = FindAll(allLines, altSearch).GetEnumerator();
			
			IEnumerator<TextPosition> altResults2 = null;
			if (search.altText2 != null)
			{
				var altSearch2 = new FindResultsWindow.SearchOptions {
					text = search.altText2,
					matchCase = search.matchCase,
					matchWord = search.matchWord,
				};
				altResults2 = FindAll(allLines, altSearch2).GetEnumerator();
			}
			
			bool more = results.MoveNext();
			bool altMore = altResults.MoveNext();
			bool altMore2 = altResults2 != null && altResults2.MoveNext();
			while (more || altMore || altMore2)
			{
				if (more && (!altMore || results.Current <= altResults.Current)
					&& (!altMore2 || results.Current <= altResults2.Current))
				{
					var line = allLines[results.Current.line];
					addResultAction(line, assetGuid, results.Current, search.text.Length);
					more = results.MoveNext();
				}
				else if (altMore && (!more || altResults.Current <= results.Current)
					&& (!altMore2 || altResults.Current <= altResults2.Current))
				{
					var line = allLines[altResults.Current.line];
					addResultAction(line, assetGuid, altResults.Current, search.altText1.Length);
					altMore = altResults.MoveNext();
				}
				else
				{
					var line = allLines[altResults2.Current.line];
					addResultAction(line, assetGuid, altResults2.Current, search.altText2.Length);
					altMore2 = altResults2.MoveNext();
				}
			}
		}
	}
	
	public static IList<string> GetOrReadAllLines(string assetGuid)
	{
		var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
		return GetOrReadAllLinesForPath(assetPath);
	}
	
	public static IList<string> GetOrReadAllLinesForPath(string assetPath)
	{
		if (string.IsNullOrEmpty(assetPath))
			return null;
		
		string[] lines = null;
		try
		{
			var textBuffer = FGTextBufferManager.TryGetBuffer(assetPath);
			if (textBuffer != null)
				return textBuffer.lines;
			
			lines = File.ReadAllLines(assetPath);
		}
		catch (IOException e)
		{
			Debug.LogError(e);
			return null;
		}
		return lines;
	}
	
	internal static IEnumerable<TextPosition> FindAll(IList<string> lines, FindResultsWindow.SearchOptions search)
	{
		var length = search.text.Length;
		if (length == 0)
			yield break;
		
		var comparison = search.matchCase ? System.StringComparison.Ordinal : System.StringComparison.OrdinalIgnoreCase;
		
		char firstChar = search.text[0];
		bool startsAsWord = firstChar == '_' || char.IsLetterOrDigit(firstChar);
		char lastChar = search.text[search.text.Length - 1];
		bool endsAsWord = lastChar == '_' || char.IsLetterOrDigit(lastChar);
		
		int skipThisWord = search.text.IndexOf(firstChar.ToString(), 1, comparison);
		if (skipThisWord < 0)
			skipThisWord = search.text.Length;
		
		var searchInSelection = !search.selection.IsEmpty;
		var l = searchInSelection ? search.selection.line : 0;
		var c = searchInSelection ? search.selection.index : 0;
		var toLine = searchInSelection ? search.selection.line + search.selection.lineOffset : lines.Count - 1;
		var toChar = search.selection.EndPosition.index;
		while (l <= toLine)
		{
			var line = lines[l];
			
			if (c > line.Length - length)
			{
				c = 0;
				++l;
				continue;
			}
			
			c = line.IndexOf(search.text, c, comparison);
			if (c < 0)
			{
				c = 0;
				++l;
				continue;
			}
			
			if (searchInSelection && l == toLine && c + length > toChar)
			{
				break;
			}
			
			if (search.matchWord)
			{
				if (startsAsWord && c > 0)
				{
					char prevChar = line[c - 1];
					if (prevChar == '_' || char.IsLetterOrDigit(prevChar))
					{
						c += skipThisWord;
						continue;
					}
				}
				if (endsAsWord && c + length < line.Length)
				{
					char nextChar = line[c + length];
					if (nextChar == '_' || char.IsLetterOrDigit(nextChar))
					{
						c += skipThisWord;
						continue;
					}
				}
			}
			
			yield return new TextPosition(l, c);
			c += length;
		}
	}
	
	public static bool ContainsWordsSequence(string assetPath, params string[] words)
	{
		if (string.IsNullOrEmpty(assetPath))
			return false;
		
		try
		{
			var lines = File.ReadAllLines(assetPath);
			var l = 0;
			var w = 0;
			var s = 0;
			while (l < lines.Length)
			{
				if (s > lines[l].Length - words[0].Length)
				{
					s = 0;
					++l;
					continue;
				}
				
				s = lines[l].IndexOf(words[0], s, System.StringComparison.Ordinal);
				if (s < 0)
				{
					s = 0;
					++l;
					continue;
				}
				
				if (s > 0)
				{
					var c = lines[l][s - 1];
					if (c == '_' || char.IsLetterOrDigit(c))
					{
						s += words[0].Length;
						continue;
					}
				}
				
				s += words[0].Length;
				if (s < lines[l].Length)
				{
					if (words[1] != "(")
					{
						var c = lines[l][s];
						s++;
						if (c != ' ' && c != '\t')
							continue;
					}
				}
				else
				{
					s = 0;
					++l;
					if (l == lines.Length)
						break;
				}
				
				w = 1;
				while (w < words.Length)
				{
					// Skip additional whitespaces
					while (s < lines[l].Length)
					{
						var c = lines[l][s];
						if (c == ' ' || c == '\t')
							++s;
						else
							break;
					}
					
					if (s == lines[l].Length)
					{
						s = 0;
						++l;
						if (l == lines.Length)
							break;
						continue;
					}
					
					if (!lines[l].Substring(s).FastStartsWith(words[w]))
					{
						w = 0;
						break;
					}
					
					s += words[w].Length;
					if (s < lines[l].Length && words[w] != "(")
					{
						var c = lines[l][s];
						if (c == '_' || char.IsLetterOrDigit(c))
						{
							w = 0;
							break;
						}
					}
					
					++w;
				}
				
				if (w == words.Length)
				{
					return true;
				}
			}
		}
		catch (IOException e)
		{
			Debug.LogError(e);
		}
		return false;
	}
	
	public static void Reset()
	{
		if (assets != null)
			assets.Clear();
	}
	
	public static List<string> FindAllTextAssets()
	{
		var hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
		hierarchyProperty.SetSearchFilter("t:TextAsset", 0);
		hierarchyProperty.Reset();
		List<string> list = new List<string>();
		while (hierarchyProperty.Next(null))
			list.Add(hierarchyProperty.guid);
		return list;
	}
	
	public static void FindAllAssemblyScripts(AssemblyDefinition.UnityAssembly assemblyId)
	{
		var editor = false;
		var firstPass = false;
		var pattern = "";
		
		switch (assemblyId)
		{
		case AssemblyDefinition.UnityAssembly.CSharpFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptFirstPass:
		case AssemblyDefinition.UnityAssembly.BooFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.BooEditorFirstPass:
			firstPass = true;
			break;
		}
		
		switch (assemblyId)
		{
		case AssemblyDefinition.UnityAssembly.CSharpFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharp:
		case AssemblyDefinition.UnityAssembly.CSharpEditor:
			pattern = ".cs";
			break;
		case AssemblyDefinition.UnityAssembly.UnityScriptFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScript:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditor:
			pattern = ".js";
			break;
		case AssemblyDefinition.UnityAssembly.BooFirstPass:
		case AssemblyDefinition.UnityAssembly.BooEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.Boo:
		case AssemblyDefinition.UnityAssembly.BooEditor:
			pattern = ".boo";
			break;
		}
		
		switch (assemblyId)
		{
		case AssemblyDefinition.UnityAssembly.CSharpEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.BooEditorFirstPass:
		case AssemblyDefinition.UnityAssembly.CSharpEditor:
		case AssemblyDefinition.UnityAssembly.UnityScriptEditor:
		case AssemblyDefinition.UnityAssembly.BooEditor:
			editor = true;
			break;
		}
		
		//var scripts = FindAssets("t:MonoScript");
		var scripts = Directory.GetFiles("Assets", "*" + pattern, SearchOption.AllDirectories);
		var count = scripts.Length;
		
		if (assets == null)
			assets = new List<string>(count);
		
		bool isUnity_5_2_1p4_orNewer = true;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		isUnity_5_2_1p4_orNewer =
		Application.unityVersion.FastStartsWith("5.2.1p") &&
			int.Parse(Application.unityVersion.Substring("5.2.1p".Length)) >= 4;
#endif
		
		for (var i = count; i --> 0; )
		{
			var path = scripts[i];
			scripts[i] = path = path.Replace('\\', '/');
			string lowerPath = path.ToLowerInvariant();
			
			if (path.Contains("/.") || AssemblyDefinition.IsIgnoredScript(lowerPath))
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			scripts[i] = AssetDatabase.AssetPathToGUID(scripts[i]);
			
			var extension = Path.GetExtension(lowerPath);
			if (extension != pattern)
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			var isFirstPass = lowerPath.StartsWithIgnoreCase("assets/standard assets/") ||
				lowerPath.StartsWithIgnoreCase("assets/pro standard assets/") ||
				lowerPath.StartsWithIgnoreCase("assets/plugins/");
			if (firstPass != isFirstPass)
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			var isEditor = false;
			if (isFirstPass && !isUnity_5_2_1p4_orNewer)
				isEditor = lowerPath.StartsWithIgnoreCase("assets/plugins/editor/") ||
				lowerPath.StartsWithIgnoreCase("assets/standard assets/editor/") ||
				lowerPath.StartsWithIgnoreCase("assets/pro standard assets/editor/");
			else
				isEditor = lowerPath.Contains("/editor/");
			if (editor != isEditor)
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			assets.Add(scripts[i]);
		}
		//var joined = string.Join(", ", scripts, 0, count);
		//Debug.Log(joined);
	}
	
	public static void FindAllAssemblyScripts(AssemblyDefinition assembly)
	{
		var scripts = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);
		var count = scripts.Length;
		
		if (assets == null)
			assets = new List<string>(count);
		
		for (var i = count; i --> 0; )
		{
			var path = scripts[i];
			scripts[i] = path = path.Replace('\\', '/');
			string lowerPath = path.ToLowerInvariant();
			
			if (path.Contains("/.") || AssemblyDefinition.IsIgnoredScript(lowerPath))
			{
				scripts[i] = scripts[--count];
				continue;
			}
			
			var assemblyFromPath = AssemblyDefinition.FromAssetPath(path);
			if (assemblyFromPath != assembly)
			{
				scripts[i] = scripts[--count];
				continue;
			}			
			
			var guid = AssetDatabase.AssetPathToGUID(scripts[i]);
			assets.Add(guid);
		}
		//var joined = string.Join("\n", scripts, 0, count);
		//Debug.Log(joined);
	}
}

}
