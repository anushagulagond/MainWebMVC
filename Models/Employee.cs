namespace MainWebMVC.Models
{
    public partial class Employee
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public int DeptId { get; set; }
        public int LibId { get; set; }
        public virtual Department? dept { get; set; }
        public virtual Library? Lib { get; set; }
    }
}
