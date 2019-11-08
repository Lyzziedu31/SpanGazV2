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

namespace SpanGazV2.Controllers.Gaz
{
    /// <summary>
    /// Controleur d'administration des Gaz
    /// </summary>
    public class GazController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        ///  retourne la liste de tous les contrats
        /// </summary>
        /// <returns>dropdownlist des contrats</returns>
        // GET: Gaz
        public ActionResult Index()
        {
            var contracts = db.tbl_607_order.ToList();
            List<SelectListItem> contractsList = new List<SelectListItem>();
            foreach (tbl_607_order tbl_607_Order in contracts)
            {
                contractsList.Add(new SelectListItem
                {
                    Text = tbl_607_Order.order_number.ToString(),
                    Value = tbl_607_Order.ID.ToString()
                });
            }
            ViewBag.Contracts = contractsList;
            return View();

        }

        /// <summary>
        /// Selection du poste
        /// </summary>
        /// <param name="IDOrderNumber">id du contrat sélectionné</param>
        /// <returns>vue partielle postes</returns>
        [HttpGet]
        public ActionResult GetPostes(int IDOrderNumber)
        {
            var postes = db.tbl_607_order_details.Where(s => s.FK_ID_order == IDOrderNumber).ToList();
            List<SelectListItem> postesList = new List<SelectListItem>();
            foreach (tbl_607_order_details tbl_607_Order_Details in postes)
            {
                postesList.Add(new SelectListItem
                {
                    Text = tbl_607_Order_Details.poste_number.ToString(),
                    Value = tbl_607_Order_Details.ID.ToString()

                });
            }
            ViewBag.FK_ID_order_details = postesList;
            return View("_PartialPostesGaz");
        }

        /// <summary>
        /// Affichage de la liste des gaz sur le poste sélectionné
        /// </summary>
        /// <param name="IDPosteNumber">id du poste sélectionné</param>
        /// <returns>liste des gaz</returns>
        public ActionResult GetPosteDetails(int IDPosteNumber)
        {
            var tbl_607_gaz = (db.tbl_607_gaz.Include(t => t.tbl_607_gaz_type).Include(t => t.tbl_607_made_tolerance).Include(t => t.tbl_607_testing_tolerance).Include(t => t.tbl_607_theorical_content).Include(t=>t.tbl_607_units).Include(t => t.tbl_607_units1).Include(t => t.tbl_607_units2)).Where(s => s.FK_ID_order_details == IDPosteNumber);
            return View("_PartialGazByPoste", tbl_607_gaz.ToList());
        }

        /// <summary>
        /// affiche la page de création d'un Gaz
        /// </summary>
        /// <param name="IDPosteNumber">id du poste qui va contenir le Gaz</param>
        /// <returns>retourne la vue Création</returns>
        // GET: Gaz/Create
        public ActionResult Create(int IDPosteNumber)
        {
            ViewBag.FK_ID_gaz_type = new SelectList(db.tbl_607_gaz_type.OrderBy(e=>e.gaz_type), "ID", "gaz_type");
            ViewBag.FK_ID_made_tolerance = new SelectList(db.tbl_607_made_tolerance.OrderBy(e=>e.made_tolerance), "ID", "made_tolerance");
            ViewBag.FK_ID_unit_made_tolerance = new SelectList(db.tbl_607_units, "ID", "unit");
            ViewBag.FK_ID_testing_tolerance = new SelectList(db.tbl_607_testing_tolerance.OrderBy(e=>e.testing_tolerance), "ID", "testing_tolerance");
            ViewBag.FK_ID_unit_testing_tolerance = new SelectList(db.tbl_607_units, "ID", "unit");
            ViewBag.FK_ID_theorical_content = new SelectList(db.tbl_607_theorical_content.OrderBy(e=>e.theorical_content), "ID", "theorical_content");
            ViewBag.FK_ID_unit_theorical_content = new SelectList(db.tbl_607_units, "ID", "unit");
            ViewBag.FK_ID_order_details = IDPosteNumber;

            return View("_CreatePartialGazByPoste");
        }

        /// <summary>
        /// Création du Gaz, Binding des données
        /// </summary>
        /// <param name="tbl_607_gaz">data</param>
        /// <param name="IDPosteNumber">id du poste qui va contenir le Gaz</param>
        /// <returns></returns>
        // POST: Gaz/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FK_ID_order_details,FK_ID_theorical_content,FK_ID_made_tolerance,FK_ID_unit_made_tolerance,FK_ID_unit_theorical_content,FK_ID_gaz_type,FK_ID_testing_tolerance,FK_ID_unit_testing_tolerance")] tbl_607_gaz tbl_607_gaz, int IDPosteNumber)
        {

            if (ModelState.IsValid)
            {
                db.tbl_607_gaz.Add(tbl_607_gaz);
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

            ViewBag.FK_ID_gaz_type = new SelectList(db.tbl_607_gaz_type.OrderBy(e => e.gaz_type), "ID", "gaz_type", tbl_607_gaz.FK_ID_gaz_type);
            ViewBag.FK_ID_made_tolerance = new SelectList(db.tbl_607_made_tolerance.OrderBy(e => e.made_tolerance), "ID", "made_tolerance", tbl_607_gaz.FK_ID_made_tolerance);
            ViewBag.FK_ID_unit_made_tolerance = new SelectList(db.tbl_607_units, "ID", "unit", tbl_607_gaz.FK_ID_unit_made_tolerance);
            ViewBag.FK_ID_testing_tolerance = new SelectList(db.tbl_607_testing_tolerance.OrderBy(e => e.testing_tolerance), "ID", "testing_tolerance", tbl_607_gaz.FK_ID_testing_tolerance);
            ViewBag.FK_ID_unit_testing_tolerance = new SelectList(db.tbl_607_units, "ID", "unit", tbl_607_gaz.FK_ID_unit_testing_tolerance);
            ViewBag.FK_ID_theorical_content = new SelectList(db.tbl_607_theorical_content.OrderBy(e => e.theorical_content), "ID", "theorical_content", tbl_607_gaz.FK_ID_theorical_content);
            ViewBag.FK_ID_unit_theorical_content = new SelectList(db.tbl_607_units, "ID", "unit", tbl_607_gaz.FK_ID_unit_theorical_content);
            return View(tbl_607_gaz);
        }
      
        /// <summary>
        /// Affichage de la vue Suppression
        /// </summary>
        /// <param name="id">id du Gaz sélectionné</param>
        /// <returns>Vue Suppression</returns>
        // GET: Gaz/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_gaz tbl_607_gaz = db.tbl_607_gaz.Find(id);
            if (tbl_607_gaz == null)
            {
                return HttpNotFound();
            }
            return View("_DeletePartialGazByPoste", tbl_607_gaz);
        }

        /// <summary>
        /// Suppression du Gaz sélectionné
        /// </summary>
        /// <param name="id">id du Gaz sélectionné</param>
        /// <returns>vue principale</returns>
        // POST: Gaz/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_gaz tbl_607_gaz = db.tbl_607_gaz.Find(id);
            db.tbl_607_gaz.Remove(tbl_607_gaz);
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
