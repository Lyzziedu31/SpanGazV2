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

namespace SpanGazV2.Controllers.BottleReceptionWizard
{
    /// <summary>
    /// Controlleur de gestion des processus de reception et destockage des bouteilles
    /// </summary>
    public class BottleReceptionWizardController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();
       
        //initialisation du dictionnaire pour le stockage des real gaz content et des idgaz
        public static Dictionary<int, float> Tbl_607_Real_Gaz_Content_temp = new Dictionary<int, float>();

        //initialisation du dictionnaire pour le stockage des gaz type et theorical content
        public static Dictionary<string, double> GazTypeAndTheoricalContent = new Dictionary<string, double>();

        /// <summary>
        /// affichage de la vue de démarrage du wizard logistique
        /// </summary>
        /// <returns>vue accueil du wizard</returns>
        public ActionResult Index()
        {
            //initialisation du model qui stockera les données temporaires
            //pas d'initialisation via new car our eviter qu'il se vide à chaque ActionResult il a été créé en static
            BottlereceptionWizardViewModelTemp.BL_ref = 0;
            BottlereceptionWizardViewModelTemp.bottle_conti_number = null;
            BottlereceptionWizardViewModelTemp.bottle_location = null;
            BottlereceptionWizardViewModelTemp.conformity_certificate = 0;
            BottlereceptionWizardViewModelTemp.expiration_date = new DateTime(1901, 01, 01);
            BottlereceptionWizardViewModelTemp.fabrication_date = new DateTime(1901, 01, 01);
            BottlereceptionWizardViewModelTemp.FK_ID_shipping_request = 0;
            BottlereceptionWizardViewModelTemp.gaz_id = 0;
            BottlereceptionWizardViewModelTemp.idcontract = 0;
            BottlereceptionWizardViewModelTemp.posteNumber = 0;
            BottlereceptionWizardViewModelTemp.manufacturer_bottle_number = null;
            BottlereceptionWizardViewModelTemp.real_content = 0;
            BottlereceptionWizardViewModelTemp.reception_date = new DateTime(1901, 01, 01);
            BottlereceptionWizardViewModelTemp.reception_quantity = 0;
            BottlereceptionWizardViewModelTemp.recieved_quantity = 0;
            BottlereceptionWizardViewModelTemp.content = "";

            //si le wizard est interrompu on vide le dictionnaire
            Tbl_607_Real_Gaz_Content_temp.Clear();

            //generation de la vue
            return View();
        }

        /// <summary>
        /// choix du process à démarrer
        /// </summary>
        /// <param name="Process">process selectionné dans le front</param>
        /// <returns>vue correspondant au process</returns>
        public ActionResult GetProcess(int? Process)
        {
            //chargement des contrats et des providers
            var tbl_607_order = db.tbl_607_order.Include(t => t.tbl_607_provider);

            //enum pour le choix du process
            switch (Process)
            {
                //choix reception d'une bouteille
                //charge la liste des contrats éligibles
                case 1:
                    tbl_607_order = tbl_607_order.Where(t => t.item_reception_active == true).OrderBy(t => t.tbl_607_provider.provider_name);
                    return View("_PartialIncomingContractSelect", tbl_607_order.ToList());

                //choix destockage d'une bouteille
                //affichage géré par le javascript à cette étape
                case 2:
                    //generation de la vue
                    return View();

                //aucun choix
                case 0:
                    //generation de la vue
                    return View();
            }
            return HttpNotFound();
        }

        /// <summary>
        /// affichage de la liste des postes
        /// </summary>
        /// <param name="searchString">filtre par valeur recherchée</param>
        /// <param name="page">page selectionnée dans l'affichage</param>
        /// <param name="Contract">id du contrat selectionné</param>
        /// <returns>liste des postes</returns>
        public ActionResult GetPoste(string searchString, int? page, int? Contract)
        {
            //barre de recherche
            ViewBag.Contract = Contract;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = null;
            }

            ViewBag.CurrentFilter = searchString;

            //requete initiale pour l'affichage des postes
            //ne sont affiches que les postes contenant des gaz
            var tbl_607_order_details = (db.tbl_607_order_details.Include(t => t.tbl_607_order).Include(t => t.tbl_607_packaging).Include(t =>t.tbl_607_shipping_request_details)).Where(s => s.FK_ID_order == Contract && s.tbl_607_gaz.Count() > 0).OrderBy(t => t.poste_number);

            if (!String.IsNullOrEmpty(searchString))
            {
                //recherche
                tbl_607_order_details = tbl_607_order_details.Include(t => t.tbl_607_order).Where(s => s.poste_number.ToString().Contains(searchString) && s.tbl_607_gaz.Count() > 0).OrderBy(t => t.poste_number);
            }

            //pagedlist
            //affichage de la page 1 par defaut
            int pageNumber = (page ?? 1);
            //affichage de 10 resultats par liste
            var onePageOfPostes = tbl_607_order_details.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfPostes = onePageOfPostes;

            //generation de la vue
            return View("_PartialIncomingPosteSelect");
        }

        /// <summary>
        /// affiche la première page d'entrée de données
        /// </summary>
        /// <param name="id">id du poste selectionné</param>
        /// <param name="Contract">id du contrat</param>
        /// <param name="poste">numéro du poste</param>
        /// <returns></returns>
        public ActionResult SelectPosteWizard(int? id, int Contract, int poste)
        {
            //on stocke l'id poste dans le model
            BottlereceptionWizardViewModelTemp.posteNumber = poste;
            //on stocke le content du poste dans le model
            BottlereceptionWizardViewModelTemp.content = db.tbl_607_order_details.Where(t => t.ID == id).Select(t => t.content_comments).FirstOrDefault();

            //recuperation de la liste des certificats
            var certif = db.tbl_607_conformity_certificate.Select(t => t.ID).ToList();
            //stockage de la liste des certificats pour la vue
            ViewBag.certif = certif;
            //recuperation de la liste des teneurs theoriques pour chaque gaz du poste
            var teneurTheo = db.tbl_607_gaz.Include(t => t.tbl_607_theorical_content.theorical_content).Where(t => t.FK_ID_order_details == id).Select(t =>t.tbl_607_theorical_content.theorical_content).ToList();
            //stockage de la liste des teneurs theoriques pour la vue
            ViewBag.teneur = teneurTheo;
            //recuperation de la liste des tolerances de fabrication pour chaque gaz du poste
            var madeTolerance = db.tbl_607_gaz.Include(t => t.tbl_607_made_tolerance).Where(t => t.FK_ID_order_details == id).Select(t=>t.tbl_607_made_tolerance.made_tolerance).ToList();
            //stockage des donnees pour la vue
            ViewBag.madeTolerance = madeTolerance;
            ViewBag.contract = Contract;
            ViewBag.idposte = id;
            ViewBag.poste = poste;
            ViewBag.shipping_request_ref = new SelectList(db.tbl_607_shipping_request_details.Include(t => t.tbl_607_shipping_request).Where(t=>t.FK_ID_order_details == id).Select(t => new { t.ID, t.tbl_607_shipping_request.shipping_request_ref }).ToList(),"Id", "shipping_request_ref");
            ViewBag.Content = BottlereceptionWizardViewModelTemp.content;
            //generation de la vue
            return View("_PartialIncomingEnterData");
        }

        /// <summary>
        /// contrôle de la valeur du champs BL ref
        /// </summary>
        /// <param name="BLRef"></param>
        /// <param name="Contract"></param>
        /// <returns>résultat du contrôle:ViewBag.message avec la valeur "OK" ou avec la valeur "At least one reception on this" si NOK</returns>
        public ActionResult BLControl(int BLRef, int Contract)
        {
            //chargement de la liste des BL existants
            var BLBase = (db.tbl_607_shipping_delivery.Include(t => t.tbl_607_shipping_request).Include(s => s.tbl_607_shipping_request.tbl_607_order).Include(o => o.tbl_607_shipping_request.tbl_607_order.tbl_607_order_details)).Where(o => o.tbl_607_shipping_request.tbl_607_order.ID == Contract).Select(t => t.BL_ref).ToList();
            //on contrôle si le BL entré existe deja
            var Control = BLBase.Contains(BLRef);
            if (Control)
            {
                //le BL existe deja, on peut recevoir plusieurs bouteilles
                BottlereceptionWizardViewModelTemp.BL_ref = BLRef;
                //on averti l'utilisateur qu'il y a deja eu au moins une reception sur ce BL
                ViewBag.message = "At least one reception on this";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            else
            {
                //le BL n'existe pas
                BottlereceptionWizardViewModelTemp.BL_ref = BLRef;
                ViewBag.blref = BLRef;
                //affichage de la pastille OK
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
        }

        /// <summary>
        /// contrôle de la valeur Reception Date
        /// </summary>
        /// <param name="BLDate"></param>
        /// <returns>résultat du contrôle:ViewBag.message avec la valeur "OK" ou avec la valeur "Must be between now and -14 days" si NOK</returns>
        public ActionResult BLDateControl(string BLDate)
        {   
            //parse de la date du BL
            var dateToCompare = DateTime.Parse(BLDate);

            //si la date de reception est comprise entre maintenant et -7 jours
            if (DateTime.Now.AddDays(-14) < dateToCompare && dateToCompare < DateTime.Now)
            {
                //OK date valide
                BottlereceptionWizardViewModelTemp.reception_date = dateToCompare;
                ViewBag.BLDate = BLDate;
                //affichage de la pastille OK
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            //NOK date invalide
            //affichage du message d'erreur
            ViewBag.message = "Must be between now and -14 days";
            //generation de la vue partielle
            return View("_PartialMessage");
        }

        /// <summary>
        /// contrôle de la valeur Certificate Number
        /// </summary>
        /// <param name="Certif"></param>
        /// <returns>résultat du contrôle:ViewBag.message avec la valeur "OK" ou avec la valeur "Already in base" si NOK</returns>
        public ActionResult CertifControl(int Certif)
        {
            //si la reference du certificat de conformite existe deja, un certificat par bouteille
            var certifBase = db.tbl_607_conformity_certificate.Select(t => t.ID);
            var control = certifBase.Contains(Certif);
            if(control)
            {
                //NOK deja present
                //affichage du message d'erreur
                ViewBag.message = "Already in base";
                //generation de la vue
                return View("_PartialMessage");
            }
            //OK nouvelle reference
            BottlereceptionWizardViewModelTemp.conformity_certificate = Certif;
            ViewBag.Certif = Certif;
            //affichage de la pastille OK
            ViewBag.message = "OK";
            //generation de la vue
            return View("_PartialMessage");
        }

        /// <summary>
        /// contrôle de la valeur Fabrication date
        /// </summary>
        /// <param name="Fabdate"></param>
        /// <returns>résultat du contrôle:ViewBag.message avec la valeur "OK" ou avec la valeur "Can't be in future" si NOK</returns>
        public ActionResult FabControl(string Fabdate)
        {
            //parse de la date de fabrication
            var dateToCompare = DateTime.Parse(Fabdate);

            //si la date est inferieure a la date du jour, une bouteille ne peux pas avoir ete fabriquee dans le futur
            if (dateToCompare < DateTime.Now)
            {
                //OK
                BottlereceptionWizardViewModelTemp.fabrication_date = dateToCompare;
                ViewBag.Fabdate = Fabdate;
                //affichage de la pastille OK
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            //NOK
            //affichage du message d'erreur
            ViewBag.message = "Can't be in future";
            //generation de la vue partielle
            return View("_PartialMessage");
        }


        /// <summary>
        /// Contrôle la valeur Expiration date
        /// </summary>
        /// <param name="Expdate"></param>
        /// <returns>résultat du contrôle:ViewBag.message avec la valeur "OK" ou avec la valeur "Can't be past date" si NOK</returns>
        public ActionResult ExpControl(string Expdate)
        {
            //parse de la date de fabrication
            var dateToCompare = DateTime.Parse(Expdate);

            //si la date est superieure a la date du jour, on refuse une bouteille qui a deja expire
            if (dateToCompare > DateTime.Now)
            {
                //OK
                BottlereceptionWizardViewModelTemp.expiration_date = dateToCompare;
                ViewBag.Expdate = Expdate;
                //affichage de la pastille OK
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            //NOK
            //affichage du message d'erreur
            ViewBag.message = "Can't be past date";
            //generation de la vue partielle
            return View("_PartialMessage");
        }

        /// <summary>
        /// affichage de la liste des gaz composant le poste
        /// </summary>
        /// <param name="idcontract"></param>
        /// <param name="idShippingRequest"></param>
        /// <param name="idposte"></param>
        /// <param name="poste"></param>
        /// <param name="Blref"></param>
        /// <param name="BLDate"></param>
        /// <param name="Certif"></param>
        /// <param name="fabdate"></param>
        /// <param name="expdate"></param>
        /// <returns>retourne la vue</returns>
        public ActionResult SelectGazWizard(int idcontract, int idShippingRequest, int idposte, int poste, int Blref, string BLDate, int Certif, string fabdate, string expdate)
        {
            //chargement de la liste des gaz du poste
            var tbl_607_gaz = (db.tbl_607_gaz.Include(t => t.tbl_607_gaz_type).Include(t => t.tbl_607_made_tolerance).Include(t => t.tbl_607_testing_tolerance).Include(t => t.tbl_607_theorical_content).Include(t => t.tbl_607_units).Include(t => t.tbl_607_units1).Include(t => t.tbl_607_units2)).Where(s => s.FK_ID_order_details == idposte);

            //stockage d'une partie des donnee dans la vue pour l'affichage suivant
            var contractnumber = db.tbl_607_order.Where(t => t.ID == idcontract).Select(t => t.order_number).ToList();
            var provider = db.tbl_607_order.Include(t => t.tbl_607_provider).Where(t => t.ID == idcontract).Select(t => t.tbl_607_provider.provider_name).ToList();
            ViewBag.IDContract = idcontract;
            ViewBag.IDPoste = idposte;
            ViewBag.ContractNumber = contractnumber.FirstOrDefault();
            ViewBag.Provider = provider.FirstOrDefault();
            ViewBag.Poste = poste;
            ViewBag.BLref = Blref;
            ViewBag.BLDate = BLDate;
            ViewBag.Certif = Certif;
            ViewBag.Fabdate = fabdate;
            ViewBag.Expdate = expdate;
            ViewBag.idShippingRequest = idShippingRequest;
            ViewBag.Content = BottlereceptionWizardViewModelTemp.content;
            //mail utilisateur
            string mailsender = System.Web.HttpContext.Current.User.Identity.Name.ToString().Remove(0, 5);
            ViewBag.mailsender = mailsender + "@contiwan.com";
            //generation de la vue partielle
            return View("_PartialGaz", tbl_607_gaz.ToList());
        }

        /// <summary>
        /// Contrôle de la valeur certificate value pour le gaz selectionné
        /// </summary>
        /// <param name="certifvalue"></param>
        /// <param name="normalvalue"></param>
        /// <param name="tol">tolerance</param>
        /// <param name="idgaz"></param>
        /// <returns>résultat du contrôle:ViewBag.message avec la valeur "OK" si la valeur est dans la marge de tolerance ou avec la valeur "Out of range" si NOK</returns>
        public ActionResult Controlgaz(float certifvalue, float normalvalue, float tol, int idgaz)
        {
            //on calcule les bornes hautes et basses des teneurs reelles conformes 
            double valuemax = normalvalue + ((normalvalue * tol)/100);
            double valuemin = normalvalue - ((normalvalue * tol)/ 100);

            //si la teneur reelle notee sur le certificat est entre les bornes
            if ((valuemin <= certifvalue) && (certifvalue <= valuemax))
            {
                //OK
                //on stocke les donnees dans le model
                BottlereceptionWizardViewModelTemp.gaz_id = idgaz;
                BottlereceptionWizardViewModelTemp.real_content = certifvalue;

                //on cree un enregistrement dans le dictionnaire
                Tbl_607_Real_Gaz_Content_temp.Add(idgaz, certifvalue);

                //affichage de la pastille OK
                ViewBag.message = "OK";
                //generation de la vue partielle
                return View("_PartialMessage");
            }
            //NOK
            //affichage du message d'erreur
            ViewBag.message = "Out of range";
            //generation de la vue partielle
            return View("_PartialMessage");
        }

        /// <summary>
        /// Affichage de la page d'entrée des données suivantes et récapitulatif des données compilées precedemment dans le wizard
        /// </summary>
        /// <param name="ManNumber">Reference fabriquant de la bouteille</param>
        /// <param name="idShippingRequest"></param>
        /// <param name="ContiNumber">Reference Continentale de la bouteille</param>
        /// <param name="idcontract"></param>
        /// <param name="contractnumber"></param>
        /// <param name="provider"></param>
        /// <param name="poste"></param>
        /// <returns>retroune la vue</returns>
        public ActionResult Recapdata(string ManNumber, int idShippingRequest, string ContiNumber, string idcontract, string contractnumber, string provider, string poste)
        {
            //on termine le stockage des donnee dans la vue pour l'affichage recap et on les stocke toutes dans le model car a ce stade elles sont toutes conformes
            var FK_ID_shipping_request = db.tbl_607_shipping_request_details.Where(t => t.ID == idShippingRequest).Select(t => t.FK_shipping_request).FirstOrDefault();
            BottlereceptionWizardViewModelTemp.FK_ID_shipping_request = FK_ID_shipping_request;
            BottlereceptionWizardViewModelTemp.manufacturer_bottle_number = ManNumber;
            BottlereceptionWizardViewModelTemp.bottle_conti_number = ContiNumber;
            BottlereceptionWizardViewModelTemp.bottle_location = "STOCK";
            ViewBag.ManNumber = BottlereceptionWizardViewModelTemp.manufacturer_bottle_number;
            ViewBag.ContiNumber = BottlereceptionWizardViewModelTemp.bottle_conti_number;
            ViewBag.IDContract = idcontract;
            BottlereceptionWizardViewModelTemp.idcontract = Convert.ToInt32(idcontract);
            ViewBag.IDPoste = BottlereceptionWizardViewModelTemp.posteNumber;
            ViewBag.Poste = poste;
            ViewBag.BLref = BottlereceptionWizardViewModelTemp.BL_ref;
            ViewBag.BLDate = BottlereceptionWizardViewModelTemp.reception_date.Date.ToString("MM/dd/yyyy");
            ViewBag.Certif = BottlereceptionWizardViewModelTemp.conformity_certificate;
            ViewBag.Fabdate = BottlereceptionWizardViewModelTemp.fabrication_date.Date.ToString("MM/dd/yyyy");
            ViewBag.Expdate = BottlereceptionWizardViewModelTemp.expiration_date.Date.ToString("MM/dd/yyyy");
            ViewBag.contractnumber = contractnumber;
            ViewBag.provider = provider;
            ViewBag.Content = BottlereceptionWizardViewModelTemp.content;
            
            //generation de la vue partielle
            return View("_PartialDataValidation");
        }

        /// <summary>
        /// sauvegarde des données
        /// </summary>
        /// <returns>vue principale</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveData()
        {
            try
            {
                // vérification de l'existance du BL
                // extraction des numeros de BL existants sur le contrat
                var BLBase = (db.tbl_607_shipping_delivery.Include(t => t.tbl_607_shipping_request).Include(s => s.tbl_607_shipping_request.tbl_607_order)
                    .Include(o => o.tbl_607_shipping_request.tbl_607_order.tbl_607_order_details))
                    .Where(o => o.tbl_607_shipping_request.tbl_607_order.ID == BottlereceptionWizardViewModelTemp.idcontract).Select(t => t.BL_ref).ToList();


                tbl_607_shipping_delivery tbl_607_Shipping_Delivery = new tbl_607_shipping_delivery();
                // comparaison avec le numero saisi par l'utilisateur
                var Control = BLBase.Contains(BottlereceptionWizardViewModelTemp.BL_ref);
                if (Control)
                {
                    //BL existant, nous allons donc uniquement proceder à un update 

                }
                else
                {
                    //BL inexistant, creation
                    tbl_607_Shipping_Delivery.BL_ref = BottlereceptionWizardViewModelTemp.BL_ref;
                    tbl_607_Shipping_Delivery.reception_date = BottlereceptionWizardViewModelTemp.reception_date;
                    tbl_607_Shipping_Delivery.FK_ID_shipping_request = BottlereceptionWizardViewModelTemp.FK_ID_shipping_request;
                    db.tbl_607_shipping_delivery.Add(tbl_607_Shipping_Delivery);
                    db.SaveChanges(); //sauvegarde
                }

                // le certificat de conformite est unique et est lie a une bouteille
                tbl_607_conformity_certificate tbl_607_Conformity_Certificate = new tbl_607_conformity_certificate();
                tbl_607_Conformity_Certificate.ID = BottlereceptionWizardViewModelTemp.conformity_certificate;
                tbl_607_Conformity_Certificate.fabrication_date = BottlereceptionWizardViewModelTemp.fabrication_date;
                tbl_607_Conformity_Certificate.expiration_date = BottlereceptionWizardViewModelTemp.expiration_date;
                db.tbl_607_conformity_certificate.Add(tbl_607_Conformity_Certificate);
                db.SaveChanges(); //sauvegarde

                // recuperation des id des enregistrements crees
                var FK_ID_shipping_delivery = db.tbl_607_shipping_delivery.Select(t => t.ID).Max();

                // recuperation de l'id location pour stock
                var stock = db.tbl_607_location.Where(t => t.bottle_location == "STOCK").Select(t => t.ID).First();

                // enregistrement dans la table bouteille
                tbl_607_bottle tbl_607_Bottle = new tbl_607_bottle();
                tbl_607_Bottle.manufacturer_bottle_number = BottlereceptionWizardViewModelTemp.manufacturer_bottle_number;
                tbl_607_Bottle.bottle_conti_number = BottlereceptionWizardViewModelTemp.bottle_conti_number;
                tbl_607_Bottle.FK_ID_conformity_certificate = BottlereceptionWizardViewModelTemp.conformity_certificate;
                tbl_607_Bottle.FK_ID_shipping_delivery = FK_ID_shipping_delivery;
                tbl_607_Bottle.FK_ID_order = BottlereceptionWizardViewModelTemp.idcontract;
                tbl_607_Bottle.FK_ID_location = stock;
                db.tbl_607_bottle.Add(tbl_607_Bottle);
                db.SaveChanges(); //sauvegarde

                // mise à jour de la quantité recue sur le poste
                tbl_607_order_details tbl_607_Order_Detail = db.tbl_607_order_details.Where(t => t.poste_number == BottlereceptionWizardViewModelTemp.posteNumber && t.FK_ID_order == BottlereceptionWizardViewModelTemp.idcontract).First();
                tbl_607_Order_Detail.recieved_quantity = tbl_607_Order_Detail.recieved_quantity ++;
                db.SaveChanges(); //sauvegarde

                // mise a jour de la quantité sur la demande de livraison
                tbl_607_shipping_request_details tbl_607_Shipping_Request_Details = db.tbl_607_shipping_request_details.Where(t => t.FK_shipping_request == BottlereceptionWizardViewModelTemp.FK_ID_shipping_request).FirstOrDefault();
                tbl_607_Shipping_Request_Details.reception_quantity = tbl_607_Shipping_Request_Details.reception_quantity ++;
                db.SaveChanges(); //sauvegarde

                // Recuperation de l'ID Bottle. L'ID gaz est deja connu = key du dico
                var idBottle = db.tbl_607_bottle.Select(t => t.ID).Max();

                //Nouvelle relève: valeurs par défaut 999bars et acteur: admin
                tbl_607_gaz_reporting tbl_607_Gaz_Reporting = new tbl_607_gaz_reporting();
                tbl_607_Gaz_Reporting.pressure_value = 999;
                tbl_607_Gaz_Reporting.FK_ID_actors = 29;
                tbl_607_Gaz_Reporting.FK_ID_bottle = idBottle;
                tbl_607_Gaz_Reporting.reporting_date = DateTime.Now;
                db.tbl_607_gaz_reporting.Add(tbl_607_Gaz_Reporting);

                // Recuperation des reals gaz dans le dictionnaire
                foreach (KeyValuePair<int, float> value in Tbl_607_Real_Gaz_Content_temp)
                {
                    tbl_607_real_gaz_content tbl_607_Real_Gaz_Content = new tbl_607_real_gaz_content();
                    tbl_607_join_bottle_gaz tbl_607_Join_Bottle_Gaz = new tbl_607_join_bottle_gaz();
                    // Enregistrement des reals gaz
                    tbl_607_Real_Gaz_Content.real_content = value.Value;//force l'ecriture dans la base alors que nous sommes encore dans la boucle
                    db.tbl_607_real_gaz_content.Add(tbl_607_Real_Gaz_Content);
                    db.SaveChanges(); //sauvegarde

                    //// Recuperation de l'id real gaz
                    var fk_real = db.tbl_607_real_gaz_content.Select(t => t.ID).Max();

                    //Ecriture de la jointure
                    tbl_607_Join_Bottle_Gaz.FK_ID_gaz = value.Key;
                    tbl_607_Join_Bottle_Gaz.FK_ID_bottle = idBottle;
                    tbl_607_Join_Bottle_Gaz.FK_ID_real_content = fk_real;
                    db.tbl_607_join_bottle_gaz.Add(tbl_607_Join_Bottle_Gaz); //force l'ecriture dans la base alors que nous sommes encore dans la boucle
                    db.SaveChanges(); //sauvegarde          

                }

                //generation de la vue
                return View("Index");
            }
            catch (Exception ex)
            {
                //generation de la vue affichage erreur
                return RedirectToAction("Index", "Ooops", new { message = ex });
            }
        }

        /// <summary>
        /// generation du mail de reception
        /// </summary>
        /// <returns>rien: l'envoi se fait par la fonction en jquery mais on doit obligatoirement avoir une fonction correspondante dans le back</returns>
        public EmptyResult SendMail()
        {
            //methode vide pour envoi du mail
            //l'envoi se fait par la fonction en jquery mais on doit obligatoirement avoir une fonction correspondante dans le back
            return new EmptyResult();
        }

        /// <summary>
        /// recherche de la ref bouteille 
        /// </summary>
        /// <param name="bottleNumber"></param>
        /// <returns> rien si la bouteille existe et en état stock, "Unknow" si elle est introuvable</returns>
        public ActionResult RemoveBottleInfo(string bottleNumber)
        {
            GazTypeAndTheoricalContent.Clear();

            try
            {
           
            var DestockBottleInfos = (from bo in db.tbl_607_bottle
                                      join l in db.tbl_607_location on bo.FK_ID_location equals l.ID
                                      join cc in db.tbl_607_conformity_certificate on bo.FK_ID_conformity_certificate equals cc.ID
                                      join sd in db.tbl_607_shipping_delivery on bo.FK_ID_shipping_delivery equals sd.ID
                                      join j in db.tbl_607_join_bottle_gaz on bo.ID equals j.FK_ID_bottle
                                      join g in db.tbl_607_gaz on j.FK_ID_gaz equals g.ID
                                      join gt in db.tbl_607_gaz_type on g.FK_ID_gaz_type equals gt.ID
                                      join tc in db.tbl_607_theorical_content on g.FK_ID_theorical_content equals tc.ID
                                      join od in db.tbl_607_order_details on g.FK_ID_order_details equals od.ID
                                      join o in db.tbl_607_order on od.FK_ID_order equals o.ID
                                      where l.bottle_location == "STOCK" && bo.bottle_conti_number == bottleNumber 
                                      select new DestockBottleInfos
                                                    {
                                                    contiNumber = bo.bottle_conti_number,
                                                    manufacturerNumber = bo.manufacturer_bottle_number,
                                                    bottleLocation = l.bottle_location,
                                                    fabricationDate = cc.fabrication_date,
                                                    expirationDate = cc.expiration_date,
                                                    receptionDate = sd.reception_date,
                                                    posteNumber = od.poste_number,
                                                    contractNumber = o.order_number,
                                                    gazType = gt.gaz_type,
                                                    theoricalContent = tc.theorical_content
                                                    }).ToList();

                if (DestockBottleInfos.Count() != 0)
                    {
                    foreach (var gaz in DestockBottleInfos)
                    {
                        GazTypeAndTheoricalContent.Add(gaz.gazType, gaz.theoricalContent);
                        ViewBag.MyDictionary = GazTypeAndTheoricalContent;
                    }

                    return View("_PartialDestockRecapData", DestockBottleInfos.FirstOrDefault());

                    }
                else
                {
                    ViewBag.message = "Unknow";
                    return View("_PartialMessage");
                }
            }
           
            catch (Exception ex)
            {
                return HttpNotFound();
            }

        }

        /// <summary>
        /// Validation du destockage d'une bouteille
        /// </summary>
        /// <param name="contiBottleNumber"></param>
        /// <returns>génération du mail</returns>
        public ActionResult ValidDestock(string contiBottleNumber)
        {
            try
            {
                var idDestock = db.tbl_607_location.Where(t => t.bottle_location == "DESTOCK").Select(t => t.ID).FirstOrDefault();
                var idStock = db.tbl_607_location.Where(t => t.bottle_location == "STOCK").Select(t => t.ID).FirstOrDefault();
                tbl_607_bottle tbl_607_Bottle = db.tbl_607_bottle.Include(t => t.tbl_607_location).Where(t => t.bottle_conti_number == contiBottleNumber && t.FK_ID_location == idStock).First();
                tbl_607_Bottle.FK_ID_location = idDestock;
                db.SaveChanges();

                //recupération du numéro de poste
                var infos = (from bo in db.tbl_607_bottle
                             join l in db.tbl_607_location on bo.FK_ID_location equals l.ID
                             join j in db.tbl_607_join_bottle_gaz on bo.ID equals j.FK_ID_bottle
                             join g in db.tbl_607_gaz on j.FK_ID_gaz equals g.ID
                             join od in db.tbl_607_order_details on g.FK_ID_order_details equals od.ID
                             join o in db.tbl_607_order on od.FK_ID_order equals o.ID
                             where bo.bottle_conti_number == contiBottleNumber && bo.FK_ID_location == idStock
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
                ViewBag.ContractNumber = infos.ContractNumber;
                //Content
                ViewBag.Content = infos.Content;

                ViewBag.mailText = "mailto:mailto@xxx.com&cc=" + mailsender + "&subject=SpanGazV2 bottle return advice&body=---Test Center CONTINENTAL CPT--- %0A---Automatic mail generated by SpanGazV2--- %0ABottle return %0APoste N° " + infos.Poste + " %0AContract: " + infos.ContractNumber + " %0ASpan bottle %0AQuantity: 1     %0AContent: " + infos.Content;

                ViewBag.message = "Reload";

                return Json(new { view = "Index", mailText = ViewBag.mailText }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return HttpNotFound();
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
