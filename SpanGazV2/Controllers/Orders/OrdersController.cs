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


namespace SpanGazV2.Controllers.Orders
{
    /// <summary>
    /// Controleur d'administration des commandes
    /// </summary>
    public class OrdersController : Controller
    {

        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// retourne la liste de toutes les commandes enregistrées dans la base
        /// </summary>
        /// <param name="sortOrder">retour du filtre selectionné dans le front (X.PagedList)</param>
        /// <param name="currentFilter">retour du filtre courant dans le front (X.PagedList)</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <returns>liste de toutes les commandes filtrés en fonction des paramètres précédents</returns>
        // GET: Orders
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Contract_NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "order_ref_desc" : "";
            ViewBag.Contract_NumberSortParm = sortOrder == "order_number" ? "order_number_desc" : "order_number";
            ViewBag.First_NameSortParm = sortOrder == "first_name" ? "first_name_desc" : "first_name";
            ViewBag.Request_DateSortParm = sortOrder == "request_date" ? "request_date_desc" : "request_date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_shipping_request = db.tbl_607_shipping_request.Include(t => t.tbl_607_actors).Include(t => t.tbl_607_order);
            
            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_shipping_request = tbl_607_shipping_request.Where(s => s.tbl_607_order.order_number.Contains(searchString)
                                       || s.tbl_607_actors.first_name.Contains(searchString)
                                       || s.request_date.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "order_ref_desc":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderByDescending(s => s.shipping_request_ref);
                    break;
                case "order_number":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderBy(s => s.tbl_607_order.order_number);
                    break;
                case "order_number_desc":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderByDescending(s => s.tbl_607_order.order_number);
                    break;
                case "first_name":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderBy(s => s.tbl_607_actors.first_name);
                    break;
                case "first_name_desc":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderByDescending(s => s.tbl_607_actors.first_name);
                    break;
                case "request_date":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderBy(s => s.request_date);
                    break;
                case "request_date_desc":
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderByDescending(s => s.request_date);
                    break;
                default:
                    tbl_607_shipping_request = tbl_607_shipping_request.OrderBy(s => s.shipping_request_ref);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_607_shipping_request.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Affichage de la page détails de la commande sélectionnée
        /// </summary>
        /// <param name="id">id de la commande sélectionnée</param>
        /// <returns>vue détails</returns>
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_shipping_request tbl_607_shipping_request = db.tbl_607_shipping_request.Find(id);
            if (tbl_607_shipping_request == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_shipping_request);
        }

        /// <summary>
        /// Affichage de la vue d'édition d'une commande
        /// </summary>
        /// <param name="id">id de la commande sélectionnée</param>
        /// <returns>vue d'édition</returns>
        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_shipping_request tbl_607_shipping_request = db.tbl_607_shipping_request.Find(id);
            if (tbl_607_shipping_request == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_ID_actors = new SelectList(db.tbl_607_actors, "id", "first_name", tbl_607_shipping_request.FK_ID_actors);
            ViewBag.FK_order = new SelectList(db.tbl_607_order, "ID", "order_number", tbl_607_shipping_request.FK_order);
            return View(tbl_607_shipping_request);
        }

        /// <summary>
        /// Binding des données éditées
        /// </summary>
        /// <param name="tbl_607_shipping_request">data</param>
        /// <returns>vue principale</returns>
        // POST: Orders/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,request_date,FK_order,FK_ID_actors")] tbl_607_shipping_request tbl_607_shipping_request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_607_shipping_request).State = EntityState.Modified;
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
            ViewBag.FK_ID_actors = new SelectList(db.tbl_607_actors, "id", "first_name", tbl_607_shipping_request.FK_ID_actors);
            ViewBag.FK_order = new SelectList(db.tbl_607_order, "ID", "order_number", tbl_607_shipping_request.FK_order);
            return View(tbl_607_shipping_request);
        }

        /// <summary>
        /// Affichage de la vue de suppression d'une commande
        /// </summary>
        /// <param name="id">id de la commande sélectionnée</param>
        /// <returns>vue suppression</returns>
        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_shipping_request tbl_607_shipping_request = db.tbl_607_shipping_request.Find(id);
            if (tbl_607_shipping_request == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_shipping_request);
        }

        /// <summary>
        /// Suppression de la commande sélectionnée
        /// </summary>
        /// <param name="id">id de la commande sélectionnée</param>
        /// <returns></returns>
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_shipping_request tbl_607_shipping_request = db.tbl_607_shipping_request.Find(id);
            db.tbl_607_shipping_request.Remove(tbl_607_shipping_request);
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
