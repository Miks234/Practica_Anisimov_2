using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Vhod.Models
{
    public class User
    {
        [Key]
        public int User_ID { get; set; }

        public int User_Role { get; set; }
        public string User_Surname { get; set; }
        public string User_Name { get; set; }
        public string User_Patronymic { get; set; }
        public string User_Login { get; set; }
        public string User_Password { get; set; }
        public bool User_Status { get; set; }
    }
}
