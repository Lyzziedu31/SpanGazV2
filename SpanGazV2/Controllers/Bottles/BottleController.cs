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

namespace SpanGazV2.Controllers.Bottles
{
    /// <summary>
    /// Controleur d'administration des bouteilles
    /// </summary>
    public class BottleController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// retourne la liste de toutes les bouteilles enregistrés dans la base
        /// </summary>
        /// <param name="sortOrder">retour du filtre selectionné dans le front (X.PagedList)</param>
        /// <param name="currentFilter">retour du filtre courant dans le front (X.PagedList)</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <returns>liste de toutes les bouteilles filtrées en fonction des paramètres précédents</returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Manufacturer_NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "manufacturer_number_desc" : "manufacturer_number_asc";
            ViewBag.Conti_NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "bottle_conti_number_desc" : "bottle_conti_number_asc";
            ViewBag.ContentSortParm = sortOrder == "order_number" ? "FK_ID_order_details_desc" : "FK_ID_order_details_asc";
            ViewBag.ConformityCertifSortParm = String.IsNullOrEmpty(sortOrder) ? "conformity_certif_desc" : "conformity_certif_asc";
            ViewBag.BottleLocationSortParm = String.IsNullOrEmpty(sortOrder) ? "location_desc" : "location_asc";
            ViewBag.BLRefSortParm = String.IsNullOrEmpty(sortOrder) ? "BLRef_desc" : "BLRef_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_bottle = db.tbl_607_bottle.Include(t => t.tbl_607_conformity_certificate).Include(t => t.tbl_607_gaz_reporting).Include(t => t.tbl_607_location).Include(t => t.tbl_607_shipping_delivery).Include(t=>t.tbl_607_order);

            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_bottle = tbl_607_bottle.Where(s => s.bottle_conti_number.ToString().Contains(searchString)
                                       || s.FK_ID_conformity_certificate.ToString().Contains(searchString)
                                       || s.FK_ID_location.ToString().Contains(searchString)
                                       || s.FK_ID_order.ToString().Contains(searchString)
                                       || s.FK_ID_shipping_delivery.ToString().Contains(searchString)
                                       || s.manufacturer_bottle_number.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "manufacturer_number_desc":
                    tbl_607_bottle = tbl_607_bottle.OrderByDescending(s => s.manufacturer_bottle_number);
                    break;
                case "manufacturer_number_asc":
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.manufacturer_bottle_number);
                    break;
                case "bottle_conti_number_desc":
                    tbl_607_bottle = tbl_607_bottle.OrderByDescending(s => s.bottle_conti_number);
                    break;
                case "bottle_conti_number_asc":
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.bottle_conti_number);
                    break;
                case "FK_ID_order_details_desc":
                    tbl_607_bottle = tbl_607_bottle.OrderByDescending(s => s.FK_ID_order);
                    break;
                case "FK_ID_order_details_asc":
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.FK_ID_order);
                    break;
               case "conformity_certif_desc":
                    tbl_607_bottle = tbl_607_bottle.OrderByDescending(s => s.tbl_607_conformity_certificate.ID);
                    break;
                case "conformity_certif_asc":
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.tbl_607_conformity_certificate.ID);
                    break;
                case "location_desc":
                    tbl_607_bottle = tbl_607_bottle.OrderByDescending(s => s.tbl_607_location.bottle_location);
                    break;
                case "location_asc":
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.tbl_607_location.bottle_location);
                    break;
                case "BLRef_desc":
                    tbl_607_bottle = tbl_607_bottle.OrderByDescending(s => s.tbl_607_shipping_delivery.BL_ref);
                    break;
                case "BLRef_asc":
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.tbl_607_shipping_delivery.BL_ref);
                    break;
                default:
                    tbl_607_bottle = tbl_607_bottle.OrderBy(s => s.bottle_conti_number);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_607_bottle.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Affichage des details de la bouteille selectionnée
        /// </summary>
        /// <param name="id">id de la bouteille selectionnée</param>
        /// <returns>vue details</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_bottle tbl_607_bottle = db.tbl_607_bottle.Find(id);
            if (tbl_607_bottle == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_bottle);
        }
        
        /// <summary>
        /// Affichage de la vue suppression de la bouteille selectionnée
        /// </summary>
        /// <param name="id">id de la bouteille selectionnée</param>
        /// <returns>vue suppression</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_bottle tbl_607_bottle = db.tbl_607_bottle.Find(id);
            if (tbl_607_bottle == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_bottle);
        }

        /// <summary>
        /// Validation de la suppression de la bouteille selectionnée
        /// </summary>
        /// <param name="id">id de la bouteille selectionnée</param>
        /// <returns>vue principale</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_bottle tbl_607_bottle = db.tbl_607_bottle.Find(id);
            
            var join_bottle = db.tbl_607_join_bottle_gaz.Where(t => t.FK_ID_bottle == id).ToList();
            foreach (var item in join_bottle)
            {
                db.tbl_607_join_bottle_gaz.Remove(item);
                db.SaveChanges();
            }

            db.tbl_607_bottle.Remove(tbl_607_bottle);
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
