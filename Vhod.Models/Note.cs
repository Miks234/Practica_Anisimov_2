using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Vhod.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public int Id_user { get; set; }
        public DateTime created_at { get; set; }
    }
}
