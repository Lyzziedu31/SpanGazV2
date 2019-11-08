using SpanGazV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SpanGazV2.Controllers.Login
{
    /// <summary>
    /// Controleur de controle d'acces à l'application
    /// </summary>
    public class LoginController : Controller
    {
        private readonly database_tc2Entities db = new database_tc2Entities();
        /// <summary>
        /// Affichage de la page de login
        /// </summary>
        /// <param name="user">récupération des identifiants de la session Windows et du nom de domaine</param>
        /// <returns>Si l'utilisateur est présent dans la table actors on affiche la page d'accueil, sinon on affiche une erreur</returns>
        public ActionResult Index(string user)
        {
            //récupération du nom de domaine et de l'id de l'utilisateur loggé sur la machine
            user = System.Web.HttpContext.Current.User.Identity.Name.ToString();

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var usersearch = new tbl_607_actors();

            try
            {
                //recherche du user dans la base
                usersearch.id_uid = (db.tbl_607_actors.Where(u => u.id_uid == user)).First().id_uid;
                return RedirectToAction("Index", "HomePage");
            }
            catch (Exception ex)
            {
                //generation de la vue affichage erreur
                return RedirectToAction("Index", "Ooops", new { message = ex });
            }

        }
    }
}