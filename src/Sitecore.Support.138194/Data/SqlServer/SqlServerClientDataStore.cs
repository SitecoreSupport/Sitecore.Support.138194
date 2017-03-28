namespace Sitecore.Support.Data.SqlServer
{
    using System.Reflection;
    using System.Web;
    using Sitecore.Caching;
    using Sitecore.Data.DataProviders.Sql;
    using Sitecore.Diagnostics;
    using Sitecore.Diagnostics.PerformanceCounters;
    using Sitecore.Sites;

    public class SqlServerClientDataStore : Sitecore.Data.SqlServer.SqlServerClientDataStore
    {
        public SqlServerClientDataStore(string connectionString, string objectLifetime) : base(connectionString, objectLifetime)
        {
        }

        public SqlServerClientDataStore(SqlDataApi api, string objectLifetime) : base(api, objectLifetime)
        {
        }


        /// <summary>
        /// The only modified method, all others are copy-pasted due to being private
        /// </summary>
        protected override void TouchActiveData()
        {
            var _cacheFieldInfo = typeof(SqlServerClientDataStore).BaseType.BaseType.GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic);

            if (_cacheFieldInfo != null)
            {
                ICache _cache = _cacheFieldInfo.GetValue(this) as ICache;
                if (_cache == null) return;
                                
                foreach (var key in _cache.GetCacheKeys())
                {
                    if (!string.IsNullOrEmpty(key) && _cache[key] != null)
                    {
                        this.Touch(key);
                    }
                }
            }
        }

        private void Touch(string key)
        {
            Assert.ArgumentNotNull(key, "key");
            this.TouchData(key);
            DataCount.DataClientDataWrites.Increment(1L);
        }



        private static bool IsDisabled()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            SiteContext site = Context.Site;
            return site != null && site.DisableClientData;
        }


    }
}