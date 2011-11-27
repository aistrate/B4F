using System;
using System.Web;
using System.Web.SessionState;

namespace B4F.TotalGiro.Client.Web.Util
{
    public class SafeSession
    {
        /// <summary>
        /// This class normally just delegates to the current HttpContext's Session, 
        /// but if that is null, it falls back on the context's Application state, 
        /// prefixing the key with the user name (e.g. "vb:LastUnhandledError" instead of just "LastUnhandledError").
        /// It is useful when handling uncaught errors (Global.asax and Authenticate/AppErrors.aspx.cs).
        /// </summary>
        private SafeSession()
        {
            Context = HttpContext.Current;

            SessionWrapper = new CollectionWrapper(key => key,
                                                   (key, val) => { if (Session != null) Session[key] = val; },
                                                   key => (Session != null ? Session[key] : null),
                                                   key => { if (Session != null) Session.Remove(key); });

            ApplicationWrapper = new CollectionWrapper(key => string.Format("{0}:{1}", Context.User.Identity.Name, key),
                                                       (key, val) => { Application[key] = val; },
                                                       key => Application[key],
                                                       key => { Application.Remove(key); });
        }

        public static SafeSession Current { get { return new SafeSession(); } }

        public HttpContext Context { get; private set; }
        public HttpSessionState Session { get { return Context.Session; } }
        public HttpApplicationState Application { get { return Context.Application; } }

        private CollectionWrapper SessionWrapper { get; set; }
        private CollectionWrapper ApplicationWrapper { get; set; }


        public void SetValue(string key, object val)
        {
            if (Session != null)
                SessionWrapper.SetValue(key, val);
            else
                ApplicationWrapper.SetValue(key, val);
        }

        public object GetValue(string key)
        {
            return SessionWrapper.GetValue(key) ?? ApplicationWrapper.GetValue(key);
        }

        public object GetValueAndRemove(string key)
        {
            return SessionWrapper.GetValueAndRemove(key) ?? ApplicationWrapper.GetValueAndRemove(key);
        }

        public void Remove(string key)
        {
            SessionWrapper.Remove(key);
            ApplicationWrapper.Remove(key);
        }

        public object this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public class CollectionWrapper
        {
            public CollectionWrapper(Func<string, string> makeInnerKey, Action<string, object> setValue,
                                     Func<string, object> getValue, Action<string> remove)
            {
                MakeInnerKey = makeInnerKey;
                SetValue_Impl = setValue;
                GetValue_Impl = getValue;
                Remove_Impl = remove;
            }

            private Func<string, string> MakeInnerKey { get; set; }

            private Action<string, object> SetValue_Impl { get; set; }
            private Func<string, object> GetValue_Impl { get; set; }
            private Action<string> Remove_Impl { get; set; }

            public void SetValue(string key, object val) { SetValue_Impl(MakeInnerKey(key), val); }
            public object GetValue(string key) { return GetValue_Impl(MakeInnerKey(key)); }
            public void Remove(string key) { Remove_Impl(MakeInnerKey(key)); }

            public object GetValueAndRemove(string key)
            {
                object val = GetValue(key);
                if (val != null)
                    Remove(key);
                return val;
            }
        }
    } 
}
