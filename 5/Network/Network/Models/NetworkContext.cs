using Microsoft.EntityFrameworkCore;

namespace Network.Models
{
    public class NetworkContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        public NetworkContext(DbContextOptions<NetworkContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Author)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedMessages)
                .WithMany(m => m.Addressees)
                .UsingEntity<AddresseeMessage>(
                    j => j
                        .HasOne(am => am.Message)
                        .WithMany(m => m.AddresseeMessages)
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasForeignKey(am => am.MessageId),
                    j => j
                        .HasOne(am => am.Addressee)
                        .WithMany(a => a.AddresseeMessages)
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasForeignKey(am => am.AddresseeId),
                    j =>
                    {
                        j.HasKey(am => new { am.AddresseeId, am.MessageId });
                    }); 
                        

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .OnDelete(DeleteBehavior.NoAction);
        }

        #region User

        public User? CreateUser(string email, string password, string name, Role role)
        {
            var user = new User
            {
                Email = email,
                Password = password,
                Name = name,
                Role = role,
                Created = DateTime.Now,
                LoggedOn = DateTime.Now
            };
            return user;
        }

        public void AddUsers(params User[] users)
        {
            Users.AddRange(users);
            SaveChanges();
        }

        public void DeleteUsers(params int[] ids)
        {
            Users.RemoveRange(Users.Where(x => ids.Contains(x.Id)));
            SaveChanges();
        }

        public void DeleteUsers(params User[] users)
        {
            Users.RemoveRange(users);
            SaveChanges();
        }

        public bool EditUser(User user, string email, string password, string name, Role? role)
        {
            if (user == null || role == null) return false;
            if (UserExists(email, name)) return false;
            user.Email = email;
            user.Password = password;
            user.Name = name;
            user.Role = role;
            SaveChanges();
            return true;
        }

        public User? FindUserByEmail(string email)
        {
            return Users.FirstOrDefault(u => u.Email == email);
        }

        public User? FindUserByName(string name)
        {
            return Users.FirstOrDefault(u => u.Name == name);
        }

        public bool UserExists(string email, string name)
        {
            if (FindUserByEmail(email) != null) return true;
            if (FindUserByName(name) != null) return true;
            return false;
        }

        public void OnLogin(User user)
        {
            user.LoggedOn = DateTime.Now;
            SaveChanges();
        }

        #endregion

        #region Role
        
        public Role? GetUserRole()
        {
            return FindRoleByName("user");
        }

        public Role? GetAdminRole()
        {
            return FindRoleByName("admin");
        }

        public Role? FindRoleByName(string roleName)
        {
            return Roles.FirstOrDefault(r => r.Name == roleName);
        }

        #endregion

        #region Message

        public void SendMessage(Message? msg)
        {
            if (msg == null) return;
            Messages.Add(msg);
            SaveChanges();
        }

        #endregion

        private void Initialization()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            var adminRole = new Role { Name = "admin" };
            var userRole = new Role { Name = "user" };
            var adminUser = new User
            {
                Email = "m1ker1nigor@yandex.ru",
                Password = "m1ker1nigor@yandex.ru",
                Name = "Myakotin Igor",
                Created = DateTime.Now,
                LoggedOn = DateTime.Now,
                Role = adminRole
            };
            var testUser = new User
            {
                Email = "test@test.ru",
                Password = "test@test.ru",
                Name = "Test",
                Created = DateTime.Now,
                LoggedOn = DateTime.Now,
                Role = adminRole
            };
            Roles.Add(userRole);
            SaveChanges();
            Roles.Add(adminRole);
            SaveChanges();
            Users.Add(adminUser);
            SaveChanges();
            Users.Add(testUser);
            SaveChanges();
        }
    }
}
