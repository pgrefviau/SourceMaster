using System;
using SourceMaster;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourceMaster.Tests
{
	[TestClass]
	public class SolutionParsingTests
	{
		[TestMethod]
		public void WhenParsingSolution()
		{
			SourceParsingManager.ParseProjectSourceFiles(
				@"C:\Users\Phil\Documents\visual studio 2015\Projects\SourceMaster\SourceMaster.sln", 
				@"SourceMaster");
		}
	}
}
