using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpanGazV2.Models
{
    /// <summary>
    /// Modèle pour l'affichage d'une liste combinée dans le wizard de commande de bouteilles
    /// </summary>
    public class ContractListOrderWizard
    {
        /// <summary>
        /// Numéro de contrat
        /// </summary>
        public string Contract { get; set; }
        /// <summary>
        /// Nom du fournisseur
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// Date de début de contrat
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Start_Date { get; set; }
        /// <summary>
        /// Date de fin de contart
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime End_Date { get; set; }

        /// <summary>
        /// Override de la méthode ToString sur la classe pour la génération du champ combiné
        /// </summary>
        /// <returns></returns>        
        public override string ToString()
        {
            return "Contract: " + Contract + "Provider: " + Provider + "Start Date: " + Start_Date + "End Date: " + End_Date;
        }

    }
}