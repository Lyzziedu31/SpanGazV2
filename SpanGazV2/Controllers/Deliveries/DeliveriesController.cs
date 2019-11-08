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

namespace SpanGazV2.Controllers.Deliveries
{
    /// <summary>
    /// Controleur d'administration des livraisons
    /// </summary>
    public class DeliveriesController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// retourne la liste de toutes les livraisons enregistrées dans la base
        /// </summary>
        /// <param name="sortOrder">retour du filtre selectionné dans le front (X.PagedList)</param>
        /// <param name="currentFilter">retour du filtre courant dans le front (X.PagedList)</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <returns>liste de toutes les livraisons filtrées en fonction des paramètres précédents</returns>
        // GET: Deliveries
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.BL_refSortParm = String.IsNullOrEmpty(sortOrder) ? "BL_ref_desc" : "";
            ViewBag.Reception_dateSortParm = sortOrder == "reception_date" ? "reception_date_desc" : "reception_date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_shipping_delivery = db.tbl_607_shipping_delivery.Include(t => t.tbl_607_shipping_request);
            
            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_shipping_delivery = tbl_607_shipping_delivery.Include(t => t.tbl_607_shipping_request).Where(s => s.BL_ref.ToString().Contains(searchString)
                                       || s.reception_date.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "BL_ref_desc":
                    tbl_607_shipping_delivery = tbl_607_shipping_delivery.OrderByDescending(s => s.BL_ref);
                    break;
                case "reception_date":
                    tbl_607_shipping_delivery = tbl_607_shipping_delivery.OrderBy(s => s.reception_date);
                    break;
                case "reception_date_desc":
                    tbl_607_shipping_delivery = tbl_607_shipping_delivery.OrderByDescending(s => s.reception_date);
                    break;
                default:
                    tbl_607_shipping_delivery = tbl_607_shipping_delivery.OrderBy(s => s.BL_ref);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_607_shipping_delivery.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Affichage de la page Détails de la livraison séléctionnée
        /// </summary>
        /// <param name="id">id de la livraison selectionnée</param>
        /// <returns>retourne les données correspondantes</returns>
        // GET: Deliveries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_shipping_delivery tbl_607_shipping_delivery = db.tbl_607_shipping_delivery.Find(id);
            if (tbl_607_shipping_delivery == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_shipping_delivery);
        }

        /// <summary>
        /// affiche la page d'édition d'une livraison
        /// </summary>
        /// <param name="id">id de la livraison sélectionnée</param>
        /// <returns>vue d'édition</returns>
        // GET: Deliveries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_shipping_delivery tbl_607_shipping_delivery = db.tbl_607_shipping_delivery.Find(id);
            if (tbl_607_shipping_delivery == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_ID_shipping_request = new SelectList(db.tbl_607_shipping_request, "ID", "ID", tbl_607_shipping_delivery.FK_ID_shipping_request);
            return View(tbl_607_shipping_delivery);
        }

        /// <summary>
        /// obtention des détails de la demande de livraison rattachée sur la vue d'édition
        /// </summary>
        /// <param name="IDShippingRequest">id de la demande de livraison</param>
        /// <returns>ref de la demande et nom/prénom du demandeur</returns>
        [HttpGet]
        public ActionResult GetShippingRequest(int IDShippingRequest)
        {
            var shippingRequest = db.tbl_607_shipping_request.Include(t => t.tbl_607_shipping_request_details).Include(t => t.tbl_607_order).Where(s => s.ID == IDShippingRequest).ToList();

            return View("_PartialShippingRequest", shippingRequest);
        }

        /// <summary>
        /// validation des données d'édition
        /// </summary>
        /// <param name="tbl_607_shipping_delivery">data</param>
        /// <returns>vue principale</returns>
        // POST: Deliveries/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BL_ref,reception_date,FK_ID_shipping_request")] tbl_607_shipping_delivery tbl_607_shipping_delivery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_607_shipping_delivery).State = EntityState.Modified;
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
            ViewBag.FK_ID_shipping_request = new SelectList(db.tbl_607_shipping_request, "ID", "ID", tbl_607_shipping_delivery.FK_ID_shipping_request);
            return View(tbl_607_shipping_delivery);
        }

        /// <summary>
        /// Suppression d'une livraison
        /// </summary>
        /// <param name="id">id de la livraison selectionnée</param>
        /// <returns>vue suppression</returns>
        // GET: Deliveries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_shipping_delivery tbl_607_shipping_delivery = db.tbl_607_shipping_delivery.Find(id);
            if (tbl_607_shipping_delivery == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_shipping_delivery);
        }

        /// <summary>
        /// confirmation de la suppression
        /// </summary>
        /// <param name="id">>id de la livraison à supprimer</param>
        /// <returns>vue principale</returns>
        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_shipping_delivery tbl_607_shipping_delivery = db.tbl_607_shipping_delivery.Find(id);
            db.tbl_607_shipping_delivery.Remove(tbl_607_shipping_delivery);
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
