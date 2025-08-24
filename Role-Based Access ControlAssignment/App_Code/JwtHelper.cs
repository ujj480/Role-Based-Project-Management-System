using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class JwtUser
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string[] Roles { get; set; }
    public bool IsInRole(string role) { foreach (var r in Roles) if (string.Equals(r, role, StringComparison.OrdinalIgnoreCase)) return true; return false; }
}

public static class JwtHelper
{
    private static string Secret = System.Configuration.ConfigurationManager.AppSettings["JWT:Secret"];
    private static string Issuer = System.Configuration.ConfigurationManager.AppSettings["JWT:Issuer"];
    private static string Audience = System.Configuration.ConfigurationManager.AppSettings["JWT:Audience"];

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        string output = input;
        output = output.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 0: break;
            case 2: output += "=="; break;
            case 3: output += "="; break;
            default: throw new Exception("Illegal base64url string!");
        }
        return Convert.FromBase64String(output);
    }

    public static string GenerateToken(JwtUser user, int minutes = 120)
    {
        var header = new Dictionary<string, object> { { "alg", "HS256" }, { "typ", "JWT" } };
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var payload = new Dictionary<string, object> {
            { "sub", user.UserId },
            { "name", user.Username },
            { "roles", user.Roles },
            { "iss", Issuer },
            { "aud", Audience },
            { "iat", now },
            { "exp", now + minutes * 60 }
        };
        var ser = new JavaScriptSerializer();
        string headerJson = ser.Serialize(header);
        string payloadJson = ser.Serialize(payload);
        string header64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        string payload64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        string signingInput = header64 + "." + payload64;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret)))
        {
            var sig = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
            var sig64 = Base64UrlEncode(sig);
            return signingInput + "." + sig64;
        }
    }

    public static JwtUser ValidateToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;
            string signingInput = parts[0] + "." + parts[1];
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret)))
            {
                var sig = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
                var sig64 = Base64UrlEncode(sig);
                if (!SlowEquals(sig64, parts[2])) return null;
            }
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            var ser = new JavaScriptSerializer();
            var payload = ser.Deserialize<Dictionary<string, object>>(payloadJson);
            long exp = Convert.ToInt64(payload["exp"]);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now > exp) return null;
            var user = new JwtUser
            {
                UserId = Convert.ToInt32(payload["sub"]),
                Username = (string)payload["name"],
                Roles = ser.ConvertToType<string[]>(payload["roles"])
            };
            return user;
        }
        catch { return null; }
    }

    private static bool SlowEquals(string a, string b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
            diff |= (uint)(a[i] ^ b[i]);
        return diff == 0;
    }

    public static void SetAuthCookie(HttpContext ctx, string token)
    {
        var c = new HttpCookie("auth_token", token);
        c.HttpOnly = true;
        c.Secure = false; // set true under HTTPS
        c.Path = "/";
        ctx.Response.Cookies.Add(c);
    }
    public static void ClearAuthCookie(HttpContext ctx)
    {
        var c = new HttpCookie("auth_token", "");
        c.Expires = DateTime.Now.AddDays(-1);
        c.Path = "/";
        ctx.Response.Cookies.Add(c);
    }
    public static JwtUser GetUserFromContext(HttpContext ctx)
    {
        var tok = ctx.Request.Cookies["auth_token"];
        if (tok == null || string.IsNullOrEmpty(tok.Value)) return null;
        return ValidateToken(tok.Value);
    }
    public static JwtUser RequireAuthenticated(HttpContext ctx)
    {
        var u = GetUserFromContext(ctx);
        if (u == null) ctx.Response.Redirect("~/Account/Login.aspx");
        return u;
    }
    public static void RequireRoles(HttpContext ctx, params string[] roles)
    {
        var u = RequireAuthenticated(ctx);
        bool ok = false;
        foreach (var r in roles) if (u.IsInRole(r)) { ok = true; break; }
        if (!ok) ctx.Response.Redirect("~/Account/Login.aspx");
    }
}
