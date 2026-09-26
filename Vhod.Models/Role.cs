using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Vhod.Models
{
    public class Role
    {
        [Key]
        public int Role_ID { get; set; }

        public string Role_Name { get; set; }
    }
}
