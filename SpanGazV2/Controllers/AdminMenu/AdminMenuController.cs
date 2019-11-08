using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpanGazV2.Models;

namespace SpanGazV2.Controllers.AdminMenu
{
    /// <summary>
    /// Controleur de gestion des accés au menus d'administration
    /// </summary>
    public class AdminMenuController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// affiche la page d'accueil de l'activité d'administration, cette activité n'est accessible qu'aux acteurs disposant du rôle "administrateur"
        /// </summary>
        /// <param name="user">UID de l'acteur</param>
        /// <returns>vue administration</returns>
        public ActionResult Index(string user)
        {
            user = System.Web.HttpContext.Current.User.Identity.Name;

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var usersearch = new tbl_607_actors();

            try
            {
                usersearch.id_uid = (db.tbl_607_actors
                                    .Where(u => u.id_uid == user.ToString() & u.role == "Manager")).First().id_uid;
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Ooops", new { message = "Reserved to Managers" });
            }

        }
    }
}