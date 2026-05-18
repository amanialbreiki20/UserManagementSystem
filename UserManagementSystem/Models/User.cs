namespace UserManagementSystem.Models
{
    public class User
    {

        public bool IsAdmin { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } 

        public string ResetToken { get; set; } = "";

        public DateTime? ResetTokenExpiry { get; set; }
    }
}
