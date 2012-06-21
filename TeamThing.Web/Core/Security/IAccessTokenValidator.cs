namespace TeamThing.Web.Core.Security
{
    public interface IAccessTokenValidator
    {
        bool ValidateToken(string accessToken, string[] scope);
    }
}