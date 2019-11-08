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
using X.PagedList;

namespace SpanGazV2.Controllers.Postes
{
    /// <summary>
    /// Controleur d'administration des postes
    /// </summary>
    public class PostesController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();
        /// <summary>
        /// Variable static pour stockage du numéro de contrat selectionné
        /// </summary>
        public static int IDContract;
        /// <summary>
        /// Variable static pour stockage du numéro de poste avant edition
        /// </summary>
        public static int ?IDPoste;

        /// <summary>
        /// Affichage du DropDownList listant les contrats
        /// </summary>
        /// <returns>liste des contrats</returns>
        // GET: Postes
        public ActionResult Index()
        {
            //mise à zero de la variable IDContract
            IDContract = 0;
            IDPoste = 0;

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
        /// retourne la liste des postes
        /// </summary>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <param name="IDContractNumber">numéro du contrat sélectionné</param>
        /// <returns>liste de tous les postes filtrés en fonction des paramètres précédents</returns>
        public ActionResult GetPostes(string searchString, int? page, int IDContractNumber)
        {
            ViewBag.IDContractNumber = IDContractNumber;
            IDContract = IDContractNumber;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = null;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_order_details = (db.tbl_607_order_details.Include(t => t.tbl_607_order).Include(t => t.tbl_607_packaging)).Where(s=> s.FK_ID_order == IDContractNumber).OrderBy(t=>t.poste_number);

            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_order_details = tbl_607_order_details.Where(s => s.poste_number.ToString().Contains(searchString)).OrderBy(t => t.poste_number);
            }
            
            int pageNumber = (page ?? 1);
            var onePageOfPostes = tbl_607_order_details.ToPagedList(pageNumber, 10);

            ViewBag.OnePageOfPostes = onePageOfPostes;
            return View("_PartialPostesByContracts");
        }

        /// <summary>
        /// Contrôle de l'existance d'un poste sur le contrat selectionné pour empécher la duplication lors de la création
        /// </summary>
        /// <param name="posteNumber">numéro du poste sélectionné</param>
        /// <returns>message qui sera traité par le javascript</returns>
        public ActionResult PosteNumberControl(int posteNumber)
        {
            //Recherche du numéro de poste dans le contrat
            var posteExist = db.tbl_607_order_details.Where(t => t.FK_ID_order == IDContract & t.poste_number == posteNumber).Count();

            if (posteExist == 0)
            {
                //pas de doublon sur le numéro de poste
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            else
            {
                //on averti l'utilisateur qu'il y a deja un poste ayant ce numéro
                ViewBag.message = "Already exist";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
        }

        /// <summary>
        /// Affichage des détails du poste sélectionné
        /// </summary>
        /// <param name="id">id du poste sélectionné</param>
        /// <returns>vue détails</returns>
        // GET: Postes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_order_details tbl_607_order_details = db.tbl_607_order_details.Find(id);
            if (tbl_607_order_details == null)
            {
                return HttpNotFound();
            }
            return View("_DetailsPartialPostesByContracts", tbl_607_order_details);
        }

        /// <summary>
        /// Vue création d'un poste
        /// </summary>
        /// <returns>vue création</returns>
        // GET: Postes/Create
        public ActionResult Create()
        {
            ViewBag.FK_ID_order = new SelectList(db.tbl_607_order, "ID", "order_number");
            ViewBag.FK_ID_packaging = new SelectList(db.tbl_607_packaging, "ID", "packaging");
            return View("_CreatePartialPostesByContracts");
        }

        /// <summary>
        /// Binding des données pour création du poste
        /// </summary>
        /// <param name="tbl_607_order_details">data</param>
        /// <returns>vue principale</returns>
        // POST: Postes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,order_quantity,recieved_quantity,poste_number,shipping_delays,gaz_lifetime,FK_ID_packaging,stock_mini,content_comments")] tbl_607_order_details tbl_607_order_details)
        {

            if (ModelState.IsValid)
            {
                tbl_607_order_details.FK_ID_order = IDContract;
                db.tbl_607_order_details.Add(tbl_607_order_details);
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

            ViewBag.FK_ID_order = new SelectList(db.tbl_607_order, "ID", "order_number", tbl_607_order_details.FK_ID_order);
            ViewBag.FK_ID_packaging = new SelectList(db.tbl_607_packaging, "ID", "packaging", tbl_607_order_details.FK_ID_packaging);
            return View(tbl_607_order_details);
        }

        /// <summary>
        /// Contrôle de l'existance d'un poste sur le contrat selectionné pour empécher la duplication lors de l'édition
        /// </summary>
        /// <param name="posteNumber">numéro de poste</param>
        /// <returns>message qui sera traité par le javascript</returns>
        public ActionResult PosteEditNumberControl(int posteNumber)
        {
            //Recherche du numéro de poste dans le contrat
            var posteExist = db.tbl_607_order_details.Where(t => t.FK_ID_order == IDContract & t.poste_number == posteNumber).Count();

            if (posteExist == 0)
            {
                //pas de doublon sur le numéro de poste
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            else if (IDPoste.HasValue)
            {
                //Recherche du numéro de poste dans le contrat
                var posteExistList = db.tbl_607_order_details.Where(t => t.FK_ID_order == IDContract & t.poste_number == posteNumber).Select(t => t.ID).ToList();

                if (posteExistList.Contains(IDPoste.Value) & posteExist == 1)
                {
                    //pas de doublon sur le numéro de poste, seul celui en édition existe
                    ViewBag.message = "OK";
                    //generation de la vue partielle
                    return View("_PartialMessage");
                }
                //on averti l'utilisateur qu'il y a deja un poste ayant ce numéro
                ViewBag.message = "Already exist";
                //generation de la vue partielle
                return View("_PartialMessage");
            }

            return View("");
        }

        /// <summary>
        /// Vue d'édition d'un poste
        /// </summary>
        /// <param name="id">id du poste sélectionné</param>
        /// <returns>vue d'édition</returns>
        // GET: Postes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_order_details tbl_607_order_details = db.tbl_607_order_details.Find(id);
            if (tbl_607_order_details == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_ID_order = new SelectList(db.tbl_607_order, "ID", "order_number", tbl_607_order_details.FK_ID_order);
            ViewBag.FK_ID_packaging = new SelectList(db.tbl_607_packaging, "ID", "packaging", tbl_607_order_details.FK_ID_packaging);
            IDPoste = id;
            return View("_EditPartialPostesByContracts", tbl_607_order_details);
        }

        /// <summary>
        /// Binding des données modifiées sur le poste
        /// </summary>
        /// <param name="tbl_607_order_details">data</param>
        /// <returns>vue principale</returns>
        // POST: Postes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,order_quantity,recieved_quantity,poste_number,shipping_delays,gaz_lifetime,FK_ID_packaging,FK_ID_order,stock_mini,content_comments")] tbl_607_order_details tbl_607_order_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_607_order_details).State = EntityState.Modified;
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
            ViewBag.FK_ID_order = new SelectList(db.tbl_607_order, "ID", "order_number", tbl_607_order_details.FK_ID_order);
            ViewBag.FK_ID_packaging = new SelectList(db.tbl_607_packaging, "ID", "packaging", tbl_607_order_details.FK_ID_packaging);
            return View(tbl_607_order_details);
        }

        /// <summary>
        /// Vue suppression d'un poste
        /// </summary>
        /// <param name="id">id du poste sélectionné</param>
        /// <returns>vue suppression</returns>
        // GET: Postes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_order_details tbl_607_order_details = db.tbl_607_order_details.Find(id);
            if (tbl_607_order_details == null)
            {
                return HttpNotFound();
            }
            return View("_DeletePartialPostesByContracts", tbl_607_order_details);
        }

        /// <summary>
        /// Validation de la suppression
        /// </summary>
        /// <param name="id">id du poste sélectionné</param>
        /// <returns>vue principale</returns>
        // POST: Postes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_order_details tbl_607_order_details = db.tbl_607_order_details.Find(id);
            db.tbl_607_order_details.Remove(tbl_607_order_details);
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
