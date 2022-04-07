using System.ComponentModel.DataAnnotations.Schema;

namespace _4th_Task.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLogIn { get; set; }
        public bool Banned { get; set; }
    }
}
