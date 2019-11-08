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
using System.Data.SqlClient;
using System.Transactions;
using X.PagedList;
using System.Xml;
using System.IO.Packaging;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Globalization;

namespace SpanGazV2.Controllers.OrderWizard
{
    /// <summary>
    /// Controleur du wizard de commande
    /// </summary>
    public class OrderWizardController : Controller
    {
        private database_tc2Entities db = new database_tc2Entities();

        //Données statiques
        /// <summary>
        /// Dictionnaire pour l'enregistrement du panier
        /// </summary>
        public static Dictionary<int, int> partialSave = new Dictionary<int, int>();
        /// <summary>
        /// Numéro du contrat en cours
        /// </summary>
        public static string contractNumber;
        /// <summary>
        /// Référence unique de la commande
        /// </summary>
        public static string sr_ref;
        /// <summary>
        /// Chemin et nom du fichier pour la génération du bon de commande
        /// </summary>
        public static string fileName;

        /// <summary>
        /// Affichage d'une DropDownList pour le choix du contrat
        /// </summary>
        /// <returns>liste des contrats</returns>
        // GET: OrderWizard
        public ActionResult Index()
        {

            //load contract list
            var contractList = (from od in db.tbl_607_order_details
                                join o in db.tbl_607_order on od.FK_ID_order equals o.ID
                                join p in db.tbl_607_provider on o.FK_ID_provider equals p.ID
                                where o.shipping_request_active == true
                                select new ContractListOrderWizard
                                {
                                    Contract = o.order_number,
                                    Provider = p.provider_name,
                                    Start_Date = o.start_date,
                                    End_Date = o.end_date,

                                }).Distinct().OrderBy(t => t.End_Date).ToList();


            return View("", contractList);
        }

        /// <summary>
        /// Affichage de la liste des postes 
        /// </summary>
        /// <param name="contract">numéro de contrat</param>
        /// <param name="searchString">retour de la valeur à chercher selectionnée dans le front (X.PagedList)</param>
        /// <param name="page">retour du numéro de page selectionné dans le front (X.PagedList)</param>
        /// <returns>Liste des postes du contrat qui contiennent des Gaz</returns>
        public ActionResult GetContract(string contract, string searchString, int? page)
        {
            contractNumber = contract;
            partialSave.Clear();

            try
            {
                ViewBag.contract = contract;

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = null;
                }

                ViewBag.CurrentFilter = searchString;

                //liste des postes
                var postesList = (from od in db.tbl_607_order_details
                                  join o in db.tbl_607_order on od.FK_ID_order equals o.ID
                                  join g in db.tbl_607_gaz on od.ID equals g.FK_ID_order_details
                                  where o.order_number == contract
                                  select new OrderWizardViewModel
                                  {
                                      idPoste = od.ID,
                                      posteNumber = od.poste_number,
                                      contractQ = od.order_quantity,
                                      recievedQ = od.recieved_quantity,
                                      lifeTime = od.gaz_lifetime,
                                      stockMini = od.stock_mini,
                                      shippingDelay = od.shipping_delays,
                                      onTheRoadQ = 0,
                                      content = od.content_comments
                                  }).OrderBy(t => t.posteNumber).Distinct().ToList();
                if (postesList.Count > 0)
                {
                    //pour chaque poste on calcule la quantité de bouteilles en cours de livraison
                    foreach (var item in postesList)
                    {
                        item.onTheRoadQ = (from od in db.tbl_607_order_details
                                           join srd in db.tbl_607_shipping_request_details on od.ID equals srd.FK_ID_order_details
                                           where item.idPoste == srd.FK_ID_order_details & (srd.shipping_quantity > srd.reception_quantity)
                                           select (int?)srd.shipping_quantity.Value - (int?)srd.reception_quantity.Value).Sum() ?? 0;
                    }

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        postesList = postesList.Where(s => s.posteNumber.ToString().Contains(searchString)).OrderBy(t => t.posteNumber).ToList();
                    }
                    int pageNumber = (page ?? 1);
                    var onePageOfPostes = postesList.ToPagedList(pageNumber, 10);
                    ViewBag.OnePageOfPostes = onePageOfPostes;
                    return View("_PartialPostesList");
                }
                else
                {
                    //le poste ne contient pas de Gaz
                    return Content("Empty");
                }
            }
            catch (Exception ex)
            {
                //generation de la vue affichage erreur
                return RedirectToAction("Index", "Ooops", new { message = ex });
            }
        }

        /// <summary>
        /// Sauvegarde dans le dictionnaire de la ligne de commande validée
        /// </summary>
        /// <param name="posteid">numéro du poste stocké dans le champ key</param>
        /// <param name="quantity">quantité stockée dans le champ value</param>
        /// <returns></returns>
        public ActionResult Savepartial(int posteid, int quantity)
        {
            partialSave.Add(posteid, quantity);
            return Content("SAVED");
        }

        /// <summary>
        /// Suppression d'une ligne de commande dans le dictionnaire
        /// </summary>
        /// <param name="posteid">numéro du poste de la ligne à rechercher dans le champ key</param>
        /// <returns></returns>
        public ActionResult RemovePartial(int posteid)
        {
            partialSave.Remove(posteid);
            return Content("REMOVED");
        }

        /// <summary>
        /// Validation du panier
        /// </summary>
        /// <param name="contract">numéro de contrat</param>
        /// <returns></returns>
        public ActionResult SaveAll(string contract)
        {
            //recuperation du user actif
            string user = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            //generation de numéro de demande
            DateTime datee = DateTime.Now;
            sr_ref = (System.Web.HttpContext.Current.User.Identity.Name.ToString().Remove(0, 5)) + "-" + datee.ToString("yyyy-MM-dd") + "-" + datee.ToString("HH:mm");
            sr_ref = sr_ref.Replace(':', '-');
            fileName = sr_ref;

            try
            {
                //recuperation de l'id user
                int fK_ID_actors = (db.tbl_607_actors.Where(u => u.id_uid == user)).First().id;

                //recuperation de l'id contrat
                int contractID = db.tbl_607_order.Where(t => t.order_number == contract).Select(t => t.ID).First();

                //sauvegarde de shipping request
                tbl_607_shipping_request tbl_607_Shipping_Request = new tbl_607_shipping_request();
                tbl_607_Shipping_Request.request_date = DateTime.Now;
                tbl_607_Shipping_Request.FK_ID_actors = fK_ID_actors;
                tbl_607_Shipping_Request.FK_order = contractID;
                tbl_607_Shipping_Request.shipping_request_ref = sr_ref;
                db.tbl_607_shipping_request.Add(tbl_607_Shipping_Request);
                db.SaveChanges(); //sauvegarde

                //recuperation de l'id de shipping request
                int idShippingRequest = db.tbl_607_shipping_request.Where(t => t.shipping_request_ref == sr_ref).Select(t => t.ID).First();

                //sauvegarde de shipping request details
                tbl_607_shipping_request_details tbl_607_Shipping_Request_Details = new tbl_607_shipping_request_details();

                foreach (KeyValuePair<int, int> value in partialSave)
                {
                    tbl_607_Shipping_Request_Details.FK_ID_order_details = value.Key;
                    tbl_607_Shipping_Request_Details.shipping_quantity = value.Value;
                    tbl_607_Shipping_Request_Details.reception_quantity = 0;
                    tbl_607_Shipping_Request_Details.FK_shipping_request = idShippingRequest;
                    db.tbl_607_shipping_request_details.Add(tbl_607_Shipping_Request_Details);
                    db.SaveChanges(); //sauvegarde
                }
                return Content(sr_ref);
            }
            catch (Exception ex)
            {
                //generation de la vue affichage erreur
                return RedirectToAction("Index", "Ooops", new { message = ex });
            }

        }

        /// <summary>
        /// Génération du bon de comande
        /// </summary>
        /// <returns>Fichier à télécharger, celui-ci est également stocké sur le serveur</returns>
        public ActionResult GenerateDocument(/*string sr_ref*/)
        {
            //données pour le remplissage
                //Demandeur
                var demandeur = db.tbl_607_actors.Where(t => t.id_uid == System.Web.HttpContext.Current.User.Identity.Name.ToString()).FirstOrDefault();
                string nomPrenom = demandeur.first_name + " " + demandeur.last_name;
                //Telephone
                string phone = "0123456789";
                //Fax
                string fax = "9876543210";
                //Adresse
                string adresse = "1 Avenue Paul Ourliac 31000 TOULOUSE";
            
                //Destinataire
                var fournisseur = db.tbl_607_order.Include(t => t.tbl_607_provider).Where(t => t.order_number == contractNumber).FirstOrDefault();
                //Nom fournisseur
                string nomFournisseur = fournisseur.tbl_607_provider.provider_name;
                //Tel fournisseur
                string phoneFournisseur = fournisseur.tbl_607_provider.phone;
                //Fax fournisseur
                string faxFournisseur = fournisseur.tbl_607_provider.fax;
                //Adresse fournisseur (seul le champ ville est necessaire)
                string villeFournisseur = fournisseur.tbl_607_provider.town;
                //Mail fournisseur
                string mailFourniseur = fournisseur.tbl_607_provider.mail;
            
            //date de la demande
            DateTime dateDemande = DateTime.Now.Date;
            
            //numéro de commande
            string numCommande = contractNumber;
            
            //semaine
            int num_semaine = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            //ref interne
            //sr_ref;

            //numero de poste et quantité
            //faire un foreach du dictionnaire partialSave -> en key le numéro du poste et en value la quantité
            foreach (KeyValuePair<int, int> value in partialSave)
            {
                int poste = value.Key;
                int quantite = value.Value;
            }

            //Gaz -> on va utiliser le comment du poste mais on pourrait aussi agréger tous les gaz.
            foreach (KeyValuePair<int, int> value in partialSave)
            {
                string comment = db.tbl_607_order_details.Include(t => t.tbl_607_order).Where(t => t.tbl_607_order.order_number == contractNumber & t.poste_number == value.Key).Select(t => t.content_comments).FirstOrDefault();
            }
           
            try
            {
                // Utilisation du NameSpace WordprocessingML:
                string WordprocessingML = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            // Création du WordML
            XmlDocument xmlStartPart = new XmlDocument();
            XmlElement tagDocument = xmlStartPart.CreateElement("w:document", WordprocessingML);
            xmlStartPart.AppendChild(tagDocument);
            XmlElement tagBody = xmlStartPart.CreateElement("w:body", WordprocessingML);
            tagDocument.AppendChild(tagBody);
            XmlElement tagParagraph = xmlStartPart.CreateElement("w:p", WordprocessingML);
            tagBody.AppendChild(tagParagraph);
            XmlElement tagRun = xmlStartPart.CreateElement("w:r", WordprocessingML);
            tagParagraph.AppendChild(tagRun);
            XmlElement tagText = xmlStartPart.CreateElement("w:t", WordprocessingML);
            tagRun.AppendChild(tagText);

            // Insertion du texte
            XmlNode nodeText = xmlStartPart.CreateNode(XmlNodeType.Text, "w:t", WordprocessingML);
                nodeText.Value = @"test";
                tagText.AppendChild(nodeText);

            //specification du dossier d'archivage
            //a decommenter si debug sur iis local
            //string downloadsPath = @"\\cw01.contiwan.com\tlsm\did25010\Spangaz\Temp\";
            //a decommenter en prod
            string downloadsPath = @"D:\SpanGazData\Orders\";

            // Création d'un nouveau package
            Package pkgOutputDoc = null;
            string temporaryFileFullPath = downloadsPath + fileName + ".docx";
            pkgOutputDoc = Package.Open(temporaryFileFullPath, FileMode.Create, FileAccess.ReadWrite);


            // Création d'une part
            Uri uri = new Uri("/word/document.xml", UriKind.Relative);
            PackagePart partDocumentXML = pkgOutputDoc.CreatePart(uri, "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml");

            StreamWriter streamStartPart = new StreamWriter(partDocumentXML.GetStream(FileMode.Create, FileAccess.Write));
            xmlStartPart.Save(streamStartPart);
            streamStartPart.Close();

            // Création de la RelationShip
            pkgOutputDoc.CreateRelationship(uri, TargetMode.Internal,
               "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "rId1");
            pkgOutputDoc.Flush();

            // Fermeture du package
            pkgOutputDoc.Close();
            
            //a decommenter en prod
            return Content(@".\Archives\Orders\" + fileName + ".docx");
            }
            catch (Exception ex)
            {
                //generation de la vue affichage erreur
                return RedirectToAction("Index", "Ooops", new { message = ex });
            }

        }

    }
}