using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeDepartmentViewModel
    {
        public Employee employee { get; set; }

        public List<SelectListItem> departments { get; set; } = new List<SelectListItem>();
    }
}
