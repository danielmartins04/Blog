namespace Blog;

public static class Configuration { 
    public static string JwtKey = "ODE5NDQ2NDAtY2IwMy00NjU3LTkxYWMtNmM0ZGExZTFjZDJi";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_api_ILTevUM/z0ey3NwCV/unWg==";
    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}