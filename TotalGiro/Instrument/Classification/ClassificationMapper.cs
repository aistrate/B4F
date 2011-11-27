using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Instruments.Classification
{
    public class ClassificationMapper
    {
        public static IList GetAssetClasses(IDalSession session)
        {
            return session.GetList(typeof(AssetClass));
        }
        public static IList GetRegionClasses(IDalSession session)
        {
            return session.GetList(typeof(RegionClass));
        }
        public static IList GetSectorClasses(IDalSession session)
        {
            return session.GetList(typeof(SectorClass));
        }
        public static IList GetInstrumentsCategories(IDalSession session)
        {
            return session.GetList(typeof(InstrumentsCategories));
        }

        public static IAssetClass GetAssetClass(IDalSession session, int assetClassId)
        {
            return session.GetTypedList<AssetClass, IAssetClass>(assetClassId).FirstOrDefault();
        }
        public static IRegionClass GetRegionClass(IDalSession session, int regionClassId)
        {
            return session.GetTypedList<RegionClass, IRegionClass>(regionClassId).FirstOrDefault();
        }
        public static ISectorClass GetSectorClass(IDalSession session, int sectorClassId)
        {
            return session.GetTypedList<SectorClass, ISectorClass>(sectorClassId).FirstOrDefault();
        }
        public static IInstrumentsCategories GetInstrumentsCategory(IDalSession session, int instrumentsCategoryId)
        {
            return session.GetTypedList<InstrumentsCategories, IInstrumentsCategories>(instrumentsCategoryId).FirstOrDefault();
        }

        public static IInstrumentsCategories GetDefaultInstrumentsCategory(IDalSession session)
        {
            IInstrumentsCategories category = null;
            string hql = "from InstrumentsCategories I where I.IsDefault = 1";
            IList<IInstrumentsCategories> list = session.GetTypedListByHQL<IInstrumentsCategories>(hql);
            if (list != null && list.Count == 1)
            {
                category = list[0];
            }
            return category;
        }
    }
}
