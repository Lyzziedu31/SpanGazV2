using SpanGazV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SpanGazV2.Controllers.HomePage
{
    /// <summary>
    /// Controleur de la page d'accueil
    /// </summary>
    public class HomePageController : Controller
    {
        private readonly database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// Affichage du NewsFeeder
        /// </summary>
        /// <returns>Affichage des commentaires</returns>
        // GET: Comments
        public ActionResult Index()
        {
            return View(db.tbl_607_comments.OrderByDescending(t => t.Date).ToList());
        }

        /// <summary>
        /// Affichage des détails du commentaire sélectionné
        /// </summary>
        /// <param name="id">id du commentaire sélectionné</param>
        /// <returns>vue détails</returns>
        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_comments tbl_607_comments = db.tbl_607_comments.Find(id);
            if (tbl_607_comments == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_comments);
        }

        /// <summary>
        /// Affichage de la vue création d'un commentaire
        /// </summary>
        /// <returns></returns>
        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Binding des données du nouveau commentaire
        /// </summary>
        /// <param name="tbl_607_comments">data</param>
        /// <returns>vue principale</returns>
        // POST: Comments/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ID_UID,Date,Comment")] tbl_607_comments tbl_607_comments)
        {
            if (ModelState.IsValid)
            {
                //sauvegarde du commentaire
                db.tbl_607_comments.Add(tbl_607_comments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_607_comments);
        }

        /// <summary>
        /// Affichage de la vue d'édition
        /// </summary>
        /// <param name="id">id du commentaire sélectionné</param>
        /// <returns>vue édition</returns>
        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_comments tbl_607_comments = db.tbl_607_comments.Find(id);
            if (tbl_607_comments == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_comments);
        }

        /// <summary>
        /// Binding des données du commentaire modifié
        /// </summary>
        /// <param name="id">id du commentaire sélectionné</param>
        /// <returns>vue principale</returns>
        // POST: Comments/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ID_UID,Date,Comment")] tbl_607_comments tbl_607_comments)
        {
            if (ModelState.IsValid)
            {
                //sauvegarde des modifications
                db.Entry(tbl_607_comments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_607_comments);
        }

        /// <summary>
        /// Affichage de la vue de suppression
        /// </summary>
        /// <param name="id">id du commentaire sélectionné</param>
        /// <returns>vue suppression</returns>
        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_comments tbl_607_comments = db.tbl_607_comments.Find(id);
            if (tbl_607_comments == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_comments);
        }

        /// <summary>
        /// validation de la suppression
        /// </summary>
        /// <param name="id">id du commentaire à supprimer</param>
        /// <returns>vue principale</returns>
        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //enregistrement de la suppression
            tbl_607_comments tbl_607_comments = db.tbl_607_comments.Find(id);
            db.tbl_607_comments.Remove(tbl_607_comments);
            db.SaveChanges();
            return RedirectToAction("Index");
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