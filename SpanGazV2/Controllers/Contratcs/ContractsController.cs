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

namespace SpanGazV2.Controllers.Contratcs
{
    /// <summary>
    /// Controleur d'administration des Contrats
    /// </summary>
    public class ContractsController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// retourne la liste de tous les contrats enregistrés dans la base
        /// </summary>
        /// <param name="sortOrder">retour du filtre selectionné dans le front (PagedList)</param>
        /// <param name="currentFilter">retour du filtre courant dans le front (PagedList)</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (PagedList)</param>
        /// <returns>liste de tous les contrats filtrées en fonction des paramètres précédents</returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.OrderSortParm = String.IsNullOrEmpty(sortOrder) ? "order_number_asc" : "";
            ViewBag.ProviderSortParm = sortOrder == "FK_ID_provider" ? "FK_ID_provider_desc" : "FK_ID_provider_details";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_order = db.tbl_607_order.Include(t => t.tbl_607_provider);

            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_order = tbl_607_order.Where(s => s.order_number.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "order_number_asc":
                    tbl_607_order = tbl_607_order.OrderByDescending(s => s.order_number);
                    break;
                case "FK_ID_provider_details":
                    tbl_607_order = tbl_607_order.OrderBy(s => s.FK_ID_provider);
                    break;
                case "FK_ID_provider_desc":
                    tbl_607_order = tbl_607_order.OrderByDescending(s => s.FK_ID_provider);
                    break;
                default:
                    tbl_607_order = tbl_607_order.OrderBy(s => s.order_number);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_607_order.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        ///  Affichage des details du contrate selectionné
        /// </summary>
        /// <param name="id">id du contrat sélectionné</param>
        /// <returns>vue details</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_order tbl_607_order = db.tbl_607_order.Find(id);
            if (tbl_607_order == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_order);
        }

        /// <summary>
        /// Affichage de la vue suppression du contrat selectionné
        /// </summary>
        /// <returns>vue d'entrée des données du nouveau contrat</returns>
        public ActionResult Create()
        {
            ViewBag.FK_ID_provider = new SelectList(db.tbl_607_provider, "ID", "provider_name");
            return View();
        }

        /// <summary>
        /// Binding des données de création du nouveau contrat
        /// </summary>
        /// <param name="tbl_607_order">datas</param>
        /// <returns>vue principale</returns>
        // POST: Contracts/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,start_date,end_date,order_number,shipping_request_active,item_reception_active,FK_ID_provider")] tbl_607_order tbl_607_order)
        {
            if (ModelState.IsValid)
            {
                db.tbl_607_order.Add(tbl_607_order);
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

            ViewBag.FK_ID_provider = new SelectList(db.tbl_607_provider, "ID", "provider_name", tbl_607_order.FK_ID_provider);
            return View(tbl_607_order);
        }

        /// <summary>
        /// affiche la page d'édition d'un contrat
        /// </summary>
        /// <param name="id">id du contrat séléctionné</param>
        /// <returns>vue d'édition</returns>
        // GET: Contracts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_order tbl_607_order = db.tbl_607_order.Find(id);
            if (tbl_607_order == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_ID_provider = new SelectList(db.tbl_607_provider, "ID", "provider_name", tbl_607_order.FK_ID_provider);
            return View(tbl_607_order);
        }

        /// <summary>
        /// Binding des données d'édition du contrat
        /// </summary>
        /// <param name="tbl_607_order">datas</param>
        /// <returns>vue principale</returns>
        // POST: Contracts/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,start_date,end_date,order_number,shipping_request_active,item_reception_active,FK_ID_provider")] tbl_607_order tbl_607_order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_607_order).State = EntityState.Modified;
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
            ViewBag.FK_ID_provider = new SelectList(db.tbl_607_provider, "ID", "provider_name", tbl_607_order.FK_ID_provider);
            return View(tbl_607_order);
        }

        /// <summary>
        /// Suppression d'un contrat
        /// </summary>
        /// <param name="id">id du contrat sélectionné</param>
        /// <returns>vue suppression</returns>
        // GET: Contracts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_order tbl_607_order = db.tbl_607_order.Find(id);
            if (tbl_607_order == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_order);
        }

        /// <summary>
        /// confirmation de la suppression
        /// </summary>
        /// <param name="id">>id de l'acteur à supprimer</param>
        /// <returns>vue principale</returns>
        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_order tbl_607_order = db.tbl_607_order.Find(id);
            db.tbl_607_order.Remove(tbl_607_order);
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
