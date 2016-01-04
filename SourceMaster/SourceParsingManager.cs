using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using SourceMaster.Output;
using SourceMaster.Semantic;
using SourceMaster.Syntax;

namespace SourceMaster
{
	public static class SourceParsingManager
	{
		public static void ParseSolutionSourceFiles(string solutionPath)
		{
			var solution = GetSolution(solutionPath);

			foreach (var project in solution.Projects)
			{
				ParseProjectSourceFiles(project);
			}
		}

		public static void ParseProjectSourceFiles(string solutionPath, params string[] projectNames)
		{
			var solution = GetSolution(solutionPath);
			var projects = solution.Projects.Where(proj => projectNames.Contains(proj.Name));

			foreach (var project in projects)
			{
				ParseProjectSourceFiles(project);
			}
		}

		private static Solution GetSolution(string solutionPath)
		{
			// start Roslyn workspace
			var workspace = MSBuildWorkspace.Create();
			
			// open solution we want to analyze
			var solutionToAnalyze = workspace
				.OpenSolutionAsync(solutionPath)
				.Result;

			return solutionToAnalyze;
		}

		private static void ParseProjectSourceFiles(Project targetProject)
		{
			foreach (var document in targetProject.Documents)
			{
				var tree = document.GetSyntaxTreeAsync().Result;
				if (tree.HasCompilationUnitRoot)
				{
					var root = tree.GetCompilationUnitRoot();
					var model = document.GetSemanticModelAsync().Result;
					var semanticInterpreter = new SemanticInterpreter(targetProject.Solution, model);

					ParseFileSyntax(root, semanticInterpreter);
				}
			}
		}

		private static List<CompositeSyntaxElement> ParseFileSyntax(CompilationUnitSyntax root, SemanticInterpreter semanticInterpreter)
		{
			var walker = new FileSyntaxWalker(semanticInterpreter);
			walker.Visit(root);

			return walker.Elements;
		}

		private static string GetRelativePathFromProjectToDocument(Document document)
		{
			return Path.Combine(document.Folders.ToArray());
		}
	}
}
