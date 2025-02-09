public class UserData {
    public int id_user { get; set; }

    public string username { get; set; } = null!;
    public string phone { get; set; } = null!;
    public string user_type { get; set; } =null!;
    public string password { get; set; } =null!;

    public DateTime DateAdded { get; set; }
}
public class UserDataDto {
    public string username { get; set; } = null!;
    public string phone { get; set; } = null!;
    public string user_type { get; set; } =null!;
    public string password { get; set; } =null!;

}

public class LoginDto {
    public int id_user { get; set; }

    public string username { get; set; } = null!;
    public string password { get; set; } =null!;

}

