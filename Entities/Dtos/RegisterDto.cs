namespace FashionStore.Entities.Dtos
{
    public class RegisterDto
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
    }
}
