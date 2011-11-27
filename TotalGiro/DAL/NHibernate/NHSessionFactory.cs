using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using B4F.TotalGiro.Dal.Auditing;
using NHibernate;
using NHibernate.Cfg;
using SysCfg = System.Configuration;

namespace B4F.TotalGiro.Dal
{
    /// <summary>
    /// Provides <i>NHibernate</i> sessions and the possibility to register additional classes with <i>NHibernate</i> by modules.
    /// </summary>
    public class NHSessionFactory : IDalSessionFactory
    {
        private static NHSessionFactory instance;

        private Configuration configuration;
        private ISessionFactory sessionFactory;

        private Dictionary<string, object> registeredAssemblies = new Dictionary<string, object>();
        private Dictionary<string, object> unregistrableAssemblies = new Dictionary<string, object>();

        private NHSessionFactory()
        {
            registerKnownAssemblies();
        }

        /// <summary>
        /// Gets the one instance of the <b>NHSessionFactory</b>. This is done with a Singleton so that we don't have
        /// to register mappings etc. with every request. It also registers the given assemblies with <i>NHibernate</i>.
        /// </summary>
        /// <param name="assemblyNames">Names of assemblies to register with <i>NHibernate</i>.</param>
        /// <returns>The one instance of <b>NHSessionFactory</b>.</returns>
        public static NHSessionFactory GetInstance(params string[] assemblyNames)
        {
            if (instance == null)
                instance = new NHSessionFactory();

            instance.registerAssemblies(assemblyNames);

            return instance;
        }

        /// <summary>
        /// Get a new <i>NHibernate</i> session.
        /// </summary>
        /// <returns>A new <i>NHibernate</i> session.</returns>
        public static IDalSession CreateSession()
        {
            return GetInstance().getSession();
        }

        /// <summary>
        /// Get a new <i>NHibernate</i> session.
        /// </summary>
        /// <returns>A new <i>NHibernate</i> session.</returns>
        IDalSession IDalSessionFactory.CreateSession()
        {
            return this.getSession();
        }

        private IDalSession getSession()
        {
            AuditLogInterceptor interceptor = new AuditLogInterceptor();
            ISession session = this.sessionFactory.OpenSession(interceptor);
            interceptor.Session = session;
            interceptor.UserName = B4F.TotalGiro.Security.SecurityManager.CurrentUser;

            return new NHSession(session);
        }

        #region Register Assemblies

        private void registerKnownAssemblies()
        {
            string[] knownAssemblies = (string[])SysCfg.ConfigurationManager.GetSection("B4FHibernate/AssembliesToHibernate");

            registerAssemblies(knownAssemblies);
        }

        private void registerAssemblies(string[] assemblyNames)
        {
            if (assemblyNames == null ||
                assemblyNames.Length == 0)
                return;

            bool registeredAny = false;

            foreach (string assemblyName in assemblyNames)
                registeredAny |= registerAssemblyTree(assemblyName);

            if (registeredAny)
                this.sessionFactory = Configuration.BuildSessionFactory();
        }

        private bool registerAssemblyTree(string assemblyName)
        {
            return registerAssemblyTree(Assembly.Load(assemblyName));
        }

        private bool registerAssemblyTree(Assembly assembly)
        {
            bool registeredAny = false;

            if (!this.registeredAssemblies.ContainsKey(assembly.FullName) &&
                !this.unregistrableAssemblies.ContainsKey(assembly.FullName))
            {
                Assembly[] referencedAssemblies = assembly.GetReferencedAssemblies()
                                                          .Select(name => Assembly.Load(name))
                                                          .Where(a => a != null &&
                                                                      !a.GlobalAssemblyCache)
                                                          .ToArray();

                foreach (Assembly referencedAssembly in referencedAssemblies)
                    registeredAny |= registerAssemblyTree(referencedAssembly);

                registeredAny |= registerAssembly(assembly);
            }

            return registeredAny;
        }

        private bool registerAssembly(Assembly assembly)
        {
            if (isAssemblyRegistrable(assembly))
            {
                Configuration.AddAssembly(assembly);
                this.registeredAssemblies.Add(assembly.FullName, null);
                return true;
            }
            else
            {
                this.unregistrableAssemblies.Add(assembly.FullName, null);
                return false;
            }
        }

        private bool isAssemblyRegistrable(Assembly assembly)
        {
            return assembly.GetManifestResourceNames()
                           .Any(resourceName => resourceName.EndsWith(".hbm.xml"));
        }

        #endregion

        #region Configuration

        private Configuration Configuration
        {
            get
            {
                if (this.configuration == null ||
                    this.configuration.ClassMappings.Count == 0)
                {
                    this.configuration = new Configuration();

                    this.configuration.Configure(getConfigFilePath());
                }

                return this.configuration;
            }
        }

        private string getConfigFilePath()
        {
            string[] pathCandidates = new string[]
            {
                SysCfg.ConfigurationManager.AppSettings.Get("ConfigPath"),

                getExecutingAssemblyLocation() + @"\hibernate.cfg.xml",
            };

            string path = pathCandidates.Where(p => !string.IsNullOrEmpty(p))
                                        .Where(p => new FileInfo(p).Exists)
                                        .FirstOrDefault();

            if (path == null)
                throw new ApplicationException(
                    string.Format("The hibernate.cfg.xml file could not be found at any of the following locations:{0}{1}",
                                  System.Environment.NewLine,
                                  string.Join(System.Environment.NewLine, pathCandidates)));

            return path;
        }

        private string getExecutingAssemblyLocation()
        {
            string path = Assembly.GetExecutingAssembly().Location;

            return path.Substring(0, path.LastIndexOf(@"\"));
        }

        #endregion
    }
}
