using System.ComponentModel.DataAnnotations;

namespace DemoJWT.DTO
{
    public class RolesRegisterDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
