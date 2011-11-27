using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.Dal;
using NHibernate.Expression;
using System.Text;

namespace B4F.TotalGiro.GUI
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


	/// <summary>
	/// Summary description for NHObjectSearchControl
	/// </summary>
	public class NHObjectSearchControl : WebControl, INamingContainer
	{

		#region variable declarations

		protected Type searchableType;
		protected StringWriter querybuffer = new StringWriter();
		List<ICriterion> expressions = new List<ICriterion>();
        private DataSet ds = new DataSet();

		#endregion

		#region Constructors

		public NHObjectSearchControl()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region Properties

		public Object SearchableObject
		{
			set { searchableType = value.GetType(); }
		}

		public Type SearchableType
		{
			get { return searchableType; }
			set { searchableType = value; }
		}


        public DataSet DataSet
        {
            get { return ds; }
            set { ds = value; }
        }
	

		#endregion

		#region protected methods

		protected void BuildExpression(string sName, string sValue)
		{
			if (sValue.Length == 0)
				return;

			int lastdot = sName.LastIndexOf('_');
			string sType = sName.Substring(lastdot + 1);
			sName = sName.Substring(0, lastdot);

			switch(sType)
			{
				case "String":
					expressions.Add(Expression.Like(sName, String.Concat("'%",sValue,"%'")));
					break;
				case "DateTime":
					expressions.Add(Expression.Gt(sName, Convert.ToDateTime(sValue)));
					break;
				case "Int16":
					expressions.Add(Expression.Eq(sName, sValue));
					break;
				case "Int32":
					goto case "Int16";
				case "Decimal":
					goto case "Int16";
				case "Boolean":
					expressions.Add(Expression.Eq(sName, Convert.ToBoolean(sValue)));
					break;
			}

		}
		#endregion

		#region overridable methods

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.Controls.Add(this.CreateControl(SearchableType));
		}
		#endregion

		#region public methods

		public void BuildQuery(ControlCollection controls)
		{
			foreach (Control control in controls)
			{
				if (control.HasControls())
				{
					BuildQuery(control.Controls);
				}
				else
				{
					switch (control.GetType().ToString())
					{
						case "System.Web.UI.WebControls.Calendar":
                            //if (querybuffer.ToString().Length > 0)
                            //    querybuffer.Write(",");
                            //BuildExpression(control.ID, ((Calendar)control).SelectedDate.ToString());
							break;
						case "System.Web.UI.WebControls.RadioButtonList":
							if (querybuffer.ToString().Length > 0)
								querybuffer.Write(",");
							BuildExpression(control.ID, ((RadioButtonList)control).SelectedValue);
							break;
						case "System.Web.UI.WebControls.TextBox":
							if (querybuffer.ToString().Length > 0)
								querybuffer.Write(",");
							BuildExpression(control.ID, ((TextBox)control).Text);
							break;
						case "System.Web.UI.WebControls.DropDownList":
							if (querybuffer.ToString().Length > 0)
								querybuffer.Write(",");
							BuildExpression(control.ID, ((DropDownList)control).Text);
							break;
						default:
							break;
					}
				}
			}
		}

		protected Control CreateControl(Type t)
		{
			PropertyInfo[] theProps = t.GetProperties();

			Array.Sort(theProps, new PropertyInfoComparer());

			HtmlTable table = new HtmlTable();
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cell = new HtmlTableCell();

			cell.InnerHtml = "Zoek " + t.ToString();
			cell.ColSpan = 2;
			row.Cells.Add(cell);
			table.Rows.Add(row);

			foreach (PropertyInfo propinfo in theProps)
			{

				row = new HtmlTableRow();
				cell = new HtmlTableCell();

				cell.InnerHtml = propinfo.Name;
				row.Cells.Add(cell);

				Control control = null;
				string ctrltype = propinfo.PropertyType.ToString();
				string checktype = ctrltype.Substring(ctrltype.LastIndexOf('.')+1);
				string ctrlvalue = "";

				if (!(checktype.Substring(0, 1) == "I" && Char.IsUpper(checktype.Substring(1, 1)[0])))
				{
					switch (checktype)
					{
						case "DateTime":
							control = new Calendar();
							((Calendar)control).SelectedDate = new DateTime(2000,6,24);
							break;
						case "Boolean":
							control = new RadioButtonList();
							ListItem liFalse = new ListItem("False", "false");
							ListItem liTrue = new ListItem("True", "true");
							((RadioButtonList)control).Items.Add(liFalse);
							((RadioButtonList)control).Items.Add(liTrue);
							((RadioButtonList)control).SelectedValue = ctrlvalue;
							break;
						case "String":
							control = new TextBox();
							if (ctrlvalue != null)
								((TextBox)control).Text = ctrlvalue;
							break;
						case "Int16":
							control = new TextBox();
							if (ctrlvalue != null)
								((TextBox)control).Text = ctrlvalue;
							break;
						case "Int32":
							control = new TextBox();
							if (ctrlvalue != null)
								((TextBox)control).Text = ctrlvalue;
							break;
						case "Decimal":
							control = new TextBox();
							if (ctrlvalue != null)
								((TextBox)control).Text = ctrlvalue;
							break;
						default:
							DataSet ds;
							try
							{
                                IDalSession session = NHSessionFactory.CreateSession();
                                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList((IList)session.GetList(propinfo.PropertyType), "Key,Key");
                                session.Close();
							}
							catch (Exception ex)
							{
								break;
							}
							control = new DropDownList();
							((DropDownList)control).DataSource = ds;
							((DropDownList)control).DataBind();
							if (ctrlvalue != null)
								((DropDownList)control).Text = ctrlvalue;
							break;
					}

					if (control != null)
					{
						control.EnableViewState = true;
						control.ID = String.Concat(propinfo.Name, "_", checktype);

						cell = new HtmlTableCell();
						cell.Controls.Add(control);
						row.Cells.Add(cell);

						// Add the control that indicates the AND/OR
						cell = new HtmlTableCell();

						DropDownList dropdownlist = new DropDownList();
						dropdownlist.ID = String.Concat("Operator_",propinfo.Name); ;
						dropdownlist.Items.Add("And");
						dropdownlist.Items.Add("Or");

						cell.Controls.Add(dropdownlist);

						
						row.Cells.Add(cell);

						table.Rows.Add(row);
					}
				}
			}

			return table;
		}

        public void MakeDs(string dataSetString)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                 this.GetList(session), dataSetString);
            session.Close();
            this.DataSet = ds;
        }

        //public DataSet GetDs(string dataSetString)
        //{
        //    DataSet ds = new DataSet();
        //    IDalSession session = NHSessionFactory.CreateSession();
        //    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
        //         this.GetList(session), dataSetString);
        //    session.Close();
        //    return ds;
        //}

        //private void GetList(out IList results, out string props)
        //{
        //    results = this.GetList();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("");

        //    Object obj = results[0];
        //    //foreach (object obj in results)
        //    //{
        //        PropertyInfo[] theProps = obj.GetType().GetProperties();
        //        foreach (PropertyInfo propinfo in theProps)
        //        {
        //            if (propinfo.Name.ToUpper().Contains("NAME"))
        //            {
        //                if (sb.Length > 0)
        //                {
        //                    sb.Append(",");
        //                }
        //                sb.Append(propinfo.Name.ToString());
        //            }
        //        }
        //    //}
        //    props = sb.ToString();
        //}

		public IList GetList()
		{
            IList results;
             
            IDalSession session = NHSessionFactory.CreateSession();
            results = session.GetList(searchableType, expressions);        
            session.Close();
            return results;
		}

        public IList GetList(IDalSession session)
        {
            IList results;
            results = session.GetList(searchableType, expressions);
            return results;
        }


		#endregion

	}
}