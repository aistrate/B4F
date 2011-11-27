using System;

namespace B4F.TotalGiro.Instruments
{
    [Flags()]
    public enum SecCategoryFilterOptions
    {
        All = 0,
        Securities = 1,
        Derivatives = 2,
        Cash = 4,
        Index = 8,
        Benchmark = 16,
        CorporateAction = 32
    }

    [Flags()]
    public enum SecCategoryTypes
    {
        Security = 1,
        Derivative = 2,
        Cash = 4,
        Index = 8,
        BenchMark = 16,
        CorporateAction = 32
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.SecCategory">SecCategory</see> class
    /// </summary>
    public interface ISecCategory
	{
		B4F.TotalGiro.Routes.IRoute DefaultRoute { get; set; }
		string Description { get; set; }
        bool IsSupported { get; }
        SecCategoryTypes SecCategoryType { get; }
        bool IsSecurity { get; }
        bool IsDerivative { get; }
		bool IsCash { get; set; }
		SecCategories Key { get; }
		string Name { get; set; }
	}
}
