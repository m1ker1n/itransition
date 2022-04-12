using Network.Models;

namespace Network.ViewModels
{
    public class AdminPanelModel
    {
        public ICollection<User> Users { get; set; } = null!;
        public ICollection<Role> Roles { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = null!;
    }
}
