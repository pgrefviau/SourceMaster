using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SourceMaster.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{action}",
				defaults: new
				{
					controller = "Main",
					action = "Index"
				}
			);

			routes.MapRoute(
				name: "GetFileSource",
				url: "{projectName}/{*filePath}",
				defaults: new
				{
					controller = "Main",
					action = "GetFileSource"
				}
			);
		}
	}
}
