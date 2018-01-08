﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebOrderEntry
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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="WebOrderEntry")]
	public partial class LocationDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public LocationDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["WebOrderEntryConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public LocationDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LocationDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LocationDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LocationDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<LOCATION_MASTER> LOCATION_MASTERs
		{
			get
			{
				return this.GetTable<LOCATION_MASTER>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.LOCATION_MASTER")]
	public partial class LOCATION_MASTER
	{
		
		private string _LOCATION_ID;
		
		private string _ADDRESS_LINE1;
		
		private string _ADDRESS_LINE2;
		
		private string _CITY;
		
		private string _STATE;
		
		private string _ZIP;
		
		public LOCATION_MASTER()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LOCATION_ID", DbType="VarChar(12) NOT NULL", CanBeNull=false)]
		public string LOCATION_ID
		{
			get
			{
				return this._LOCATION_ID;
			}
			set
			{
				if ((this._LOCATION_ID != value))
				{
					this._LOCATION_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ADDRESS_LINE1", DbType="VarChar(35)")]
		public string ADDRESS_LINE1
		{
			get
			{
				return this._ADDRESS_LINE1;
			}
			set
			{
				if ((this._ADDRESS_LINE1 != value))
				{
					this._ADDRESS_LINE1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ADDRESS_LINE2", DbType="VarChar(35)")]
		public string ADDRESS_LINE2
		{
			get
			{
				return this._ADDRESS_LINE2;
			}
			set
			{
				if ((this._ADDRESS_LINE2 != value))
				{
					this._ADDRESS_LINE2 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CITY", DbType="VarChar(30)")]
		public string CITY
		{
			get
			{
				return this._CITY;
			}
			set
			{
				if ((this._CITY != value))
				{
					this._CITY = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_STATE", DbType="VarChar(2)")]
		public string STATE
		{
			get
			{
				return this._STATE;
			}
			set
			{
				if ((this._STATE != value))
				{
					this._STATE = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ZIP", DbType="VarChar(12)")]
		public string ZIP
		{
			get
			{
				return this._ZIP;
			}
			set
			{
				if ((this._ZIP != value))
				{
					this._ZIP = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
