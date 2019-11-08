using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpanGazV2.Models;
using PagedList;

namespace SpanGazV2.Controllers.Actors
{
    /// <summary>
    /// Controleur d'administration des acteurs (Team)
    /// </summary>
    public class ActorsController : Controller
    {
        private readonly database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// retourne la liste de tous les acteurs enregistrés dans la base
        /// </summary>
        /// <param name="sortOrder">retour du filtre selectionné dans le front (X.PagedList)</param>
        /// <param name="currentFilter">retour du filtre courant dans le front (X.PagedList)</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <returns>liste de tous les acteurs filtrés en fonction des paramètres précédents</returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.UIDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_uid_desc" : "";
            ViewBag.Last_NameSortParm = sortOrder == "last_name" ? "last_name_desc" : "last_name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_Actors = from s in db.tbl_607_actors
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_Actors = tbl_607_Actors.Where(s => s.last_name.Contains(searchString)
                                       || s.id_uid.Contains(searchString)
                                       || s.first_name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "id_uid_desc":
                    tbl_607_Actors = tbl_607_Actors.OrderByDescending(s => s.id_uid);
                    break;
                case "last_name":
                    tbl_607_Actors = tbl_607_Actors.OrderBy(s => s.last_name);
                    break;
                case "last_name_desc":
                    tbl_607_Actors = tbl_607_Actors.OrderByDescending(s => s.last_name);
                    break;
                default:
                    tbl_607_Actors = tbl_607_Actors.OrderBy(s => s.id_uid);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_607_Actors.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Affichage de la page Détails de l'acteur séléctionné
        /// </summary>
        /// <param name="id">id de l'acteur selectionné</param>
        /// <returns>retourne les données correspondantes</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_actors tbl_607_actors = db.tbl_607_actors.Find(id);
            if (tbl_607_actors == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_actors);
        }

        /// <summary>
        /// affiche la page de création d'un nouvel acteur
        /// </summary>
        /// <returns>retourne la vue</returns>
        public ActionResult Create()
        {
            ViewBag.role = db.tbl_607_actors.Select(r => new SelectListItem
            {
                Text = r.role
            }).Distinct();

            return View();
        }

        /// <summary>
        /// creation d'un nouvel acteur
        /// </summary>
        /// <param name="tbl_607_actors">binding des données retournées par le front</param>
        /// <returns>vue principale si succés</returns>
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_uid,first_name,last_name,role")] tbl_607_actors tbl_607_actors)
        {
            if (ModelState.IsValid)
            {
                db.tbl_607_actors.Add(tbl_607_actors);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    string s = ex.Message;
                    return RedirectToAction("../Ooops", new { message = s });
                }
            }

            return View(tbl_607_actors);
        }

        /// <summary>
        /// affiche la page d'édition d'un acteur
        /// </summary>
        /// <param name="id">id de l'acteur selectionné</param>
        /// <returns>vue edition</returns>
        public ActionResult Edit(int? id)
        {
            ViewBag.role = db.tbl_607_actors.Select(r => new SelectListItem
            {
                Text = r.role
            }).Distinct();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_actors tbl_607_actors = db.tbl_607_actors.Find(id);
            if (tbl_607_actors == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_actors);
        }

        /// <summary>
        /// validation des modification sur l'acteur
        /// </summary>
        /// <param name="tbl_607_actors">binding des données retournées par le front</param>
        /// <returns>vue principale</returns>
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_uid,first_name,last_name,role")] tbl_607_actors tbl_607_actors)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_607_actors).State = EntityState.Modified;
                try
                {
                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        string s = ex.Message;
                        return RedirectToAction("../Ooops", new { message = s });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    string s = ex.Message;
                    return RedirectToAction("../Ooops", new { message = s });
                }
            }
            return View(tbl_607_actors);
        }

        /// <summary>
        /// Suppression d'un acteur
        /// </summary>
        /// <param name="id">id de l'acteur selectionné</param>
        /// <returns>vue suppression</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_actors tbl_607_actors = db.tbl_607_actors.Find(id);
            if (tbl_607_actors == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_actors);
        }

        /// <summary>
        /// confirmation de la suppression
        /// </summary>
        /// <param name="id">>id de l'acteur à supprimer</param>
        /// <returns>vue principale</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_actors tbl_607_actors = db.tbl_607_actors.Find(id);
            db.tbl_607_actors.Remove(tbl_607_actors);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException ex)
            {
                string s = ex.Message;
                return RedirectToAction("../Ooops", new { message = s });
            }
        }

        /// <summary>
        /// Libère les ressources non managées utilisées par Control et ses contrôles enfants et libère éventuellement les ressources managées.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
