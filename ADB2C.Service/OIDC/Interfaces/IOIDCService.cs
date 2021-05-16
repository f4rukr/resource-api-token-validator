using ADB2C.Model.Models.OIDC;

namespace ADB2C.Service.OIDC.Interfaces
{
    public interface IOIDCService
    {
        string BuildIdToken();
        string GetJwks();
        string GetMetadata();
    }
}
