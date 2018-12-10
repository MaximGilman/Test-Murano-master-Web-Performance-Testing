using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Murano_Denis_Bardakov.Models
{
    [Table("list_employees")]
    public class Employees
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Имя")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [StringLength(50, ErrorMessage = "Длина строки должна быть до 50 символов")]
        [Display(Name = "Должность")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Статус")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [DataType(DataType.Currency)]
        [Display(Name = "Зарплата")]
        public decimal Salary { get; set; }
    }
}