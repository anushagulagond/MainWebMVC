using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MainWebMVC.Models
{
    public class EmpDept
    {
        [DisplayName("Employee Id")]
        public int EmpId { get; set; }

        [DisplayName("Employee Name")]
        public string EmpName { get; set; }

        [DisplayName("Department Id")]
        public int DeptId { get; set; }

        [DisplayName("Department Name")]
        public string DeptName { get; set; }

        [DisplayName("Department Location")]
        public string DeptLoc { get; set; }

        [Display(Name = "Library Id")]
        public int LibId { get; set; }

        [Display(Name ="Library Name")]
        public string LibName { get; set; }
        
        public virtual Employee? emp { get; set; }
        public virtual Department? dept { get; set; }
        public virtual Library? Lib { get; set; }

    }
}
