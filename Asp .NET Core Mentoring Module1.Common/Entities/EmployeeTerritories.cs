using Asp_.NET_Core_Mentoring_Module1.Entities;

namespace Asp_.NET_Core_Mentoring_Module1.Common.Entities
{
    public class EmployeeTerritories
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; }

        public virtual Employees Employee { get; set; }
        public virtual Territories Territory { get; set; }
    }
}
