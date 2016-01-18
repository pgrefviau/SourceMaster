using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using SourceMaster.Extensions;
using SourceMaster.Output;

namespace SourceMaster.Semantic
{
	public class SemanticCache
	{
		private static readonly Dictionary<ISymbol, string> _symbolToAssignedId = new Dictionary<ISymbol, string>();
		private static readonly Dictionary<string, SymbolMetadata> _symbolIdToMetadata = new Dictionary<string, SymbolMetadata>();
		private readonly HashSet<ISymbol> _encounteredSymbolsNotInSource = new HashSet<ISymbol>();

		public Project CurrentProject { get; }

		private SemanticModel Model { get; }

		public SemanticCache(Project project, SemanticModel model)
		{
			CurrentProject = project;
			Model = model;
		}

		public SymbolMetadata this[string symbolId] => _symbolIdToMetadata[symbolId];

		private static bool TryGettingSymbolMetadata(ISymbol symbol, out SymbolMetadata metadata)
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
			if (identifierSymbol == null || identifierSymbol is INamespaceSymbol || _encounteredSymbolsNotInSource.Contains(identifierSymbol))
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
				.Select(path => CurrentProject.GetRelativePathToFile(path))
				.ToArray();

			if (!filePathsOfSourceDeclarations.Any())
			{
				return false;
			}

			var assignedSymbolId = Guid.NewGuid().ToString("N");

			generatedSymbolMetadata = new SymbolMetadata(
				assignedSymbolId,
				symbol.MetadataName,
				filePathsOfSourceDeclarations,
				symbol.Kind);

			return true;
		}
	}
}
