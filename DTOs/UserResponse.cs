namespace Software_Engineering_2025.DTOs
{
    public class UserResponse
    {
        public int User_ID { get; set; }
        public int User_Type_ID { get; set; }
        public string? User_Type_Name { get; set; }
        public string? Title { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Created_At { get; set; }
        public bool Is_Activated { get; set; }
    }
}