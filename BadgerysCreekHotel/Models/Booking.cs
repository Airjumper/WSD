using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgerysCreekHotel.Models
{
    public class Booking
    {

        public int ID { get; set; }

        //foreign key
        public int RoomID { get; set; }

        public string CustomerEmail { get; set; }

        [Display(Name = "CheckIn Date")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Display(Name = "CheckOut Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [DataType(DataType.Currency)]
        public decimal  Cost { get; set; }



        //navigation property
        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }

    }
}
