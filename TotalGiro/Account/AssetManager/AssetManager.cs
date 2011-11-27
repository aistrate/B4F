using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using B4F.TotalGiro.Dal;


namespace B4F.TotalGiro.Accounts
{
	public class AssetManager : IAssetManager 
	{
		protected AssetManager() {}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string BoSymbol
		{
			get { return boSymbol; }
			set { boSymbol = value; }
		}

		public virtual IClearingAccount ClearingAccount
		{
			get { return clearingAccount; }
			set { clearingAccount = value; }
		}	


		public override string ToString()
		{
			return this.Name.ToString();
		}

		public override int GetHashCode()
		{
			return key.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is AssetManager)
			{
				if (((AssetManager)obj).key == this.key)
					return true;
				else
					return false;
			}
			else
			{
				return false;
			}
		}

		#region Private Variables

		internal int key;
		private string name;
		private string boSymbol;
		private IClearingAccount clearingAccount;

		#endregion

		//public class DataAccessLayer : IDal
		//{
		//    public DataAccessLayer(IDalSession NhSession)
		//    {
		//        this.DalSession = NhSession;
		//    }

		//    public IDalSession DalSession
		//    {
		//        get { return this.nhsession; }
		//        set { nhsession = value; }
		//    }

		//    public AssetManager GetAssetManager(Int32 id)
		//    {
		//        return (AssetManager)DalSession.GetObjectInstance(typeof(AssetManager), id);
		//    }

		//    public IList GetAssetManagers()
		//    {
		//        return DalSession.GetList(typeof(AssetManager));
		//    }

		//    #region Private Variables

		//    private IDalSession nhsession;

		//    #endregion
		//}
	}
}
