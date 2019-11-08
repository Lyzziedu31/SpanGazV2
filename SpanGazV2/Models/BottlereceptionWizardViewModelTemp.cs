using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpanGazV2.Models
{
    /// <summary>
    /// Modèle de stockage temporaire des données du Wizard de réception d'une bouteille
    /// </summary>
    public static class BottlereceptionWizardViewModelTemp
    {
        /// <summary>
        /// id du contrat
        /// </summary>
        public static int idcontract { get; set; }
        /// <summary>
        /// numéro de poste
        /// </summary>
        public static int posteNumber { get; set; }
        /// <summary>
        /// Référence du BL
        /// </summary>
        public static int BL_ref { get; set; }
        /// <summary>
        /// Date de réception
        /// </summary>
        public static DateTime reception_date { get; set; }
        /// <summary>
        /// numéro du certificat de conformité
        /// </summary>
        public static int conformity_certificate { get; set; }
        /// <summary>
        /// date de fabrication
        /// </summary>
        public static DateTime fabrication_date { get; set; }
        /// <summary>
        /// date d'expiration
        /// </summary>
        public static DateTime expiration_date { get; set; }
        /// <summary>
        /// quantité restant à recevoir
        /// </summary>
        public static Nullable<int> reception_quantity { get; set; }
        /// <summary>
        /// quantité reçue
        /// </summary>
        public static Nullable<int> recieved_quantity { get; set; }
        /// <summary>
        /// id Gaz
        /// </summary>
        public static int gaz_id { get; set; }
        /// <summary>
        /// concentration réelle
        /// </summary>
        public static float real_content { get; set; }
        /// <summary>
        /// numéro fabriquant de la bouteille
        /// </summary>
        public static string manufacturer_bottle_number { get; set; }
        /// <summary>
        /// numéro conti de la bouteille
        /// </summary>
        public static string bottle_conti_number { get; set; }
        /// <summary>
        /// localistaion de la bouteille
        /// </summary>
        public static string bottle_location { get; set; }
        /// <summary>
        /// clef étrangère de la demande de la demande de livraison
        /// </summary>
        public static int FK_ID_shipping_request { get; set; }
        /// <summary>
        /// commentaires sur le contenu
        /// </summary>
        public static string content { get; set; }

    }
        
}