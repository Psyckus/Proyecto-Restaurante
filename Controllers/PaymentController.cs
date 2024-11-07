using Microsoft.AspNetCore.Mvc;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;
using System.Net;
using System.Net.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PaymentController : Controller
{
    private readonly PayPalService _payPalService;
    private readonly ApplicationDbContext _context;

    public PaymentController(PayPalService payPalService, ApplicationDbContext context)
    {
        _payPalService = payPalService;
        _context = context;
    }

    public async Task<IActionResult> ProceedToPay(decimal amount)
    {
        // Update the purchase status to "pago"
        var compra = _context.Compras.FirstOrDefault(c => c.estado_compra == "Pendiente");
        if (compra != null)
        {
            compra.estado_compra = "pago";
            await _context.SaveChangesAsync();

            // Create or update a payment record associated with this purchase
            var pago = new Pagos
            {
                compra_id = compra.compra_id,
                estado_pago = "pagado",
                metodo_pago = "PayPal",
                fecha_pago = DateTime.Now
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            // Store purchase details in ViewBag to pass to the success page
            ViewBag.Total = compra.total;
            ViewBag.TransactionId = Guid.NewGuid().ToString(); // Generate a temporary transaction ID
        }

        try
        {
            // Create a payment with PayPal
            var payment = _payPalService.CreatePayment(amount, "USD",
                Url.Action("PaymentSuccess", "ShoppingCart", null, Request.Scheme),
                Url.Action("PaymentCancel", "ShoppingCart", null, Request.Scheme)
            );

            // Get the PayPal approval URL to open in a new window
            var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;
            ViewBag.PayPalUrl = approvalUrl;

            // Explicitly specify the view path for ShoppingCart/paymentsuccess
            return View("~/Views/ShoppingCart/paymentsuccess.cshtml");
        }
        catch (Exception ex)
        {
            // Log the error and redirect to PaymentSuccess with an error message
            ViewBag.Error = $"An error occurred: {ex.Message}";
            return View("~/Views/ShoppingCart/paymentsuccess.cshtml");
        }
    }


    public async Task<IActionResult> SendPaymentConfirmationEmail()
    {
        // Obtener la última compra con estado "pago"
        var compra = _context.Compras.Include(c => c.Usuario)
                                      .FirstOrDefault(c => c.estado_compra == "pago");

        if (compra == null || compra.Usuario == null)
        {
            TempData["Error"] = "No se encontraron detalles de la compra o el usuario.";
            return RedirectToAction("Index", "Home"); // Cambia "Index" por cualquier página principal de confirmación que prefieras
        }

        var usuario = compra.Usuario;

        // Configuración del correo electrónico
        var message = new MailMessage();
        message.From = new MailAddress("this.is.for.testing.only.uh@gmail.com");
        message.To.Add(usuario.correo); // Correo del usuario activo
        message.Subject = "Confirmación de Pago - MVW Proyecto Mesas Comida";
        message.Body = $@"
    ¡Gracias por tu compra, {usuario.nombre}!
    Detalles de tu compra:
    Fecha de la compra: {compra.fecha_compra:dd/MM/yyyy}
    Total pagado: {compra.total:C}
    Número de transacción: {Guid.NewGuid()}
";
        message.IsBodyHtml = false;

        // Configuración del cliente SMTP
        using (var smtp = new SmtpClient("smtp.gmail.com", 587)) // Asegúrate de que esta configuración sea correcta
        {
            smtp.Credentials = new NetworkCredential("this.is.for.testing.only.uh@gmail.com", "zfqr hkbv ixhh kbqn");
            smtp.EnableSsl = true;

            try
            {
                await smtp.SendMailAsync(message);
                TempData["Message"] = "Correo enviado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo enviar el correo: {ex.Message}";
            }
        }

        // Redirigir a la página principal o cualquier otra página de confirmación
        return RedirectToAction("Index", "Home");
    }
}   
