using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Network.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Subject { get; set; } = String.Empty;
        public string Body { get; set; } = String.Empty;
        public DateTime Created { get; set; }

        public int? ReplyToId { get; set; }
        [JsonIgnoreAttribute]
        public virtual Message? ReplyTo { get; set; } = null;

        public int AuthorId { get; set; }
        [JsonIgnoreAttribute]
        public virtual User Author { get; set; } = null!;

        [JsonIgnoreAttribute]
        public virtual ICollection<User> Addressees { get; set; } = null!;
        [JsonIgnoreAttribute]
        public virtual ICollection<AddresseeMessage> AddresseeMessages { get; set; } = null!;
    }
}
