using Microsoft.AspNetCore.Mvc;

namespace MVW_Proyecto_Mesas_Comida.Controllers
{
	public class Dashboard : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
