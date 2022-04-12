namespace Network.Models
{
    public class AddresseeMessage
    {
        public int AddresseeId { get; set; }
        public virtual User Addressee { get; set; } = null!;

        public int MessageId { get; set; }
        public virtual Message Message { get; set; } = null!;
    }
}
