using System;
using System.Reflection;
using System.Collections;
using System.Text;

namespace TotalGiroGUI
{
	class PropertyInfoComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			PropertyInfo a, b;

			a = x as PropertyInfo;
			b = y as PropertyInfo;

			return a.Name.CompareTo(b.Name);
		}
	}

}
