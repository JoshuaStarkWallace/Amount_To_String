using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AmountToString.Models;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace AmountToString.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /* Integer indexes retrieve the name for the ones place */
        static String[] one = { "", "one ", "two ", "three ", "four ",
                            "five ", "six ", "seven ", "eight ",
                            "nine ", "ten ", "eleven ", "twelve ",
                            "thirteen ", "fourteen ", "fifteen ",
                            "sixteen ", "seventeen ", "eighteen ",
                            "nineteen " };

        /* Integer indexes retrieve the name for the tens place */
        static String[] ten = { "", "", "twenty ", "thirty ", "forty ",
                            "fifty ", "sixty ", "seventy ", "eighty ",
                            "ninety " };


        static String twoDigitValueToText(ulong value, String suffix)
        {
            String str = "";

            if (suffix == "" && value >= 21 && value <= 99)
            {
                /* Hypehens should be used when writing two-word number
                from twenty-one to ninety-nine (inclusive) as words.
                A blank suffix tells us that we're not handling a case
                like "thousands" or "millions". */
                str += ten[value / 10].TrimEnd(' ') + '-' + one[value % 10];
            }
            else if (value > 19)
            {
                str += ten[value / 10] + one[value % 10];
            }
            else
            {
                str += one[value];
            }

            if (value != 0)
            {
                str += suffix;
            }

            return str;
        }

        static String valueToText(ulong value)
        {
            String output = "";

            /* [100,000,000 - 1,000,000,000) */
            String description = "";
            if (((value / 10000000) % 100) == 0)
            {
                description = "hundred million ";
            }
            else
            {
                description = "hundred ";
            }
            output += twoDigitValueToText(value / 100000000, description);

            /* [1,000,000 - 100,000,000) */
            output += twoDigitValueToText((value / 10000000) % 100, "million ");

            /* [100,000 - 1,000,000) */
            if (((value / 1000) % 100) == 0)
            {
                description = "hundred thousand ";
            }
            else
            {
                description = "hundred ";
            }
            output += twoDigitValueToText((value / 100000) % 10, description);

            /* [1000 - 100,000) */
            output += twoDigitValueToText((value / 1000) % 100, "thousand ");

            /* [100 - 1000) */
            output += twoDigitValueToText((value / 100) % 10, "hundred ");

            /* [0 - 100) */
            output += twoDigitValueToText(value % 100, "");

            return output;
        }

        [HttpPost]
        public ActionResult Index(FormInput input)
        {
            if (input.Amount >= 1000000000 || input.Amount <= -1000000000)
            {
                ViewData["result"] = "Error! Input amount must be less than one billion and greater than negative one billion.";
            }
            else if (ModelState.IsValid)
            {
                String prefix = "";

                if (input.Amount < 0)
                {
                    prefix = "negative ";
                    input.Amount *= -1;
                }

                int cents = (int)(Math.Round(input.Amount % 1, 2) * 100);

                /* Prepend a 0 if the cents amount is single digit */
                String centsString = cents.ToString();
                if (cents < 10)
                {
                    centsString = "0" + centsString;
                }
                String ResultString = prefix + valueToText((ulong)input.Amount) + "and " + centsString + "/100 dollars";

                /* Capitalize the first character in the returned string */
                ViewData["result"] = ResultString.First().ToString().ToUpper() + ResultString.Substring(1);
            }
            else
            {
                ViewData["result"] = "Error!";
            }
            return View();
        }
    }
}
