using System.Collections.Concurrent;
using BlazorTestV2.Model;

namespace BlazorTestV2.Service
{
    /// <summary>
    /// 傳遞全站共用變數(如 VisitorCount), 中介事件觸發(如使用者登入事件)
    /// Singleton 層級服務 , 需要在 AuthenticationStateProvider 前
    /// </summary>
    public class MySyncService : IDisposable
    {
        
        readonly ILogger<MySyncService> _logger;
        readonly IConfiguration _config;
        private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
        private int VisitorCount = 0; //在線人數

        public MySyncService(ILogger<MySyncService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _logger.LogWarning("MySyncService.Initial", null);
        }

        void IDisposable.Dispose() {
            _logger.LogWarning("MySyncService.Dispose", null);
        }

        #region 在線人數計算
        public async Task AddVisitor()
        {
            VisitorCount++;
            this.NotifyVisitorChanged(VisitorCount, EventArgs.Empty);
        }

        public async Task MinusVisitor()
        {
            VisitorCount--;
            this.NotifyVisitorChanged(VisitorCount, EventArgs.Empty);
        }

        public int GetVisitorCount()
        {
            return VisitorCount;
        }
        #endregion

        #region 委派事件
        public event EventHandler<EventArgs> OnUserChanged;
        public void NotifyUserChanged(S_Member member, EventArgs e)
        {
            OnUserChanged?.Invoke(member, e);
        }

        public event EventHandler<EventArgs> OnVisitorChanged;
        public void NotifyVisitorChanged(int VisitorCount, EventArgs e)
        {
            OnVisitorChanged?.Invoke(VisitorCount, e);
        }

        
        //public Action VisitorChanged { get; set; }
        //public Action<int>? CountChanged { get; set; }
        #endregion
    }
}
