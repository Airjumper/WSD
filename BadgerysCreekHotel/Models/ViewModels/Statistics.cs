using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgerysCreekHotel.Models.ViewModels
{
    public class Statistics
    {


        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        [Display(Name = "Number of Bookings")]
        public int NumOfBooking { get; set; }


        [Display(Name = "Postcode")]
        public string PostCode { get; set; }

        [Display(Name = "Number of Customers")]
        public int NumOfCusotmer { get; set; }



    }
}
