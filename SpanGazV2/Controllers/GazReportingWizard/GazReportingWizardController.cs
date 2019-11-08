using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MsgKit;
using MsgKit.Enums;
using SpanGazV2.Models;
using X.PagedList;
using System.Data.SqlClient;
using System.Transactions;

namespace SpanGazV2.Controllers.GazReportingWizard
{
    /// <summary>
    /// Controleur Wizard de controle des pressions de Gaz
    /// </summary>
    public class GazReportingWizardController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();
        static int? location;
        public static string newBottleN;

        /// <summary>
        /// Affichage du DropDownList des bancs de tests
        /// </summary>
        /// <returns>Vue principale</returns>
        // GET: GazReportingWizard
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Process">id dans la table location -> id= 1 BM ; id= 5 BR</param>
        /// <returns>affichage des vignettes bouteilles</returns>
        public ActionResult GetProcess(int? Process)
        {
            if (Process != null)
                {
                location = Process;

                //Récupération des données concernant toutes bouteilles de l'emplacement sélectionné
                var GazReportingWizardViewModel = (from gr in db.tbl_607_gaz_reporting
                                                   join a in db.tbl_607_actors on gr.FK_ID_actors equals a.id
                                                   join bo in db.tbl_607_bottle on gr.FK_ID_bottle equals bo.ID
                                                   join loc in db.tbl_607_location on bo.FK_ID_location equals loc.ID
                                                   join cc in db.tbl_607_conformity_certificate on bo.FK_ID_conformity_certificate equals cc.ID
                                                   join j in db.tbl_607_join_bottle_gaz on bo.ID equals j.FK_ID_bottle
                                                   join g in db.tbl_607_gaz on j.FK_ID_gaz equals g.ID
                                                   join od in db.tbl_607_order_details on g.FK_ID_order_details equals od.ID
                                                   where loc.ID == Process
                                                   select new GazReportingWizardViewModel
                                                   {
                                                       bottle_conti_number = bo.bottle_conti_number,
                                                       pressure_value = gr.pressure_value,
                                                       reporting_date = gr.reporting_date,
                                                       first_name = gr.tbl_607_actors.first_name,
                                                       last_name = gr.tbl_607_actors.last_name,
                                                       expiration_date = cc.expiration_date,
                                                       content = od.content_comments
                                                   }).ToList();
                //On grouoe les données et on les ordonne
                var res = from element in GazReportingWizardViewModel
                          group element by element.bottle_conti_number
                          into groups
                          select groups.OrderByDescending(p => p.reporting_date).First();



                switch (Process)
                {
                    //On retroune les données
                    case 1:
                        return View("_PartialBottleList", res);
                    //On retroune les données
                    case 5:
                        return View("_PartialBottleList", res);
                    //On ne fait rien
                    case 0:
                        return View();
                }
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        /// <summary>
        /// Controle de la nouvelle pression et de la date de peremption pour affichage dynamique
        /// </summary>
        /// <param name="reportvalue">valeur relevée à l'instant</param>
        /// <param name="continumber">Numéro Conti de la bouteille</param>
        /// <param name="lastpressure">Pression précédente</param>
        /// <param name="process">id dans la table location -> id= 1 BM ; id= 5 BR</param>
        /// <param name="expiration_date">date d'expiration</param>
        /// <returns>affichage dynamique: vert si OK, Orange si Warning, Rouge si NOK</returns>
        public ActionResult NewReport(int reportvalue, int continumber, int? lastpressure, int process, DateTime expiration_date)
        {
            tbl_607_gaz_reporting tbl_607_Gaz_Reporting = new tbl_607_gaz_reporting();
            string user = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            int idtech = db.tbl_607_actors.Where(t => t.id_uid == user).Select(t => t.id).FirstOrDefault();
            int idbottle = db.tbl_607_bottle.Where(t => t.bottle_conti_number == continumber.ToString() && t.FK_ID_location == process).Select(t => t.ID).FirstOrDefault();

            //la pression précédente est supérieure à la pression relevée et la bouteille n'est pas expirée
            //on retourne le message "invalid"
            if (lastpressure <= reportvalue & expiration_date > DateTime.Now)
             {
                ViewBag.message = "invalid";
                return View("_PartialMessageValidation");
            }
            //la pression est inférieure à la pression précédente et est supérieure à 50 bars, et la bouteille n'est pas expirée
            //on retourne le message "ok" 
            else if (lastpressure > reportvalue & reportvalue >50 & expiration_date > DateTime.Now)
            {
                tbl_607_Gaz_Reporting.FK_ID_actors = idtech;
                tbl_607_Gaz_Reporting.FK_ID_bottle = idbottle;
                tbl_607_Gaz_Reporting.pressure_value = reportvalue;
                tbl_607_Gaz_Reporting.reporting_date = DateTime.Now;
                db.tbl_607_gaz_reporting.Add(tbl_607_Gaz_Reporting);
                db.SaveChanges();
                ViewBag.message = "ok";
                return View("_PartialMessageValidation");

            }
            //la pression est inférieure à la pression précédente et est comprise entre 50 et 20 bars, et la bouteille n'est pas expirée
            //on retourne le message "warning"
            else if (lastpressure > reportvalue & reportvalue <= 50 && reportvalue > 20 & expiration_date > DateTime.Now)
            {
                tbl_607_Gaz_Reporting.FK_ID_actors = idtech;
                tbl_607_Gaz_Reporting.FK_ID_bottle = idbottle;
                tbl_607_Gaz_Reporting.pressure_value = reportvalue;
                tbl_607_Gaz_Reporting.reporting_date = DateTime.Now;
                db.tbl_607_gaz_reporting.Add(tbl_607_Gaz_Reporting);
                db.SaveChanges();
                ViewBag.message = "warning";
                return View("_PartialMessageValidation");

            }
            //la bouteille est expirée
            //on retourne le message "expired"
            else if (expiration_date <= DateTime.Now)
            {
                tbl_607_Gaz_Reporting.FK_ID_actors = idtech;
                tbl_607_Gaz_Reporting.FK_ID_bottle = idbottle;
                tbl_607_Gaz_Reporting.pressure_value = reportvalue;
                tbl_607_Gaz_Reporting.reporting_date = DateTime.Now;
                db.tbl_607_gaz_reporting.Add(tbl_607_Gaz_Reporting);
                db.SaveChanges();
                ViewBag.message = "expired";
                return View("_PartialMessageValidation");
            }
            //la pression est inférieure à la pression précédente et est inférieure à 20 bars, et la bouteille n'est pas expirée
            //on retourne le message "nok"
            else
            {
                tbl_607_Gaz_Reporting.FK_ID_actors = idtech;
                tbl_607_Gaz_Reporting.FK_ID_bottle = idbottle;
                tbl_607_Gaz_Reporting.pressure_value = reportvalue;
                tbl_607_Gaz_Reporting.reporting_date = DateTime.Now;
                db.tbl_607_gaz_reporting.Add(tbl_607_Gaz_Reporting);
                db.SaveChanges();
                ViewBag.message = "nok";
                return View("_PartialMessageValidation");
            }
    
        }

        /// <summary>
        /// recherche d'une bouteille de remplacement
        /// </summary>
        /// <param name="contiBottleNumber">Numéro Conti de la bouteille à remplacer</param>
        /// <returns>numéro de la bouteille ayant le même contenu et ayant la date de péremption la plus proche</returns>
        public ActionResult searchNewBottle(string contiBottleNumber)
        {
            try
            {
                //on récupère le numéro de poste de la bouteille
            var posteID = (from b in db.tbl_607_bottle
                           join l in db.tbl_607_location on b.FK_ID_location equals l.ID
                           join j in db.tbl_607_join_bottle_gaz on b.ID equals j.FK_ID_bottle
                           join g in db.tbl_607_gaz on j.FK_ID_gaz equals g.ID
                           join od in db.tbl_607_order_details on g.FK_ID_order_details equals od.ID
                           where b.bottle_conti_number.Contains(contiBottleNumber) & l.ID == location
                           select new { ID = od.ID }).FirstOrDefault();
                //on cherche une bouteille ayant le même contenu et ayant la date de péremption la plus proche
                var newBottleNumber = (from b in db.tbl_607_bottle
                                   join l in db.tbl_607_location on b.FK_ID_location equals l.ID
                                   join cc in db.tbl_607_conformity_certificate on b.FK_ID_conformity_certificate equals cc.ID
                                   join j in db.tbl_607_join_bottle_gaz on b.ID equals j.FK_ID_bottle
                                   join g in db.tbl_607_gaz on j.FK_ID_gaz equals g.ID
                                   join od in db.tbl_607_order_details on g.FK_ID_order_details equals od.ID
                                   where od.ID == posteID.ID & l.ID == 14 & cc.expiration_date >= DateTime.Now
                                   select new { BottleNumber = b.bottle_conti_number, expDate = cc.expiration_date }).OrderByDescending(p => p.expDate).FirstOrDefault();
                //on envoie vers la vue le numéro de la nouvelles bouteille et on le stocke dans une variable statique
            newBottleN = newBottleNumber.BottleNumber;
            ViewBag.message = newBottleNumber.BottleNumber;
            return View("_PartialMessageValidation");
            }
            catch
            {
                ViewBag.message = "Not found";
                return View("_PartialMessageValidation");
            }
        }

        /// <summary>
        /// Déstockage de la bouteille usagée et enregistrement de la mise en service de la nouvelle
        /// </summary>
        /// <param name="oldBottleNumber">numéro de la bouteille usagée</param>
        /// <returns>génération du mail informant de la dépose de l'ancienne bouteille</returns>
        public ActionResult saveNewBottle(string oldBottleNumber)
        {
            try
            {
                //sauvegarde du déstockage de la bouteille usagée
            var oldBottle = db.tbl_607_bottle.Where(t => t.bottle_conti_number == oldBottleNumber & t.FK_ID_location == location).FirstOrDefault();
            int destockVar = db.tbl_607_location.Where(t => t.bottle_location == "DESTOCK").Select(t => t.ID).First();
            oldBottle.FK_ID_location = destockVar;
            db.SaveChanges();
                //sauvegarde de la MES de la nouvelle bouteille
            int stockVar = db.tbl_607_location.Where(t => t.bottle_location == "STOCK").Select(t => t.ID).First();
            var newBottle = db.tbl_607_bottle.Where(t => t.bottle_conti_number == newBottleN & t.FK_ID_location == stockVar).FirstOrDefault();
            newBottle.FK_ID_location = location.Value;
            db.SaveChanges();

                //recupération des infos
                var infos = (from bo in db.tbl_607_bottle
                             join l in db.tbl_607_location on bo.FK_ID_location equals l.ID
                             join j in db.tbl_607_join_bottle_gaz on bo.ID equals j.FK_ID_bottle
                             join g in db.tbl_607_gaz on j.FK_ID_gaz equals g.ID
                             join od in db.tbl_607_order_details on g.FK_ID_order_details equals od.ID
                             join o in db.tbl_607_order on od.FK_ID_order equals o.ID
                             where bo.bottle_conti_number == oldBottleNumber && bo.FK_ID_location == destockVar
                             select new
                             {
                                 Poste = od.poste_number,
                                 ContractNumber = o.order_number,
                                 Content = od.content_comments
                             }).First();

                //Variables Mail
                string mailsender = (System.Web.HttpContext.Current.User.Identity.Name.ToString().Remove(0, 5)) + "@contiwan.com";
                //Numéro de poste
                ViewBag.Poste = infos.Poste;
                //Numéro de contrat
                //ViewBag.ContractNumber = infos.ContractNumber;
                //Content
                //ViewBag.Content = infos.Content;

                ViewBag.mailText = "mailto:mailto@xxx.com&cc=" + mailsender + "&subject=SpanGazV2 bottle return advice&body=---Test Center CONTINENTAL CPT--- %0A---Automatic mail generated by SpanGazV2--- %0ABottle return %0APoste N° " + infos.Poste + " %0AContract: " + infos.ContractNumber + " %0ASpan bottle %0AQuantity: 1     %0AContent: " + infos.Content;

                newBottleN = "";
                ViewBag.message = "Reload";
         
                return Json(new { message = ViewBag.message, mailText = ViewBag.mailText }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //generation de la vue affichage erreur
                return RedirectToAction("Index", "Ooops", new { message = ex });
            }
        }
        
    }
    
}
