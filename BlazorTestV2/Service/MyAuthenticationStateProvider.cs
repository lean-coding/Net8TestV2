using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using BlazorTestV2.Model;

namespace BlazorTestV2.Service
{
    public class MyAuthenticationStateProvider : AuthenticationStateProvider
    {
        //private readonly ProtectedSessionStorage storage;
        private readonly ProtectedLocalStorage storage;
        private ClaimsPrincipal anonymousPrincipal = new(new ClaimsIdentity());

        /*public MyAuthenticationStateProvider(ProtectedSessionStorage protectedSessionStorage)
        {
            this.storage = protectedSessionStorage;
        }*/

        public MyAuthenticationStateProvider(ProtectedLocalStorage protectedLocalStorage)
        {
            this.storage = protectedLocalStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await storage.GetAsync<S_Member>("LoginMember");
                S_Member? loginMember = null;

                if (result.Success)
                {
                    loginMember = result.Value;
                }

                if (loginMember is null)
                {
                    return await Task.FromResult(new AuthenticationState(anonymousPrincipal));
                }

                ClaimsIdentity identity = new(new List<Claim> {
                    new Claim(ClaimTypes.Name, loginMember.Nickname),
                    new Claim(ClaimTypes.Email, loginMember.EMail),
                    new Claim(ClaimTypes.Role, "Member"),
              }, "CustomAuthentication");

                ClaimsPrincipal principal = new(identity);
                return await Task.FromResult(new AuthenticationState(principal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(anonymousPrincipal));
            }
        }

        public async Task UpdateState(S_Member loginMember)
        {
            ClaimsPrincipal principal;

            if (loginMember is null)
            {
                await storage.DeleteAsync("LoginUserInfo");
                principal = anonymousPrincipal;
            }
            else
            {
                await storage.SetAsync("LoginUserInfo", loginMember);
                ClaimsIdentity identity = new(new List<Claim>
                {
                  new Claim(ClaimTypes.Name, loginMember.Nickname),
                            new Claim(ClaimTypes.Email, loginMember.EMail),
                            new Claim(ClaimTypes.Role, "Member"),
                }, "CustomAuthentication");

                principal = new ClaimsPrincipal(identity);
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal!)));
        }
    }

}

