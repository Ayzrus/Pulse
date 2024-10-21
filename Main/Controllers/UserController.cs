using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers
{
	public class UserController : Controller
	{
		public IActionResult Index()
		{

			if (HttpContext.Session.GetString("Autenticado") != "true")
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}
	}
}
