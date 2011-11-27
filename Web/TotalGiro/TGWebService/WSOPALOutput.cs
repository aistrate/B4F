// Copyright 2004, Microsoft Corporation
// Sample Code - Use restricted to terms of use defined in the accompanying license agreement (EULA.doc)

//--------------------------------------------------------------
// Autogenerated by XSDObjectGen version 1.4.4.0
// Schema file: opalimport.xsd
// Creation Date: 4/2/2007 11:45:05 AM
//--------------------------------------------------------------

using System;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;

namespace B4F.Web.TGWebService
{

	public struct Declarations
	{
		public const string SchemaVersion = "";
	}

	[Serializable]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class CashflowCollection : ArrayList
	{
		public B4F.Web.TGWebService.Cashflow Add(B4F.Web.TGWebService.Cashflow obj)
		{
			base.Add(obj);
			return obj;
		}

		public B4F.Web.TGWebService.Cashflow Add()
		{
			return Add(new B4F.Web.TGWebService.Cashflow());
		}

		public void Insert(int index, B4F.Web.TGWebService.Cashflow obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(B4F.Web.TGWebService.Cashflow obj)
		{
			base.Remove(obj);
		}

		new public B4F.Web.TGWebService.Cashflow this[int index]
		{
			get { return (B4F.Web.TGWebService.Cashflow) base[index]; }
			set { base[index] = value; }
		}
	}

	[Serializable]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class QuestionAnswerCollection : ArrayList
	{
		public string Add(string obj)
		{
			base.Add(obj);
			return obj;
		}

		public void Insert(int index, string obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(string obj)
		{
			base.Remove(obj);
		}

		new public string this[int index]
		{
			get { return (string) base[index]; }
			set { base[index] = value; }
		}
	}



	[XmlType(TypeName="Cashflow"),Serializable]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class Cashflow
	{

		[XmlElement(ElementName="Description",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Description;
		
		[XmlIgnore]
		public string Description
		{ 
			get { return __Description; }
			set { __Description = value; }
		}

		[XmlElement(ElementName="Indexation",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Indexation;
		
		[XmlIgnore]
		public string Indexation
		{ 
			get { return __Indexation; }
			set { __Indexation = value; }
		}

		[XmlElement(ElementName="Type",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Type;
		
		[XmlIgnore]
		public string Type
		{ 
			get { return __Type; }
			set { __Type = value; }
		}

		[XmlElement(ElementName="Value",IsNullable=false,DataType="double")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public double __Value;
		
		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool __ValueSpecified;
		
		[XmlIgnore]
		public double @Value
		{ 
			get { return __Value; }
			set { __Value = value; __ValueSpecified = true; }
		}

		[XmlElement(ElementName="StartPeriod",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __StartPeriod;
		
		[XmlIgnore]
		public string StartPeriod
		{ 
			get { return __StartPeriod; }
			set { __StartPeriod = value; }
		}

		[XmlElement(ElementName="EndPeriod",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __EndPeriod;
		
		[XmlIgnore]
		public string EndPeriod
		{ 
			get { return __EndPeriod; }
			set { __EndPeriod = value; }
		}

		public Cashflow()
		{
		}
	}


	[XmlRoot(ElementName="Project",IsNullable=false),Serializable]
	public class Project
	{

		[XmlElement(ElementName="Version",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Version;
		
		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool __VersionSpecified;
		
		[XmlIgnore]
		public string Version
		{ 
			get { return __Version; }
			set { __Version = value; __VersionSpecified = true; }
		}

		[XmlElement(ElementName="ProjectName",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ProjectName;
		
		[XmlIgnore]
		public string ProjectName
		{ 
			get { return __ProjectName; }
			set { __ProjectName = value; }
		}

		[XmlElement(Type=typeof(ClientData),ElementName="ClientData",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ClientData __ClientData;
		
		[XmlIgnore]
		public ClientData ClientData
		{
			get
			{
				if (__ClientData == null) __ClientData = new ClientData();		
				return __ClientData;
			}
			set {__ClientData = value;}
		}

		[XmlElement(Type=typeof(Emotion),ElementName="Emotion",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Emotion __Emotion;
		
		[XmlIgnore]
		public Emotion Emotion
		{
			get
			{
				if (__Emotion == null) __Emotion = new Emotion();		
				return __Emotion;
			}
			set {__Emotion = value;}
		}

		[XmlElement(ElementName="CapitalStartValue",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __CapitalStartValue;
		
		[XmlIgnore]
		public string CapitalStartValue
		{ 
			get { return __CapitalStartValue; }
			set { __CapitalStartValue = value; }
		}

		[XmlElement(Type=typeof(CashFlows),ElementName="CashFlows",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public CashFlows __CashFlows;
		
		[XmlIgnore]
		public CashFlows CashFlows
		{
			get
			{
				if (__CashFlows == null) __CashFlows = new CashFlows();		
				return __CashFlows;
			}
			set {__CashFlows = value;}
		}

		[XmlElement(Type=typeof(TaxSettings),ElementName="TaxSettings",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public TaxSettings __TaxSettings;
		
		[XmlIgnore]
		public TaxSettings TaxSettings
		{
			get
			{
				if (__TaxSettings == null) __TaxSettings = new TaxSettings();		
				return __TaxSettings;
			}
			set {__TaxSettings = value;}
		}

		[XmlElement(Type=typeof(Objectives),ElementName="Objectives",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Objectives __Objectives;
		
		[XmlIgnore]
		public Objectives Objectives
		{
			get
			{
				if (__Objectives == null) __Objectives = new Objectives();		
				return __Objectives;
			}
			set {__Objectives = value;}
		}

		public Project()
		{
		}
	}


	[XmlType(TypeName="ClientData"),Serializable]
	public class ClientData
	{

		[XmlElement(ElementName="ClientType",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientType;
		
		[XmlIgnore]
		public string ClientType
		{ 
			get { return __ClientType; }
			set { __ClientType = value; }
		}

		[XmlElement(ElementName="ClientCode",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientCode;
		
		[XmlIgnore]
		public string ClientCode
		{ 
			get { return __ClientCode; }
			set { __ClientCode = value; }
		}

		[XmlElement(ElementName="ClientName",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientName;
		
		[XmlIgnore]
		public string ClientName
		{ 
			get { return __ClientName; }
			set { __ClientName = value; }
		}

		[XmlElement(ElementName="ClientAddress",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientAddress;
		
		[XmlIgnore]
		public string ClientAddress
		{ 
			get { return __ClientAddress; }
			set { __ClientAddress = value; }
		}

		[XmlElement(ElementName="ClientZipCode",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientZipCode;
		
		[XmlIgnore]
		public string ClientZipCode
		{ 
			get { return __ClientZipCode; }
			set { __ClientZipCode = value; }
		}

		[XmlElement(ElementName="ClientCity",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientCity;
		
		[XmlIgnore]
		public string ClientCity
		{ 
			get { return __ClientCity; }
			set { __ClientCity = value; }
		}

		[XmlElement(ElementName="ClientPhoneNumber",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientPhoneNumber;
		
		[XmlIgnore]
		public string ClientPhoneNumber
		{ 
			get { return __ClientPhoneNumber; }
			set { __ClientPhoneNumber = value; }
		}

		[XmlElement(ElementName="ClientGenderType",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientGenderType;
		
		[XmlIgnore]
		public string ClientGenderType
		{ 
			get { return __ClientGenderType; }
			set { __ClientGenderType = value; }
		}

		[XmlElement(ElementName="ClientDOB",IsNullable=false,DataType="date")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DateTime __ClientDOB;
		
		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool __ClientDOBSpecified;
		
		[XmlIgnore]
		public DateTime ClientDOB
		{ 
			get { return __ClientDOB; }
			set { __ClientDOB = value; __ClientDOBSpecified = true; }
		}
		
		[XmlIgnore]
		public DateTime ClientDOBUtc
		{ 
			get { return __ClientDOB.ToUniversalTime(); }
			set { __ClientDOB = value.ToLocalTime(); __ClientDOBSpecified = true; }
		}

		[XmlElement(ElementName="ClientMatrimonyType",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __ClientMatrimonyType;
		
		[XmlIgnore]
		public string ClientMatrimonyType
		{ 
			get { return __ClientMatrimonyType; }
			set { __ClientMatrimonyType = value; }
		}

		[XmlElement(ElementName="PartnerName",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __PartnerName;
		
		[XmlIgnore]
		public string PartnerName
		{ 
			get { return __PartnerName; }
			set { __PartnerName = value; }
		}

		[XmlElement(ElementName="PartnerDOB",IsNullable=false,DataType="date")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DateTime __PartnerDOB;
		
		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool __PartnerDOBSpecified;
		
		[XmlIgnore]
		public DateTime PartnerDOB
		{ 
			get { return __PartnerDOB; }
			set { __PartnerDOB = value; __PartnerDOBSpecified = true; }
		}
		
		[XmlIgnore]
		public DateTime PartnerDOBUtc
		{ 
			get { return __PartnerDOB.ToUniversalTime(); }
			set { __PartnerDOB = value.ToLocalTime(); __PartnerDOBSpecified = true; }
		}

		[XmlElement(ElementName="PartnerGenderType",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __PartnerGenderType;
		
		[XmlIgnore]
		public string PartnerGenderType
		{ 
			get { return __PartnerGenderType; }
			set { __PartnerGenderType = value; }
		}

		public ClientData()
		{
			__ClientDOB = DateTime.Now;
			__PartnerDOB = DateTime.Now;
		}
	}


	[XmlType(TypeName="Emotion"),Serializable]
	public class Emotion
	{

		[XmlElement(Type=typeof(QuestionsAnswers),ElementName="QuestionsAnswers",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public QuestionsAnswers __QuestionsAnswers;
		
		[XmlIgnore]
		public QuestionsAnswers QuestionsAnswers
		{
			get
			{
				if (__QuestionsAnswers == null) __QuestionsAnswers = new QuestionsAnswers();		
				return __QuestionsAnswers;
			}
			set {__QuestionsAnswers = value;}
		}

		public Emotion()
		{
		}
	}


	[XmlType(TypeName="QuestionsAnswers"),Serializable]
	public class QuestionsAnswers
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return QuestionAnswerCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return QuestionAnswerCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get { return (string) QuestionAnswerCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return QuestionAnswerCollection.Count; }
        }

        public void Clear()
		{
			QuestionAnswerCollection.Clear();
        }

		public string Remove(int index) 
		{ 
            string obj = QuestionAnswerCollection[index];
            QuestionAnswerCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            QuestionAnswerCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(string),ElementName="QuestionAnswer",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public QuestionAnswerCollection __QuestionAnswerCollection;
		
		[XmlIgnore]
		public QuestionAnswerCollection QuestionAnswerCollection
		{
			get
			{
				if (__QuestionAnswerCollection == null) __QuestionAnswerCollection = new QuestionAnswerCollection();
				return __QuestionAnswerCollection;
			}
			set {__QuestionAnswerCollection = value;}
		}

		public QuestionsAnswers()
		{
		}
	}


	[XmlType(TypeName="CashFlows"),Serializable]
	public class CashFlows
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return CashFlowCollection.GetEnumerator();
		}

		public B4F.Web.TGWebService.Cashflow Add(B4F.Web.TGWebService.Cashflow obj)
		{
			return CashFlowCollection.Add(obj);
		}

		[XmlIgnore]
		public B4F.Web.TGWebService.Cashflow this[int index]
		{
			get { return (B4F.Web.TGWebService.Cashflow) CashFlowCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return CashFlowCollection.Count; }
        }

        public void Clear()
		{
			CashFlowCollection.Clear();
        }

		public B4F.Web.TGWebService.Cashflow Remove(int index) 
		{ 
            B4F.Web.TGWebService.Cashflow obj = CashFlowCollection[index];
            CashFlowCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            CashFlowCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(B4F.Web.TGWebService.Cashflow),ElementName="CashFlow",IsNullable=false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public CashflowCollection __CashFlowCollection;
		
		[XmlIgnore]
		public CashflowCollection CashFlowCollection
		{
			get
			{
				if (__CashFlowCollection == null) __CashFlowCollection = new CashflowCollection();
				return __CashFlowCollection;
			}
			set {__CashFlowCollection = value;}
		}

		public CashFlows()
		{
		}
	}


	[XmlType(TypeName="TaxSettings"),Serializable]
	public class TaxSettings
	{

		[XmlElement(ElementName="UseTax",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __UseTax;
		
		[XmlIgnore]
		public string UseTax
		{ 
			get { return __UseTax; }
			set { __UseTax = value; }
		}

		public TaxSettings()
		{
		}
	}


	[XmlType(TypeName="Objectives"),Serializable]
	public class Objectives
	{

		[XmlElement(ElementName="TargetCriterion",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __TargetCriterion;
		
		[XmlIgnore]
		public string TargetCriterion
		{ 
			get { return __TargetCriterion; }
			set { __TargetCriterion = value; }
		}

		[XmlElement(ElementName="TargetYear",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __TargetYear;
		
		[XmlIgnore]
		public string TargetYear
		{ 
			get { return __TargetYear; }
			set { __TargetYear = value; }
		}

		[XmlElement(ElementName="CapitalTargetValue",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __CapitalTargetValue;
		
		[XmlIgnore]
		public string CapitalTargetValue
		{ 
			get { return __CapitalTargetValue; }
			set { __CapitalTargetValue = value; }
		}

		[XmlElement(ElementName="RelativeTargetValue",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __RelativeTargetValue;
		
		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool __RelativeTargetValueSpecified;
		
		[XmlIgnore]
		public string RelativeTargetValue
		{ 
			get { return __RelativeTargetValue; }
			set { __RelativeTargetValue = value; __RelativeTargetValueSpecified = true; }
		}

		[XmlElement(ElementName="PerformanceTargetValue",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __PerformanceTargetValue;
		
		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool __PerformanceTargetValueSpecified;
		
		[XmlIgnore]
		public string PerformanceTargetValue
		{ 
			get { return __PerformanceTargetValue; }
			set { __PerformanceTargetValue = value; __PerformanceTargetValueSpecified = true; }
		}

		[XmlElement(ElementName="InflationType",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __InflationType;
		
		[XmlIgnore]
		public string InflationType
		{ 
			get { return __InflationType; }
			set { __InflationType = value; }
		}

		[XmlElement(ElementName="PersonalObjectiveDescription",IsNullable=false,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __PersonalObjectiveDescription;
		
		[XmlIgnore]
		public string PersonalObjectiveDescription
		{ 
			get { return __PersonalObjectiveDescription; }
			set { __PersonalObjectiveDescription = value; }
		}

		public Objectives()
		{
		}
	}
}