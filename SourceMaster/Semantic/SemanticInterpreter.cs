using System;
using System.Collections.Generic;
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
		private readonly Dictionary<ISymbol, string> _symbolToAssignedId = new Dictionary<ISymbol, string>();
		private readonly Dictionary<string, SymbolMetadata> _symbolIdToMetadata = new Dictionary<string, SymbolMetadata>();
		private readonly HashSet<ISymbol> _encounteredSymbolsNotInSource = new HashSet<ISymbol>();


		public Solution Solution { get; }
		private SemanticModel Model { get; }


		public SemanticInterpreter(Solution solution, SemanticModel model)
		{
			Solution = solution;
			Model = model;
		}

		public SymbolMetadata this[string symbolId] => _symbolIdToMetadata[symbolId];

		private bool TryGetSymbolMetadata(ISymbol symbol, out SymbolMetadata metadata)
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

		public bool EnsureSymbolMapping(SyntaxNode syntax, out SymbolMetadata associatedSymbolMetadata)
		{
			var identifierSymbol = Model.GetSymbolInfo(syntax).Symbol ?? Model.GetDeclaredSymbol(syntax);
			associatedSymbolMetadata = null;

			if (identifierSymbol is INamespaceSymbol || _encounteredSymbolsNotInSource.Contains(identifierSymbol))
			{
				return false;
			}

			if (TryGetSymbolMetadata(identifierSymbol, out associatedSymbolMetadata))
			{
				return true;
			}

			// TODO: Check if symbol is defined in source before adding. Otherwise, treat as litteral
			//if (associatedSymbolMetadata.)
			//{
				// _encounteredSymbolsNotInSource.Add()
				// return false;
			//}

			var assignedSymbolId = Guid.NewGuid().ToString("N");

			_symbolToAssignedId[identifierSymbol] = assignedSymbolId;

			associatedSymbolMetadata = GetSymbolMetadata(identifierSymbol, syntax, assignedSymbolId);
			_symbolIdToMetadata[assignedSymbolId] = associatedSymbolMetadata;

			return true;
		}

		private SymbolMetadata GetSymbolMetadata(ISymbol symbol, SyntaxNode syntax, string id)
		{
			var declarations = symbol
				.DeclaringSyntaxReferences
				.Select(@ref => @ref.GetSyntax())
				.ToArray();

			if (declarations.Length == 1)
			{
			}

			var references = SymbolFinder.FindReferencesAsync(symbol, Solution)
				.Result.
				ToArray();
			
			return new SymbolMetadata(id);
		}
	}
}
