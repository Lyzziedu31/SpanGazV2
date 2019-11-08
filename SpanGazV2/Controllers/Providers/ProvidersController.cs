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

namespace SpanGazV2.Controllers.Providers
{
    /// <summary>
    /// Controleur d'administration des fournisseurs
    /// </summary>
    public class ProvidersController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        /// <summary>
        /// retourne la liste de tous les fournisseurs enregistrés dans la base
        /// </summary>
        /// <param name="sortOrder">retour du filtre selectionné dans le front (X.PagedList)</param>
        /// <param name="currentFilter">retour du filtre courant dans le front (X.PagedList)</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <returns>liste de tous les fournisseurs filtrés en fonction des paramètres précédents</returns>
        // GET: Providers
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Provider_NameSortParm = String.IsNullOrEmpty(sortOrder)  ? "provider_name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tbl_607_Provider = from s in db.tbl_607_provider
                                       select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                tbl_607_Provider = tbl_607_Provider.Where(s => s.provider_name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "provider_name":
                    tbl_607_Provider = tbl_607_Provider.OrderBy(s => s.provider_name);
                    break;
                case "provider_name_desc":
                    tbl_607_Provider = tbl_607_Provider.OrderByDescending(s => s.provider_name);
                    break;
                default:
                    tbl_607_Provider = tbl_607_Provider.OrderBy(s => s.provider_name);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tbl_607_Provider.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Affichage de la page Détails du fournisseur séléctionné
        /// </summary>
        /// <param name="id">id du fournisseur selectionné</param>
        /// <returns>retourne les données correspondantes</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_provider tbl_607_provider = db.tbl_607_provider.Find(id);
            if (tbl_607_provider == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_provider);
        }

        /// <summary>
        /// affiche la page de création d'un nouveau fournisseur
        /// </summary>
        /// <returns>retourne la vue</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Binding des données du nouveau fournisseur
        /// </summary>
        /// <param name="tbl_607_provider">data</param>
        /// <returns>vue principale</returns>
        // POST: Providers/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,provider_name,adress,cp,town,phone,fax,mail,adress_compl")] tbl_607_provider tbl_607_provider)
        {
            if (ModelState.IsValid)
            {
                db.tbl_607_provider.Add(tbl_607_provider);
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

            return View(tbl_607_provider);
        }

        /// <summary>
        /// affiche la page d'édition d'un fournisseur
        /// </summary>
        /// <param name="id">id du fournisseur selectionné</param>
        /// <returns>vue edition</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_607_provider tbl_607_provider = db.tbl_607_provider.Find(id);
            if (tbl_607_provider == null)
            {
                return HttpNotFound();
            }
            return View(tbl_607_provider);
        }

        /// <summary>
        /// validation des modification sur le fournisseur
        /// </summary>
        /// <param name="tbl_607_provider">data</param>
        /// <returns>vue principale</returns>
        // POST: Providers/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,provider_name,adress,cp,town,phone,fax,mail,adress_compl")] tbl_607_provider tbl_607_provider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_607_provider).State = EntityState.Modified;
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
            return View(tbl_607_provider);
        }

        /// <summary>
        /// Suppression d'un fournisseur
        /// </summary>
        /// <param name="id">id du fournisseur sélectionné</param>
        /// <returns>vue suppression</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //nombre d'occurences provider/contract
                int counter = db.tbl_607_order.Include(t => t.FK_ID_provider).Where(t => t.FK_ID_provider == id).Count();

                if (counter == 0)
                {
                    tbl_607_provider tbl_607_provider = db.tbl_607_provider.Find(id);
                    if (tbl_607_provider == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tbl_607_provider);
                }
                else
                {
                    //on averti l'utilisateur qu'il y a deja un poste ayant ce numéro
                    ViewBag.message = "Forbidden";
                    //generation de la vue partielle
                    return View("_PartialMessage");
                }
            }
            
        }

        /// <summary>
        /// confirmation de la suppression
        /// </summary>
        /// <param name="id">>id du fournisseurr à supprimer</param>
        /// <returns>vue principale</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_607_provider tbl_607_provider = db.tbl_607_provider.Find(id);
            db.tbl_607_provider.Remove(tbl_607_provider);
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
