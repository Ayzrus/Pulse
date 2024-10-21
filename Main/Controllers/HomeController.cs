using Main.Class;
using Main.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
			if (HttpContext.Session.GetString("Autenticado") == "true")
			{
				return RedirectToAction("Index", "Dashboard");
			}
			return View();
        }

        public IActionResult Registar()
        {
			if (HttpContext.Session.GetString("Autenticado") == "true")
			{
				return RedirectToAction("Index", "Dashboard");
			}
			return View();
        }

        [HttpPost]
        public JsonResult CriarConta([FromBody] UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                // Cria um novo usuário com os dados do modelo
                UserClass usuario = new()
                {
                    Nome = model.Fullname,
                    Username = model.Username,
                    Email = model.Email,
                    Telefone = int.Parse(model.Telefone),
                    Password = model.Password,
                    Foto = model.Foto
                };

                // Tenta inserir o usuário no banco de dados e captura o resultado
                bool sucesso = UserClass.InserirUsuario(usuario);

                if (sucesso)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao registrar o usuário. Tente novamente." });
                }
            }

            // Se houver erros de validação
            return Json(new { success = false, message = "Erro ao registrar o usuário." });
        }

		[HttpPost]
		public JsonResult Login([FromBody] LoginModel model)
		{
			if (ModelState.IsValid)
			{
				var (success, message, username, foto, nome, telefone, email) = UserClass.ValidarLogin(model.UsernameOrEmail, model.Password);

				if (!success)
				{
					return Json(new { success = false, message });
				}

				HttpContext.Session.SetString("Autenticado", "true");
				HttpContext.Session.SetString("username", username);
				HttpContext.Session.SetString("foto", foto);
				HttpContext.Session.SetString("nome", nome);
				HttpContext.Session.SetString("telefone", telefone);
				HttpContext.Session.SetString("email", email);

				// Lógica para autenticação bem-sucedida aqui (por exemplo, criar cookie de sessão)
				return Json(new { success = true });
			}

			return Json(new { success = false, message = "Dados de login inválidos." });
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Error404()
        {
            return View();
        }

        // Error handling action
        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            // Check for 404 status code
            if (statusCode == 404)
            {
                return View("Error404"); // Return your custom 404 view
            }

            return View("Error"); // Return a generic error view for other status codes
        }

    }
}
