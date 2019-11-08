using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpanGazV2.Models
{
    /// <summary>
    /// Modèle temporaire pour le stockage des données combinées de plusieurs tables et l'affichage de celles-ci dans la vue
    /// </summary>
    public class DestockBottleInfos
    {
        /// <summary>
        /// Numéro Conti de la bouteille
        /// </summary>
        public string contiNumber { get; set; }
        /// <summary>
        /// Numéro fabricant de la bouteille
        /// </summary>
        public string manufacturerNumber { get; set; }
        /// <summary>
        /// Localisation de la bouteille
        /// </summary>
        public string bottleLocation { get; set; }
        /// <summary>
        /// date de fabrication
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime fabricationDate { get; set; }
        /// <summary>
        /// date d'expiration
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime expirationDate { get; set; }
        /// <summary>
        /// date de reception
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime receptionDate { get; set; }
        /// <summary>
        /// numéro de poste
        /// </summary>
        public int posteNumber { get; set; }
        /// <summary>
        /// numéro du contrat
        /// </summary>
        public string contractNumber { get; set; }
        /// <summary>
        /// gaz type
        /// </summary>
        public string gazType { get; set; }
        /// <summary>
        /// concentration théorique
        /// </summary>
        public double theoricalContent { get; set; }

    }
}