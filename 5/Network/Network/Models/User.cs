using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Network.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime LoggedOn { get; set; }

        public int RoleId { get; set; }
        [JsonIgnoreAttribute]
        public virtual Role Role { get; set; } = null!;

        [JsonIgnoreAttribute]
        public virtual ICollection<Message> SentMessages { get; set; } = null!;
        [JsonIgnoreAttribute]
        public virtual ICollection<Message> ReceivedMessages { get; set; } = null!;
        [JsonIgnoreAttribute]
        public virtual ICollection<AddresseeMessage> AddresseeMessages { get; set; } = null!;
    }
}
