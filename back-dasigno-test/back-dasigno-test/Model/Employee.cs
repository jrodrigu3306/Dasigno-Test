using System.ComponentModel.DataAnnotations;

namespace back_dasigno_test.Model
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string? SecondName { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string? SecondSurname { get; set; }
        public DateOnly Birthdate { get; set; }
        public double Salary { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
     
    public class EmployeeUpdateModel
    {
        public int EmployeeId { get; set; }
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? SecondName { get; set; }
        [MaxLength(50)]
        public string? Surname { get; set; }
        [MaxLength(50)]
        public string? SecondSurname { get; set; }
        public DateOnly? Birthdate { get; set; }
        public double? Salary { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

}
