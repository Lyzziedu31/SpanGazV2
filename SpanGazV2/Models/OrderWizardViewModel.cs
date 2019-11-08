using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpanGazV2.Models
{
    /// <summary>
    /// Modèle temporaire pour l'affichage des données des postes récupérées sur plusieurs tables
    /// </summary>
    public class OrderWizardViewModel
    {
        /// <summary>
        /// numéro du poste
        /// </summary>
        public int posteNumber { get; set; }
        /// <summary>
        /// quantité au contrat
        /// </summary>
        public int contractQ { get; set; }
        /// <summary>
        /// quantité reçue
        /// </summary>
        public int recievedQ { get; set; }
        /// <summary>
        /// quantité en cours de livraison
        /// </summary>
        public int onTheRoadQ { get; set; }
        /// <summary>
        /// Durée de vie
        /// </summary>
        public int lifeTime { get; set; }
        /// <summary>
        /// Stock minimum
        /// </summary>
        public int stockMini { get; set; }
        /// <summary>
        /// délai de livraison
        /// </summary>
        public int shippingDelay { get; set; }
        /// <summary>
        /// id du poste
        /// </summary>
        public int idPoste { get; set; }
        /// <summary>
        /// contenu
        /// </summary>
        public string content { get; set; }
    }
}