using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee employee { get; set; }

        public int currentComputerId { get; set; }
        public int selectedComputerId { get; set; }

        public List<SelectListItem> departments { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> computers { get; set; } = new List<SelectListItem>();

    }
}
