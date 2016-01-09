using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using SourceMaster.Output;

namespace SourceMaster.Semantic
{
	public class SemanticInterpreter
	{
		private static readonly Dictionary<ISymbol, string> _symbolToAssignedId = new Dictionary<ISymbol, string>();
		private static readonly Dictionary<string, SymbolMetadata> _symbolIdToMetadata = new Dictionary<string, SymbolMetadata>();
		private readonly HashSet<ISymbol> _encounteredSymbolsNotInSource = new HashSet<ISymbol>();

		public Project CurrentProject { get; }

		private SemanticModel Model { get; }

		public SemanticInterpreter(Project project, SemanticModel model)
		{
			CurrentProject = project;
			Model = model;
		}

		public SymbolMetadata this[string symbolId] => _symbolIdToMetadata[symbolId];

		private bool TryGettingSymbolMetadata(ISymbol symbol, out SymbolMetadata metadata)
		{
			metadata = null;
			string symbolId;

			if (!_symbolToAssignedId.TryGetValue(symbol, out symbolId))
			{
				return false;
			}

			metadata = _symbolIdToMetadata[symbolId];
			return true;
		}

		public bool TryEnsuringSymbolMapping(SyntaxNode syntax, out SymbolMetadata associatedSymbolMetadata)
		{
			associatedSymbolMetadata = null;

			var identifierSymbol = Model.GetSymbolInfo(syntax).Symbol ?? Model.GetDeclaredSymbol(syntax);
			if (identifierSymbol is INamespaceSymbol || _encounteredSymbolsNotInSource.Contains(identifierSymbol))
			{
				return false;
			}

			if (TryGettingSymbolMetadata(identifierSymbol, out associatedSymbolMetadata))
			{
				return true;
			}

			var declaredInSource = TryGeneratingSymbolMetadata(identifierSymbol, syntax, out associatedSymbolMetadata);
			if (!declaredInSource)
			{
				_encounteredSymbolsNotInSource.Add(identifierSymbol);
				 return false;
			}

			_symbolToAssignedId[identifierSymbol] = associatedSymbolMetadata.Id;
			_symbolIdToMetadata[associatedSymbolMetadata.Id] = associatedSymbolMetadata;

			return true;
		}

		private bool TryGeneratingSymbolMetadata(ISymbol symbol, SyntaxNode syntax, out SymbolMetadata generatedSymbolMetadata)
		{
			generatedSymbolMetadata = null;

			var filePathsOfSourceDeclarations = symbol
				.Locations
				.Where(location => location.IsInSource)
				.Select(location =>  location.GetLineSpan().Path)
				.Select(path => GetRelativePathToProjectFile(path))
				.ToArray();

			if (!filePathsOfSourceDeclarations.Any())
			{
				return false;
			}

			var assignedSymbolId = Guid.NewGuid().ToString("N");

			generatedSymbolMetadata = new SymbolMetadata(
				assignedSymbolId,
				symbol.MetadataName,
				filePathsOfSourceDeclarations);

			return true;
		}

		private string GetRelativePathToProjectFile(string absoluteFilePath)
		{
			var projectDirectoryPath = Path.GetDirectoryName(CurrentProject.FilePath);

			return MakeRelativePath(projectDirectoryPath, absoluteFilePath);
		}

		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static String MakeRelativePath(String fromPath, String toPath)
		{
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);

			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.ToUpperInvariant() == "FILE")
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}
	}
}
