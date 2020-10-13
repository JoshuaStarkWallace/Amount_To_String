using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmountToString.Models
{
    public class FormInput
    {
        [Display(Name = "Amount")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }
}
