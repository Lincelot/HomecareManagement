using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace HomecareManagement.Models.Web
{
    public class AccountModel
    {
        public int uid { get; set; }
        [Required]
        [Display(Name = "Username")]
        public String username { get; set; }
        [Required]
        [Display(Name = "Password")]
        public String password { get; set; }
        public int level { get; set; }
        public String displayname { get; set; }
    }
}