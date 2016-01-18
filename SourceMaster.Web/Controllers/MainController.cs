using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SourceMaster.Web.Models;

namespace SourceMaster.Web.Controllers
{
	public class MainController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult GetFileSource(string projectName, string filePath)
		{
			var parsedProjectSourceFiles = SourceParsingManager.ParseProjectSourceFiles(
				@"C:\Users\Phil\Documents\visual studio 2015\Projects\SourceMaster\SourceMaster.sln",
				@"SourceMaster");

			var projectParsingResults = parsedProjectSourceFiles[projectName];
			var adjustedFilePath = filePath.Replace('/', '\\');
			var fullFilePath = Path.Combine(projectName, adjustedFilePath);

			var sourceFile = projectParsingResults?[fullFilePath];
			var parentDirectoryPath = Path.GetDirectoryName(fullFilePath) ?? "";

			var mappings = projectParsingResults
				.Where(kvp => kvp.Key.StartsWith(parentDirectoryPath))
				.ToDictionary(kvp => kvp.Key.Replace(parentDirectoryPath, string.Empty), kvp => kvp.Value.Path);

			return View(
				new FileSourceViewModel
				{
					Content = sourceFile,
					CurrentDirectoryFileNamesToPaths = mappings
				}
			);
		}

		//[HttpGet]
		//public ActionResult GetFilesInDirectory(string projectName, string path)
		//{
		//	var adjustedFilePath = path.Replace('/', '\\');
		//	var fullFilePath = Path.Combine(projectName, adjustedFilePath);

		//	var sourceFile = projectParsingResults?[fullFilePath];

		//	return View(sourceFile);
		//}
	}
}