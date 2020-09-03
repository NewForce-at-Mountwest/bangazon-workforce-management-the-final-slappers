using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeTrainingViewModel
    {
        public Employee employee { get; set; }

        public List<SelectListItem> programs { get; set; } = new List<SelectListItem>();
        public List<int> selectedPrograms { get; set; } = new List<int>();
    }
}
