using SpanGazV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpanGazV2.Controllers.ErrorPages
{
    /// <summary>
    /// Gestion des erreurs
    /// </summary>
    public class OoopsController : Controller
    {
        /// <summary>
        /// affichage de la page d'erreur
        /// </summary>
        /// <param name="message">message retourné par le controleur qui lève l'exception</param>
        /// <returns>vue erreur et message</returns>
        // GET: Ooops
        public ActionResult Index(string message)
        {
            ViewBag.message = message;
            return View();
        }
             
    }
}