using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpanGazV2.Models
{
    /// <summary>
    /// Modèle temporaire pour le stockage des informations qui seront affichées dans le wizard GazReporting
    /// </summary>
    public class GazReportingWizardViewModel
    {
        /// <summary>
        /// Numéro conti de la bouteille
        /// </summary>
        public string bottle_conti_number { get; set; }
        /// <summary>
        /// pression relevée
        /// </summary>
        public int? pressure_value { get; set; }
        /// <summary>
        /// date de relève
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? reporting_date { get; set; }
        /// <summary>
        /// prénom du technicien
        /// </summary>
        public string? first_name { get; set; }
        /// <summary>
        /// nom du technicien
        /// </summary>
        public string? last_name { get; set; }
        /// <summary>
        /// date d'expiration
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime expiration_date { get; set; }
        /// <summary>
        /// contenu
        /// </summary>
        public string content { get; set; }

    }
}