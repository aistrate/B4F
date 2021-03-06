﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace B4F.TotalGiro.LINQ.BOAccounts
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[System.Data.Linq.Mapping.DatabaseAttribute(Name="Cleopatra")]
	public partial class CleopatraDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public CleopatraDataContext() : 
				base(global::B4F.TotalGiro.LINQ.BOAccounts.Properties.Settings.Default.CleopatraConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public CleopatraDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CleopatraDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CleopatraDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CleopatraDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<BADGE> BADGEs
		{
			get
			{
				return this.GetTable<BADGE>();
			}
		}
	}
	
	[Table(Name="dbo.BADGES")]
	public partial class BADGE
	{
		
		private string _ACCNUMB;
		
		private string _ACCTYPE;
		
		private string _ACCSUB;
		
		private string _NAAMKORT;
		
		private string _CUSTGRP;
		
		private System.Nullable<char> _BALANCYN;
		
		private string _REKNUM;
		
		private System.Nullable<char> _CLOSEACC;
		
		private System.Nullable<char> _SOORTREL;
		
		private string _SCFEEFLB;
		
		private System.Nullable<char> _PFMFEE;
		
		private System.Nullable<char> _BTWCODE;
		
		private string _INPUSER;
		
		private System.Nullable<System.DateTime> _INPDATE;
		
		private string _INPTIME;
		
		public BADGE()
		{
		}
		
		[Column(Storage="_ACCNUMB", DbType="NVarChar(3)")]
		public string ACCNUMB
		{
			get
			{
				return this._ACCNUMB;
			}
			set
			{
				if ((this._ACCNUMB != value))
				{
					this._ACCNUMB = value;
				}
			}
		}
		
		[Column(Storage="_ACCTYPE", DbType="NVarChar(2)")]
		public string ACCTYPE
		{
			get
			{
				return this._ACCTYPE;
			}
			set
			{
				if ((this._ACCTYPE != value))
				{
					this._ACCTYPE = value;
				}
			}
		}
		
		[Column(Storage="_ACCSUB", DbType="NVarChar(12)")]
		public string ACCSUB
		{
			get
			{
				return this._ACCSUB;
			}
			set
			{
				if ((this._ACCSUB != value))
				{
					this._ACCSUB = value;
				}
			}
		}
		
		[Column(Storage="_NAAMKORT", DbType="NVarChar(20)")]
		public string NAAMKORT
		{
			get
			{
				return this._NAAMKORT;
			}
			set
			{
				if ((this._NAAMKORT != value))
				{
					this._NAAMKORT = value;
				}
			}
		}
		
		[Column(Storage="_CUSTGRP", DbType="NVarChar(3)")]
		public string CUSTGRP
		{
			get
			{
				return this._CUSTGRP;
			}
			set
			{
				if ((this._CUSTGRP != value))
				{
					this._CUSTGRP = value;
				}
			}
		}
		
		[Column(Storage="_BALANCYN", DbType="NVarChar(1)")]
		public System.Nullable<char> BALANCYN
		{
			get
			{
				return this._BALANCYN;
			}
			set
			{
				if ((this._BALANCYN != value))
				{
					this._BALANCYN = value;
				}
			}
		}
		
		[Column(Storage="_REKNUM", DbType="NVarChar(4)")]
		public string REKNUM
		{
			get
			{
				return this._REKNUM;
			}
			set
			{
				if ((this._REKNUM != value))
				{
					this._REKNUM = value;
				}
			}
		}
		
		[Column(Storage="_CLOSEACC", DbType="NVarChar(1)")]
		public System.Nullable<char> CLOSEACC
		{
			get
			{
				return this._CLOSEACC;
			}
			set
			{
				if ((this._CLOSEACC != value))
				{
					this._CLOSEACC = value;
				}
			}
		}
		
		[Column(Storage="_SOORTREL", DbType="NVarChar(1)")]
		public System.Nullable<char> SOORTREL
		{
			get
			{
				return this._SOORTREL;
			}
			set
			{
				if ((this._SOORTREL != value))
				{
					this._SOORTREL = value;
				}
			}
		}
		
		[Column(Storage="_SCFEEFLB", DbType="NVarChar(2)")]
		public string SCFEEFLB
		{
			get
			{
				return this._SCFEEFLB;
			}
			set
			{
				if ((this._SCFEEFLB != value))
				{
					this._SCFEEFLB = value;
				}
			}
		}
		
		[Column(Storage="_PFMFEE", DbType="NVarChar(1)")]
		public System.Nullable<char> PFMFEE
		{
			get
			{
				return this._PFMFEE;
			}
			set
			{
				if ((this._PFMFEE != value))
				{
					this._PFMFEE = value;
				}
			}
		}
		
		[Column(Storage="_BTWCODE", DbType="NVarChar(1)")]
		public System.Nullable<char> BTWCODE
		{
			get
			{
				return this._BTWCODE;
			}
			set
			{
				if ((this._BTWCODE != value))
				{
					this._BTWCODE = value;
				}
			}
		}
		
		[Column(Storage="_INPUSER", DbType="NVarChar(4)")]
		public string INPUSER
		{
			get
			{
				return this._INPUSER;
			}
			set
			{
				if ((this._INPUSER != value))
				{
					this._INPUSER = value;
				}
			}
		}
		
		[Column(Storage="_INPDATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> INPDATE
		{
			get
			{
				return this._INPDATE;
			}
			set
			{
				if ((this._INPDATE != value))
				{
					this._INPDATE = value;
				}
			}
		}
		
		[Column(Storage="_INPTIME", DbType="NVarChar(8)")]
		public string INPTIME
		{
			get
			{
				return this._INPTIME;
			}
			set
			{
				if ((this._INPTIME != value))
				{
					this._INPTIME = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
