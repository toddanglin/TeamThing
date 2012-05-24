namespace TeamThing.Web.Controllers
{
    public interface IOAuthProvider
    {
        BasicUserData GetUser();

        bool IsTokenValid();
    }
}