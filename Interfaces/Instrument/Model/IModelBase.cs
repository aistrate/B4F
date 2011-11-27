using System;
using System.Collections;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This enumeration lists the types of which a model can be constructed.
    /// A model can be constructed of instruments and of other models.
    /// </summary>
	public enum ModelComponentType
	{
		/// <summary>
		/// This model component is a model
		/// </summary>
        Model,
        /// <summary>
        /// This model component is an instrument
        /// </summary>
        Instrument
	}

    public enum ExecutionOnlyOptions
    {
        NotAllowed=0,
        Allowed,
        Always        
    }

    public enum ModelType
    {
        PortfiolioModel = 0,
        BenchMark
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Model">Model</see> class
    /// </summary>
    public interface IModelBase
	{
		int Key { get; set; }
		string ModelName { get; set; }
        string ShortName { get; set; }
        string Description { get; set; }
        string ModelNotes { get; set; }
		IModelVersion LatestVersion { get; set; }
        IAssetManager AssetManager { get; set; }
        string ToString();
        decimal TempBenchMark { get; set; }
        decimal BenchMarkValue { get; set; }
        decimal IBoxxTarget { get; set; }
        decimal MSCIWorldTarget { get; set; }
        decimal CompositeTarget { get; set; }
        decimal ExpectedReturn { get; set; }
        decimal StandardDeviation { get; set; }
        bool IsPublic { get; set; }
        bool IsActive { get; set; }
        bool IsSubModel { get; set; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
        string CreatedBy { get; set; }
        ModelType ModelType { get; }
        int GetHashCode();
        IModelVersionCollection ModelVersions { get; }
	}
}