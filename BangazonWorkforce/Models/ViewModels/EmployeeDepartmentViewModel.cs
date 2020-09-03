using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeDepartmentViewModel
    {
        public Employee employee { get; set; }

        public List<SelectListItem> departments { get; set; } = new List<SelectListItem>();
    }
}
