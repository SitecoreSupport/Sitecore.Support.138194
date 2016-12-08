using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Data.SqlServer
{
    using System.Reflection;
    using Sitecore.Caching;
    using Sitecore.Configuration;
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
            var myField = typeof(SqlServerClientDataStore).BaseType
                             .BaseType
                             .GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic);

            if (myField != null)
            {
                ICache tempCache = myField.GetValue(this) as ICache;
                if (tempCache==null)
                {
                    return;
                }
                string[] cacheKeys = tempCache.GetCacheKeys();
                string[] array = cacheKeys;
                for (int i = 0; i < array.Length; i++)
                {
                    string key = array[i];
                    object obj = tempCache[key];
                    if (obj != null)
                    {
                        this.Touch(key as string);
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