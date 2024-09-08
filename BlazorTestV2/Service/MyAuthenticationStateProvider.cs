using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using BlazorTestV2.Model;

namespace BlazorTestV2.Service
{
    public class MyAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage storage;
        private ClaimsPrincipal anonymousPrincipal = new(new ClaimsIdentity());
        private readonly MySyncService sync;

        public MyAuthenticationStateProvider(ProtectedLocalStorage protectedLocalStorage, MySyncService syncService)
        {
            this.storage = protectedLocalStorage;
            this.sync = syncService;
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
                }, "MyAuthentication");

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
                await storage.DeleteAsync("LoginMember");
                principal = anonymousPrincipal;
            }
            else
            {
                await storage.SetAsync("LoginMember", loginMember);
                ClaimsIdentity identity = new(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginMember.Nickname),
                    new Claim(ClaimTypes.Email, loginMember.EMail),
                    new Claim(ClaimTypes.Role, "Member"),
                }, "MyAuthentication");

                principal = new ClaimsPrincipal(identity);
                                
            }

            //同步前端
            sync.NotifyUserChanged(loginMember, EventArgs.Empty);
        }

        public async Task CheckAuthenticationState()
        {
            //更新前端 登入State
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        /// <summary>
        /// 利用首次 CircuitId 作為標記同一瀏覽器的 Key 值 => 判斷 LocalStorage 內容是否為同一瀏覽器
        /// </summary>
        /// <param name="CircuitId"></param>
        public async void SetUniqueID(string CircuitId)
        {
            bool Reset = true;
            int TimeoutMinutes = 20; //效期 20分鐘
            try
            {
                var result = await storage.GetAsync<string>("BrowserUniqueID");

                if (result.Success)
                {
                    var timeout = await storage.GetAsync<DateTime>("BrowserTimeout");
                    if (timeout.Success)
                    {
                        if (((DateTime)timeout.Value).CompareTo(DateTime.Now) > 0)
                        {
                            await storage.SetAsync("BrowserTimeout", DateTime.Now.AddMinutes(TimeoutMinutes)); //延長 20分鐘
                            Reset = false;
                        }
                    }
                }
            }
            catch { }

            if (Reset)
            {
                try
                {
                    await storage.SetAsync("BrowserUniqueID", CircuitId);
                    await storage.SetAsync("BrowserTimeout", DateTime.Now.AddMinutes(TimeoutMinutes));
                    await sync.AddVisitor();
                }
                catch { }
            }
        }

    }
}

