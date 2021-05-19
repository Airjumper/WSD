using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BadgerysCreekHotel.Models
{
    public class Customer
    {
        [Key, Required]
        [DataType(DataType.EmailAddress)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Sur Name")]
        [RegularExpression(@"^[A-Za-z-']{2,20}$", ErrorMessage = "Magazine name only consist of English letters, hyphen and apostrophe with 2-20 long.")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Given Name")]
        [RegularExpression(@"^[A-Za-z-']{2,20}$", ErrorMessage = "Magazine name only consist of English letters, hyphen and apostrophe with 2-20 long.")]
        public string GivenName { get; set; }


        [NotMapped] // not mapping this property to database, but exist in memory
        public string FullName => $" {GivenName}   {Surname}";


        [Required]
        [RegularExpression(@"^[0-8]{1}[0-9]{3}$", ErrorMessage = "Invalid format.  Post number should be 4 digits and not start with 9 ")]
        public String PostNumber { get; set; }



        //Navigation properties
        public ICollection<Booking> TheBookings  { get; set; }

    }
}
