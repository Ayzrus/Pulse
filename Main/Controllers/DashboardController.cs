using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{

			if (HttpContext.Session.GetString("Autenticado") != "true")
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		[HttpPost]
		public IActionResult Logout()
		{
			HttpContext.Session.Remove("Autenticado");

			return RedirectToAction("Index", "Home");
		}

	}
}
