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
	
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class CsParser : FGParser
{
	public override HashSet<string> Keywords { get { return keywords; } }
	public override HashSet<string> BuiltInLiterals { get { return scriptLiterals; } }

	public override bool IsBuiltInType(string word)
	{
		return builtInTypes.Contains(word);
	}

	public override bool IsBuiltInLiteral(string word)
	{
		return word == "true" || word == "false" || word == "null";
	}

	private static readonly HashSet<string> keywords = new HashSet<string> {
		"abstract", "as", "base", "break", "case", "catch", "checked", "class", "const", "continue",
		"default", "delegate", "do", "else", "enum", "event", "explicit", "extern", "finally",
		"fixed", "for", "foreach", "goto", "if", "implicit", "in", "interface", "internal", "is",
		"lock", "namespace", "new", "operator", "out", "override", "params", "private",
		"protected", "public", "readonly", "ref", "return", "sealed", "sizeof", "stackalloc", "static",
		"struct", "switch", "this", "throw", "try", "typeof", "unchecked", "unsafe", "using", "virtual",
		"volatile", "while"
	};

	//private static readonly string[] csPunctsAndOps = {
	//	"{", "}", ";", "#", ".", "(", ")", "[", "]", "++", "--", "->", "+", "-",
	//	"!", "~", "++", "--", "&", "*", "/", "%", "+", "-", "<<", ">>", "<", ">",
	//	"<=", ">=", "==", "!=", "&", "^", "|", "&&", "||", "??", "?", "::", ":",
	//	"=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=", "=>"
	//};

	private static readonly HashSet<string> csOperators = new HashSet<string>{
		"++", "--", "->", "+", "-", "!", "~", "++", "--", "&", "*", "/", "%", "+", "-", "<<", ">>", "<", ">",
		"<=", ">=", "==", "!=", "&", "^", "|", "&&", "||", "??", "?", "::", ":",
		"=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=", "=>",
	};

	private static readonly HashSet<string> preprocessorKeywords = new HashSet<string>{
		"define", "elif", "else", "endif", "endregion", "error", "if", "line", "pragma", "region", "undef", "warning"
	};

	private static readonly HashSet<string> builtInTypes = new HashSet<string> {
		"bool", "byte", "char", "decimal", "double", "float", "int", "long", "object", "sbyte", "short",
		"string", "uint", "ulong", "ushort", "void"
	};
	
	private static readonly HashSet<string> keywordsAndBuiltInTypes;

	static CsParser()
	{
		keywordsAndBuiltInTypes = new HashSet<string>();
		keywordsAndBuiltInTypes.UnionWith(keywords);
		keywordsAndBuiltInTypes.UnionWith(builtInTypes);
		
		//var all = new HashSet<string>(csKeywords);
		//all.UnionWith(csTypes);
		//all.UnionWith(csPunctsAndOps);
		//all.UnionWith(scriptLiterals);
		//tokenLiterals = new string[all.Count];
		//all.CopyTo(tokenLiterals);
		//Array.Sort<string>(tokenLiterals);
	}

	protected override void ParseAll(string bufferName)
	{
		var scanner = CsGrammar.Scanner.New(CsGrammar.Instance, textBuffer.formatedLines, bufferName);
		parseTree = CsGrammar.Instance.ParseAll(scanner);
		scanner.Delete();
	}

	public override FGGrammar.IScanner MoveAfterLeaf(ParseTree.Leaf leaf)
	{
		var scanner = CsGrammar.Scanner.New(CsGrammar.Instance, textBuffer.formatedLines, assetPath);
		var result = leaf == null ? scanner : scanner.MoveAfterLeaf(leaf) ? scanner : null;
		if (result == null)
			scanner.Delete();
		return result;
	}

	public override bool ParseLines(int fromLine, int toLineInclusive)
	{
		var formatedLines = textBuffer.formatedLines;

		for (var line = Math.Max(0, fromLine); line <= toLineInclusive; ++line)
		{
			var tokens = formatedLines[line].tokens;
			for (var i = tokens.Count; i --> 0; )
			{
				var t = tokens[i];
				//if (t.parent != null)
				//{
				//	//t.parent.line = line;
				//	t.parent.tokenIndex = i;
				//}
				/*if (t.tokenKind == SyntaxToken.Kind.ContextualKeyword)
				{
					t.tokenKind = SyntaxToken.Kind.Identifier;
					t.style = textBuffer.styles.normalStyle;
				}
				else*/ if (t.tokenKind == SyntaxToken.Kind.Missing)
				{
					if (t.parent != null && t.parent.parent != null)
						t.parent.parent.syntaxError = null;
					tokens.RemoveAt(i);
				}
			}
		}

		var scanner = CsGrammar.Scanner.New(CsGrammar.Instance, formatedLines, assetPath);
		//CsGrammar.Instance.ParseAll(scanner);
		scanner.MoveToLine(fromLine, parseTree);
//        if (scanner.CurrentGrammarNode == null)
//        {
//            if (!scanner.MoveNext())
//                return false;
			
//            FGGrammar.Rule startRule = CsGrammar.Instance.r_compilationUnit;

////			if (parseTree == null)
////			{
////				parseTree = new ParseTree();
////				var rootId = new Id(startRule.GetNt());
////				ids[Start.GetNt()] = startRule;
////			rootId.SetLookahead(this);
////			Start.parent = rootId;
//                scanner.CurrentParseTreeNode = parseTree.root;// = new ParseTree.Node(rootId);
//                scanner.CurrentGrammarNode = startRule;//.Parse(scanner);
			
//                scanner.ErrorParseTreeNode = scanner.CurrentParseTreeNode;
//                scanner.ErrorGrammarNode = scanner.CurrentGrammarNode;
//            //}
//        }

		//Debug.Log("Parsing line " + (fromLine + 1) + " starting from " + scanner.CurrentLine() + ", token " + scanner.CurrentTokenIndex() + " currentToken " + scanner.Current);

		var grammar = CsGrammar.Instance;
		var canContinue = true;
		for (var line = Math.Max(0, scanner.CurrentLine() - 1); canContinue && line <= toLineInclusive; line = scanner.CurrentLine() - 1)
			canContinue = grammar.ParseLine(scanner, line);
			//if (!(canContinue = grammar.ParseLine(scanner, line)))
			//	if (scanner.Current.tokenKind != SyntaxToken.Kind.EOF)
			//		Debug.Log("can't continue at line " + (line + 1) + " token " + scanner.Current);

		if (canContinue && toLineInclusive == formatedLines.Length - 1)
			canContinue = grammar.GetParser.ParseStep(scanner);
		
		scanner.Delete();

		//Debug.Log("canContinue == " + canContinue);

		for (var line = fromLine; line <= toLineInclusive; ++line)
			foreach (var t in formatedLines[line].tokens)
				if (t.tokenKind == SyntaxToken.Kind.ContextualKeyword)
					t.style = t.text == "value" ? textBuffer.styles.parameterStyle : textBuffer.styles.keywordStyle;

		return canContinue;
		//return true;
	}
	
	public override void FullRefresh()
	{
		base.FullRefresh();
		
		parserThread = new System.Threading.Thread(() =>
		{
			this.OnLoaded();
			this.parserThread = null;
		});
		parserThread.Start();
	}
	
	private static string[] rspFileNames = { "Assets/mcs.rsp", "Assets/smcs.rsp", "Assets/gmcs.rsp", "Assets/csc.rsp" };
	private static char[] optionsSeparators = { ' ', '\n', '\r' };
	private static char[] definesSeparators = { ';', ',' };
	private static HashSet<string> activeScriptCompilationDefines;
	protected static HashSet<string> GetActiveScriptCompilationDefines()
	{
		if (activeScriptCompilationDefines != null)
			return activeScriptCompilationDefines;
		
		activeScriptCompilationDefines = new HashSet<string>(UnityEditor.EditorUserBuildSettings.activeScriptCompilationDefines);
		string rspText = null;
		try
		{
			string rspFileName = null;
			for (var i = 0; i < rspFileNames.Length; ++i )
			{
				if (System.IO.File.Exists(rspFileNames[i]))
				{
					rspFileName = rspFileNames[i];
					break;
				}
			}
		
			if (rspFileName != null)
			{
				rspText = System.IO.File.ReadAllText(rspFileName);
			}
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
		
		if (rspText != null)
		{
			var options = rspText.Split(optionsSeparators, StringSplitOptions.RemoveEmptyEntries);
			for (var i = options.Length; i --> 0; )
			{
				var option = options[i];
				if (option.StartsWithIgnoreCase("-define:") || option.StartsWithIgnoreCase("/define:"))
				{
					option = option.Substring("-define:".Length);
				}
				else if (option.StartsWithIgnoreCase("-d:") || option.StartsWithIgnoreCase("/d:"))
				{
					option = option.Substring("-d:".Length);
				}
				else
				{
					continue;
				}
					
				var defineOptions = option.Split(definesSeparators, StringSplitOptions.RemoveEmptyEntries);
				for (var j = defineOptions.Length; j --> 0; )
					activeScriptCompilationDefines.Add(defineOptions[j]);
			}
		}
		
		return activeScriptCompilationDefines;
	}

	public override void LexLine(int currentLine, FGTextBuffer.FormatedLine formatedLine)
	{
		formatedLine.index = currentLine;

		if (parserThread != null)
			parserThread.Join();
		parserThread = null;

		string textLine = textBuffer.lines[currentLine];

		//Stopwatch sw1 = new Stopwatch();
		//Stopwatch sw2 = new Stopwatch();
		
		if (currentLine == 0)
		{
			var defaultScriptDefines = GetActiveScriptCompilationDefines();
			if (scriptDefines == null || !scriptDefines.SetEquals(defaultScriptDefines))
			{
				if (scriptDefines == null)
				{
					scriptDefines = new HashSet<string>(defaultScriptDefines);
				}
				else
				{
					scriptDefines.Clear();
					scriptDefines.UnionWith(defaultScriptDefines);
				}
			}
		}
		
		//sw2.Start();
		if (formatedLine.tokens != null)
			formatedLine.tokens.Clear();
		Tokenize(textLine, formatedLine);

//		syntaxTree.SetLineTokens(currentLine, lineTokens);
		var lineTokens = formatedLine.tokens;

		if (textLine.Length == 0)
		{
			formatedLine.tokens.Clear();
		}
		else if (textBuffer.styles != null)
		{
			var lineWidth = textBuffer.CharIndexToColumn(textLine.Length, currentLine);
			if (lineWidth > textBuffer.longestLine)
				textBuffer.longestLine = lineWidth;

			for (var i = 0; i < lineTokens.Count; ++i)
			{
				var token = lineTokens[i];
				switch (token.tokenKind)
				{
					case SyntaxToken.Kind.Whitespace:
					case SyntaxToken.Kind.Missing:
						token.style = textBuffer.styles.normalStyle;
						break;

					case SyntaxToken.Kind.Punctuator:
						token.style = IsOperator(token.text) ? textBuffer.styles.operatorStyle : textBuffer.styles.punctuatorStyle;
						break;

					case SyntaxToken.Kind.Keyword:
						if (IsBuiltInType(token.text))
						{
							if (token.text == "string" || token.text == "object")
								token.style = textBuffer.styles.builtInRefTypeStyle;
							else
								token.style = textBuffer.styles.builtInValueTypeStyle;
						}
						else if (IsControlKeyword(token.text))
						{
							token.style = textBuffer.styles.controlKeywordStyle;
						}
						else
						{
							token.style = textBuffer.styles.keywordStyle;
						}
						break;

					case SyntaxToken.Kind.Identifier:
						if (token.text == "true" || token.text == "false" || token.text == "null")
						{
							token.style = textBuffer.styles.builtInLiteralsStyle;
							token.tokenKind = SyntaxToken.Kind.BuiltInLiteral;
						}
						else
						{
							token.style = textBuffer.styles.normalStyle;
						}
						break;

					case SyntaxToken.Kind.IntegerLiteral:
					case SyntaxToken.Kind.RealLiteral:
						token.style = textBuffer.styles.constantStyle;
						break;

					case SyntaxToken.Kind.Comment:
						var regionKind = formatedLine.regionTree.kind;
						var inactiveLine = regionKind > FGTextBuffer.RegionTree.Kind.LastActive;
						token.style = inactiveLine ? textBuffer.styles.inactiveCodeStyle : textBuffer.styles.commentStyle;
						break;

					case SyntaxToken.Kind.Preprocessor:
						token.style = textBuffer.styles.preprocessorStyle;
						break;

					case SyntaxToken.Kind.PreprocessorSymbol:
						token.style = textBuffer.styles.defineSymbols;
						break;

					case SyntaxToken.Kind.PreprocessorArguments:
					case SyntaxToken.Kind.PreprocessorCommentExpected:
					case SyntaxToken.Kind.PreprocessorDirectiveExpected:
					case SyntaxToken.Kind.PreprocessorUnexpectedDirective:
						token.style = textBuffer.styles.normalStyle;
						break;

					case SyntaxToken.Kind.CharLiteral:
					case SyntaxToken.Kind.StringLiteral:
					case SyntaxToken.Kind.VerbatimStringBegin:
					case SyntaxToken.Kind.VerbatimStringLiteral:
					case SyntaxToken.Kind.InterpolatedStringWholeLiteral:
					case SyntaxToken.Kind.InterpolatedStringStartLiteral:
					case SyntaxToken.Kind.InterpolatedStringMidLiteral:
					case SyntaxToken.Kind.InterpolatedStringEndLiteral:
					case SyntaxToken.Kind.InterpolatedStringFormatLiteral:
						token.style = textBuffer.styles.stringStyle;
						break;
				}
				lineTokens[i] = token;
			}
		}
	}
	
	protected bool IsControlKeyword(string text)
	{
		return
			text == "if" ||
			text == "else" ||
			text == "for" ||
			text == "foreach" ||
			text == "while" ||
			text == "do" ||
			text == "break" ||
			text == "continue" ||
			text == "return" ||
			text == "yield" ||
			text == "when" ||
			text == "switch" ||
			text == "case" ||
			text == "try" ||
			text == "catch" ||
			text == "finally" ||
			text == "throw";
	}
	
	protected override void Tokenize(string line, FGTextBuffer.FormatedLine formatedLine)
	{
		var tokens = formatedLine.tokens;
		if (tokens == null)
		{
			tokens = new List<SyntaxToken>();
			formatedLine.tokens = tokens;
		}

		int startAt = 0;
		int length = line.Length;
		SyntaxToken token;

		SyntaxToken ws = ScanWhitespace(line, ref startAt);
		if (ws != null)
		{
			tokens.Add(ws);
			ws.formatedLine = formatedLine;
		}

		if (formatedLine.blockState == FGTextBuffer.BlockState.None && startAt < length && line[startAt] == '#')
		{
			tokens.Add(new SyntaxToken(SyntaxToken.Kind.Preprocessor, "#") { formatedLine = formatedLine });
			++startAt;

			ws = ScanWhitespace(line, ref startAt);
			if (ws != null)
			{
				tokens.Add(ws);
				ws.formatedLine = formatedLine;
			}

			var error = false;
			var commentsOnly = false;
			var preprocessorCommentsAllowed = true;
			
			token = ScanWord(line, ref startAt);
			if (!preprocessorKeywords.Contains(token.text))
			{
				token.tokenKind = SyntaxToken.Kind.PreprocessorDirectiveExpected;
				tokens.Add(token);
				token.formatedLine = formatedLine;
				
				error = true;
			}
			else
			{
				token.tokenKind = SyntaxToken.Kind.Preprocessor;
				tokens.Add(token);
				token.formatedLine = formatedLine;
	
				ws = ScanWhitespace(line, ref startAt);
				if (ws != null)
				{
					tokens.Add(ws);
					ws.formatedLine = formatedLine;
				}

				if (token.text == "if")
				{
					bool active = ParsePPOrExpression(line, formatedLine, ref startAt);
					bool inactiveParent = formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive;
					if (active && !inactiveParent)
					{
						OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.If);
						commentsOnly = true;
					}
					else
					{
						OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.InactiveIf);
						commentsOnly = true;
					}
				}
				else if (token.text == "elif")
				{
					if (formatedLine.regionTree.parent == null)
					{
						token.tokenKind = SyntaxToken.Kind.PreprocessorUnexpectedDirective;
					}
					else
					{
						bool active = ParsePPOrExpression(line, formatedLine, ref startAt);
						bool inactiveParent = formatedLine.regionTree.parent.kind > FGTextBuffer.RegionTree.Kind.LastActive;
						bool setActive = active && !inactiveParent;
						if (formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.If)
						{
							setActive = false;
						}
						else if (formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.Elif ||
							formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveElif)
						{
							if (setActive && formatedLine.regionTree.parent != null)
							{
								var lineIndex = formatedLine.index;
								var ifIndex = -1;
								var activeIndex = -1;
								var siblings = formatedLine.regionTree.parent.children;
								for (var i = siblings.Count; i --> 0; )
								{
									var siblingLine = siblings[i].line;
									var siblingIndex = siblingLine.index;
									if (siblingIndex < lineIndex)
									{
										if (siblingIndex > activeIndex &&
											siblingLine.regionTree.kind < FGTextBuffer.RegionTree.Kind.LastActive)
										{
											activeIndex = siblingIndex;
										}
										if (siblingIndex > ifIndex &&
											(siblingLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.If ||
											siblingLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveIf))
										{
											ifIndex = siblingIndex;
										}
									}
								}
								
								if (activeIndex >= ifIndex)
								{
									setActive = false;
								}
							}
						}
						else if (formatedLine.regionTree.kind != FGTextBuffer.RegionTree.Kind.InactiveIf)
						{
							token.tokenKind = SyntaxToken.Kind.PreprocessorUnexpectedDirective;
							setActive = !inactiveParent;
						}

						if (token.tokenKind != SyntaxToken.Kind.PreprocessorUnexpectedDirective)
						{
							if (setActive)
							{
								OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.Elif);
							}
							else
							{
								OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.InactiveElif);
							}
						}
					}
				}
				else if (token.text == "else")
				{
					if (formatedLine.regionTree.parent == null)
					{
						token.tokenKind = SyntaxToken.Kind.PreprocessorUnexpectedDirective;
					}
					else
					{
						bool inactiveParent = formatedLine.regionTree.parent.kind > FGTextBuffer.RegionTree.Kind.LastActive;
						bool setActive = !inactiveParent;
						if (formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.If ||
							formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.Elif)
						{
							setActive = false;
						}
						else if (formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveIf ||
							formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveElif)
						{
							if (setActive)
							{
								var lineIndex = formatedLine.index;
								var ifIndex = -1;
								var activeIndex = -1;
								var siblings = formatedLine.regionTree.parent.children;
								for (var i = siblings.Count; i --> 0; )
								{
									var siblingLine = siblings[i].line;
									var siblingIndex = siblingLine.index;
									if (siblingIndex < lineIndex)
									{
										if (siblingIndex > activeIndex &&
											siblingLine.regionTree.kind < FGTextBuffer.RegionTree.Kind.LastActive)
										{
											activeIndex = siblingIndex;
										}
										if (siblingIndex > ifIndex &&
										(siblingLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.If ||
											siblingLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveIf))
										{
											ifIndex = siblingIndex;
										}
									}
								}
								
								if (activeIndex >= ifIndex)
								{
									setActive = false;
								}
							}
						}
						else if (formatedLine.regionTree.kind != FGTextBuffer.RegionTree.Kind.InactiveIf)
						{
							token.tokenKind = SyntaxToken.Kind.PreprocessorUnexpectedDirective;
						}

						if (token.tokenKind != SyntaxToken.Kind.PreprocessorUnexpectedDirective)
						{
							if (setActive)
							{
								OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.Else);
							}
							else
							{
								OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.InactiveElse);
							}
						}
					}
				}
				else if (token.text == "endif")
				{
					if (formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.If ||
						formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.Elif ||
						formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.Else ||
						formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveIf ||
						formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveElif ||
						formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveElse)
					{
						CloseRegion(formatedLine);
					}
					else
					{
						token.tokenKind = SyntaxToken.Kind.PreprocessorUnexpectedDirective;
					}
				}
				else if (token.text == "region")
				{
					var inactive = formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive;
					if (inactive)
					{
						OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.InactiveRegion);
					}
					else
					{
						OpenRegion(formatedLine, FGTextBuffer.RegionTree.Kind.Region);
					}
					preprocessorCommentsAllowed = false;
				}
				else if (token.text == "endregion")
				{
					if (formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.Region ||
						formatedLine.regionTree.kind == FGTextBuffer.RegionTree.Kind.InactiveRegion)
					{
						CloseRegion(formatedLine);
					}
					else
					{
						token.tokenKind = SyntaxToken.Kind.PreprocessorUnexpectedDirective;
					}
					preprocessorCommentsAllowed = false;
				}
				else if (token.text == "define" || token.text == "undef")
				{
					var symbol = FGParser.ScanIdentifierOrKeyword(line, ref startAt);
					if (symbol != null && symbol.text != "true" && symbol.text != "false")
					{
						symbol.tokenKind = SyntaxToken.Kind.PreprocessorSymbol;
						formatedLine.tokens.Add(symbol);
						symbol.formatedLine = formatedLine;

						scriptDefinesChanged = true;
						
						var inactive = formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive;
						if (!inactive)
						{
							if (token.text == "define")
							{
								if (!scriptDefines.Contains(symbol.text))
								{
									scriptDefines.Add(symbol.text);
									//scriptDefinesChanged = true;
								}
							}
							else
							{
								if (scriptDefines.Contains(symbol.text))
								{
									scriptDefines.Remove(symbol.text);
									//scriptDefinesChanged = true;
								}
							}
						}
					}
				}
				else if (token.text == "error" || token.text == "warning")
				{
					preprocessorCommentsAllowed = false;
				}
			}
			
			if (!preprocessorCommentsAllowed)
			{
				ws = ScanWhitespace(line, ref startAt);
				if (ws != null)
				{
					tokens.Add(ws);
					ws.formatedLine = formatedLine;
				}
				if (startAt < length)
				{
					var textArgument = line.Substring(startAt);
					textArgument.TrimEnd(new [] {' ', '\t'});
					tokens.Add(new SyntaxToken(SyntaxToken.Kind.PreprocessorArguments, textArgument) { formatedLine = formatedLine });
					startAt += textArgument.Length;
					if (startAt < length)
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.Whitespace, line.Substring(startAt)) { formatedLine = formatedLine });
				}
				return;
			}
			
			while (startAt < length)
			{
				ws = ScanWhitespace(line, ref startAt);
				if (ws != null)
				{
					tokens.Add(ws);
					ws.formatedLine = formatedLine;
					continue;
				}
				
				var firstChar = line[startAt];
				if (startAt < length - 1 && firstChar == '/' && line[startAt + 1] == '/')
				{
					tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) { formatedLine = formatedLine });
					break;
				}
				else if (commentsOnly)
				{
					tokens.Add(new SyntaxToken(SyntaxToken.Kind.PreprocessorCommentExpected, line.Substring(startAt)) { formatedLine = formatedLine });
					break;						
				}
				
				if (char.IsLetterOrDigit(firstChar) || firstChar == '_')
				{
					token = ScanWord(line, ref startAt);
					token.tokenKind = SyntaxToken.Kind.PreprocessorArguments;
					tokens.Add(token);
					token.formatedLine = formatedLine;
				}
				else if (firstChar == '"')
				{
					token = FGParser.ScanStringLiteral(line, ref startAt);
					token.tokenKind = SyntaxToken.Kind.PreprocessorArguments;
					tokens.Add(token);
					token.formatedLine = formatedLine;
				}
				else if (firstChar == '\'')
				{
					token = ScanCharLiteral(line, ref startAt);
					token.tokenKind = SyntaxToken.Kind.PreprocessorArguments;
					tokens.Add(token);
					token.formatedLine = formatedLine;
				}
				else
				{
					token = new SyntaxToken(SyntaxToken.Kind.PreprocessorArguments, firstChar.ToString()) { formatedLine = formatedLine };
					tokens.Add(token);
					++startAt;
				}
				
				if (error)
				{
					token.tokenKind = SyntaxToken.Kind.PreprocessorDirectiveExpected;
				}
			}
			
			return;
		}
		
		var inactiveLine = formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive;
		
		while (startAt < length)
		{
			switch (formatedLine.blockState)
			{
				case FGTextBuffer.BlockState.None:
					ws = ScanWhitespace(line, ref startAt);
					if (ws != null)
					{
						tokens.Add(ws);
						ws.formatedLine = formatedLine;
						continue;
					}
					
					if (inactiveLine)
					{
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) { formatedLine = formatedLine });
						startAt = length;
						break;
					}

					if (line[startAt] == '/' && startAt < length - 1)
					{
						if (line[startAt + 1] == '/')
						{
							tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, "//") { formatedLine = formatedLine });
							startAt += 2;
							tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) { formatedLine = formatedLine });
							startAt = length;
							break;
						}
						else if (line[startAt + 1] == '*')
						{
							tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, "/*") { formatedLine = formatedLine });
							startAt += 2;
							formatedLine.blockState = FGTextBuffer.BlockState.CommentBlock;
							break;
						}
					}

					if (line[startAt] == '\'')
					{
						token = ScanCharLiteral(line, ref startAt);
						tokens.Add(token);
						token.formatedLine = formatedLine;
						break;
					}

					if (line[startAt] == '\"' || !isCSharp4 && line[startAt] == '$')
					{
						ScanStringLiteral(line, ref startAt, formatedLine);
						break;
					}

					if (startAt < length - 1 && line[startAt] == '@' && line[startAt + 1] == '\"')
					{
						token = new SyntaxToken(SyntaxToken.Kind.VerbatimStringBegin, line.Substring(startAt, 2)) { formatedLine = formatedLine };
						tokens.Add(token);
						startAt += 2;
						formatedLine.blockState = FGTextBuffer.BlockState.StringBlock;
						break;
					}

					if (line[startAt] >= '0' && line[startAt] <= '9'
					    || startAt < length - 1 && line[startAt] == '.' && line[startAt + 1] >= '0' && line[startAt + 1] <= '9')
					{
						token = ScanNumericLiteral(line, ref startAt);
						tokens.Add(token);
						token.formatedLine = formatedLine;
						break;
					}

					token = ScanIdentifierOrKeyword(line, ref startAt);
					if (token != null)
					{
						tokens.Add(token);
						token.formatedLine = formatedLine;
						break;
					}

					// Multi-character operators / punctuators
					// "++", "--", "<<", ">>", "<=", ">=", "==", "!=", "&&", "||", "??", "+=", "-=", "*=", "/=", "%=",
					// "&=", "|=", "^=", "<<=", ">>=", "=>", "::"
					var punctuatorStart = startAt++;
					if (startAt < line.Length)
					{
						var nextCharacter = line[startAt];
						switch (line[punctuatorStart])
						{
							case '?':
								if (nextCharacter == '?')
									++startAt;
								break;
							case '+':
								if (nextCharacter == '+' || nextCharacter == '=')
									++startAt;
								break;
							case '-':
								if (nextCharacter == '-' || nextCharacter == '=')
									++startAt;
								break;
							case '<':
								if (nextCharacter == '=')
									++startAt;
								else if (nextCharacter == '<')
								{
									++startAt;
									if (startAt < line.Length && line[startAt] == '=')
										++startAt;
								}
								break;
							case '>':
								if (nextCharacter == '=')
									++startAt;
								//else if (startAt < line.Length && line[startAt] == '>')
								//{
								//    ++startAt;
								//    if (line[startAt] == '=')
								//        ++startAt;
								//}
								break;
							case '=':
								if (nextCharacter == '=' || nextCharacter == '>')
									++startAt;
								break;
							case '&':
								if (nextCharacter == '=' || nextCharacter == '&')
									++startAt;
								break;
							case '|':
								if (nextCharacter == '=' || nextCharacter == '|')
									++startAt;
								break;
							case '*':
							case '/':
							case '%':
							case '^':
							case '!':
								if (nextCharacter == '=')
									++startAt;
								break;
							case ':':
								if (nextCharacter == ':')
									++startAt;
								break;
						}
					}
					tokens.Add(new SyntaxToken(SyntaxToken.Kind.Punctuator, line.Substring(punctuatorStart, startAt - punctuatorStart)) { formatedLine = formatedLine });
					break;

				case FGTextBuffer.BlockState.CommentBlock:
					int commentBlockEnd = line.IndexOf("*/", startAt, StringComparison.Ordinal);
					if (commentBlockEnd == -1)
					{
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) { formatedLine = formatedLine });
						startAt = length;
					}
					else
					{
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt, commentBlockEnd + 2 - startAt)) { formatedLine = formatedLine });
						startAt = commentBlockEnd + 2;
						formatedLine.blockState = FGTextBuffer.BlockState.None;
					}
					break;

				case FGTextBuffer.BlockState.StringBlock:
					int i = startAt;
					int closingQuote = line.IndexOf('\"', startAt);
					while (closingQuote != -1 && closingQuote < length - 1 && line[closingQuote + 1] == '\"')
					{
						i = closingQuote + 2;
						closingQuote = line.IndexOf('\"', i);
					}
					if (closingQuote == -1)
					{
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.VerbatimStringLiteral, line.Substring(startAt)) { formatedLine = formatedLine });
						startAt = length;
					}
					else
					{
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.VerbatimStringLiteral, line.Substring(startAt, closingQuote - startAt)) { formatedLine = formatedLine });
						startAt = closingQuote;
						tokens.Add(new SyntaxToken(SyntaxToken.Kind.VerbatimStringLiteral, line.Substring(startAt, 1)) { formatedLine = formatedLine });
						++startAt;
						formatedLine.blockState = FGTextBuffer.BlockState.None;
					}
					break;
			}
		}
	}

	private new SyntaxToken ScanIdentifierOrKeyword(string line, ref int startAt)
	{
		var token = FGParser.ScanIdentifierOrKeyword(line, ref startAt);
		if (token != null && token.tokenKind == SyntaxToken.Kind.Keyword && !IsKeywordOrBuiltInType(token.text))
			token.tokenKind = SyntaxToken.Kind.Identifier;
		return token;
	}

	private bool IsKeyword(string word)
	{
		return keywords.Contains(word);
	}

	private bool IsKeywordOrBuiltInType(string word)
	{
		return keywordsAndBuiltInTypes.Contains(word);
	}

	private bool IsOperator(string text)
	{
		return csOperators.Contains(text);
	}
	
	private void ScanStringLiteral(string line, ref int startAt, FGTextBuffer.FormatedLine formatedLine)
	{
		if (line[startAt] == '$')
		{
			ScanInterpolatedStringLiteral(line, ref startAt, formatedLine);
			return;
		}
		
		var i = startAt + 1;
		while (i < line.Length)
		{
			if (line[i] == '\"')
			{
				++i;
				break;
			}
			if (line[i] == '\\' && i < line.Length - 1)
				++i;
			++i;
		}
		
		if (formatedLine != null)
		{
			var token = new SyntaxToken(SyntaxToken.Kind.StringLiteral, line.Substring(startAt, i - startAt));
			formatedLine.tokens.Add(token);
			token.formatedLine = formatedLine;
		}
		
		startAt = i;
	}
	
	private void ScanInterpolatedStringLiteral(string line, ref int startAt, FGTextBuffer.FormatedLine formatedLine)
	{
		var interpolatedStringTokenKind = SyntaxToken.Kind.InterpolatedStringStartLiteral;
		
		var i = startAt + 1;
		if (i < line.Length && line[i] == '"')
		{
			++i;

			while (i < line.Length)
			{
				char c = line[i];
				
				if (c == '{')
				{
					if (i + 1 < line.Length && line[i + 1] == '{')
					{
						++i;
					}
					else
					{
						if (formatedLine != null)
						{
							var token = new SyntaxToken(
								interpolatedStringTokenKind,
								line.Substring(startAt, i - startAt));
							formatedLine.tokens.Add(token);
							token.formatedLine = formatedLine;
						}
						
						interpolatedStringTokenKind = SyntaxToken.Kind.InterpolatedStringMidLiteral;

						startAt = i;
						int formatStartAt = SkipStringInterpolation(line, ref i);
						if (formatedLine != null)
						{
							if (formatStartAt >= 0)
							{
								Tokenize(line.Substring(startAt, formatStartAt - startAt), formatedLine);
								
								if (line[formatStartAt] == ':')
								{
									var token = new SyntaxToken(SyntaxToken.Kind.Punctuator, ":");
									formatedLine.tokens.Add(token);
									token.formatedLine = formatedLine;
									
									int formatLength = i - (formatStartAt + 1);
									if (formatLength > 0)
									{
										token = new SyntaxToken(
											SyntaxToken.Kind.InterpolatedStringFormatLiteral,
											line.Substring(formatStartAt + 1, formatLength));
										formatedLine.tokens.Add(token);
										token.formatedLine = formatedLine;
									}
								}
							}
							else
							{
								Tokenize(line.Substring(startAt, i - startAt), formatedLine);
							}
										
							if (i < line.Length && line[i] == '}')
							{
								++i;
								SyntaxToken token = new SyntaxToken(SyntaxToken.Kind.Punctuator, "}");
								formatedLine.tokens.Add(token);
								token.formatedLine = formatedLine;
							}
						}
						startAt = i;
						
						continue;
					}
				}
				
				++i;
				
				if (c == '"')
				{
					break;
				}
				
				if (c == '\\' && i < line.Length)
				{
					++i;
				}
			}
		}

		if (formatedLine != null)
		{
			SyntaxToken token;
			if (interpolatedStringTokenKind == SyntaxToken.Kind.InterpolatedStringStartLiteral)
			{
				token = new SyntaxToken(
					SyntaxToken.Kind.InterpolatedStringWholeLiteral,
					line.Substring(startAt, i - startAt));
				formatedLine.tokens.Add(token);
				token.formatedLine = formatedLine;
			}
			else
			{
				token = new SyntaxToken(
					SyntaxToken.Kind.InterpolatedStringEndLiteral,
					line.Substring(startAt, i - startAt));
				formatedLine.tokens.Add(token);
				token.formatedLine = formatedLine;
			}
		}
		
		startAt = i;
	}

	private int SkipStringInterpolation(string line, ref int i)
	{
		var length = line.Length;
		++i;
		while (i < length)
		{
			char c = line[i];
			if (c == '}' || c == ':')
				break;
			
			if (!ScanRegularBalancedText(line, ref i, true))
				break;
		}
		
		if (i >= length)
			return -1;
		
		if (line[i] == ':')
		{
			var formatStartAt = i;
			++i;
			
			// skip format string (without closing brace)	
			while (i < length)
			{
				char c = line[i];
				if (/*c == ':' ||*/ c == '"')
					break;
				if (c == '{')
				{
					if (i + 1 < line.Length && line[i + 1] == '{')
						++i;
					else
						break;
				}
				else if (c == '}')
				{
					if (i + 1 < line.Length && line[i + 1] == '}')
						++i;
					else
						break;
				}
				
				++i;
				
				if (c == '\\')
				{
					if (i < line.Length)
						++i;
					else
						break;
				}
			}

			return formatStartAt;
		}
		
		return -1;
	}

	private bool ScanRegularBalancedText(string line, ref int i, bool scanInterpolationFormat)
	{
		var startAt = i;
		var length = line.Length;
		if (i >= length)
			return false;
		
		while (i < length)
		{
			char c = line[i];
			if (c == '$' || c == '"' || c == '@' && c + 1 < length && line[c + 1] == '"')
			{
				ScanStringLiteral(line, ref i, null);
				continue;
			}
			if (c == '/' && i + 1 < length)
			{
				++i;
				
				char next = line[i];
				if (next == '/')
				{
					i = length;
					break;
				}
				else if (next == '*')
				{
					++i;
					while (i < length)
					{
						if (line[i] != '*')
						{
							++i;
							continue;
						}
						++i;
						if (i < length && line[i] == '/')
						{
							++i;
							break;
						}
					}
					continue;
				}
			}
			else if (c == '}' || c == ')' || c == ']' || scanInterpolationFormat && c == ':')
			{
				break;
			}
			
			++i;
			
			if (c == '{')
			{
				ScanRegularBalancedText(line, ref i, false);
				if (i < length && line[i] == '}')
					++i;
			}
			else if (c == '[')
			{
				ScanRegularBalancedText(line, ref i, false);
				if (i < length && line[i] == ']')
					++i;
			}
			else if (c == '(')
			{
				ScanRegularBalancedText(line, ref i, false);
				if (i < length && line[i] == ')')
					++i;
			}
		}
		
		return i > startAt;
	}
}

}
