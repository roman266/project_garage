namespace project_garage.Models.JWTSettings
{
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }

        public JWTSettings(IConfiguration config)
        {
            Key = config["Jwt:Key"];
            Issuer = config["Jwt:Issuer"];
            Audience = config["Jwt:Audience"];
            TokenValidityInMinutes = config.GetValue<int>("Jwt:TokenValidityInMinutes");
            RefreshTokenValidityInDays = config.GetValue<int>("Jwt:RefreshTokenValidityInDays");
            }
        }
}
