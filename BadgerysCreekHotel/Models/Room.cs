using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgerysCreekHotel.Models
{
    public class Room
    {
        public int RoomID { get; set; }

        [Required]
        [RegularExpression("^[123G]$")]
        public string Level { get; set; }

        [Range(1,3)]
        public int  BedCount { get; set; }


        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public ICollection<Booking> TheBookings { get; set; }

    }
}
