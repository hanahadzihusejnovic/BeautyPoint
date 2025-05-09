namespace BeautyPoint.Dtos
{
    public class UserInfoDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
