using System;
using System.Collections.Generic;
using System.Text;
namespace Library.Model.Dtos.Auth;

public class AccessToken
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}