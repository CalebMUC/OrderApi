using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class County
    {
        [Key]
        public int CountyId { get; set; }
        public int CountyCode { get; set; }
        public string CountyName { get; set; }
        public DateTime CreatedOn { get; set; }

        // One County has many Towns
        public virtual ICollection<Town> Towns { get; set; }
    }

    public class Town
    {
        [Key]
        public int TownId { get; set; }
        public string TownName { get; set; }
        public DateTime CreatedOn { get; set; }

        // Foreign key to County
        public int CountyId { get; set; }

        // Navigation property to County
        [ForeignKey("CountyId")]
        public virtual County County { get; set; }

        // One Town has many DeliveryStations
        public virtual ICollection<DeliveryStation> DeliveryStations { get; set; }
    }

    public class DeliveryStation
    {
        [Key]
        public int DeliveryStationId { get; set; }
        public string DeliveryStationName { get; set; }
        public DateTime CreatedOn { get; set; }

        // Foreign key to Town
        public int TownId { get; set; }

        // Navigation property to Town
        [ForeignKey("TownId")]
        public virtual Town Town { get; set; }
    }
}
