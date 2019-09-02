using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoJWT.DTO
{
    public class RolManagementDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RolId { get; set; }
    }
}
