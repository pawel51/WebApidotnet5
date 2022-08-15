using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTO.InDto
{
    public abstract class ChangeCompanyDto
    {

        [Required(ErrorMessage = "Name is a required field")]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Address is a required field")]
        [MinLength(3)]
        [MaxLength(30)]
        public string Address { get; set; }
        [Required(ErrorMessage = "Country is a required field")]
        [MinLength(3)]
        [MaxLength(30)]
        public string Country { get; set; }

        public IEnumerable<AddEmployeeDto> Employees { get; set; }
    }
}
