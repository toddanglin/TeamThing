namespace TeamThing.Web.Core.Security
{
    public interface IOAuthProvider
    {
        BasicUserData GetUser();

        bool IsTokenValid();
    }
}