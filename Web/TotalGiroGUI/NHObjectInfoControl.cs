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

namespace B4F.TotalGiro.GUI
{

	/// <summary>
	/// Summary description for NHObjectSearchControl
	/// </summary>
	public class NHObjectInfoControl : WebControl, INamingContainer
	{

		#region variable declarations

		protected Object infoObject;
		protected int maxdepth = 2;

		#endregion

		#region Constructors

		public NHObjectInfoControl()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region Properties

		public Object InfoObject
		{
			get { return infoObject; }
			set { infoObject = value; }
		}

		public int MaxDepth
		{
			get { return maxdepth; }
			set { maxdepth = value; }
		}

		#endregion

		#region protected methods

		#endregion

		#region overridable methods

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.Controls.Add(this.CreateControl(this.InfoObject, 0));
		}
		#endregion

		#region public methods


		protected Control CreateControl(Object p_infoobject, int level)
		{
			Type objectType = p_infoobject.GetType();

			PropertyInfo[] theProps = objectType.GetProperties();

			Array.Sort(theProps, new PropertyInfoComparer());

			HtmlTable table = new HtmlTable();
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cell = new HtmlTableCell();

			cell.InnerHtml = String.Concat("Object: ",InfoObject.ToString(), ", Type:", objectType.ToString());
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
				string checktype = ctrltype.Substring(ctrltype.LastIndexOf('.') + 1);

                if (checktype != "Object")
                {
                    Object propvalue = propinfo.GetValue(infoObject, null);
                    string ctrlvalue = "";
                    if (propvalue != null)
                        ctrlvalue = propvalue.ToString();


                    switch (checktype)
                    {
                        case "DateTime":
                            control = new Calendar();
                            ((Calendar)control).SelectedDate = new DateTime(2000, 6, 24);
                            break;
                        case "Boolean":
                            control = new RadioButtonList();
                            ListItem liFalse = new ListItem("False", "false");
                            ListItem liTrue = new ListItem("True", "true");
                            ((RadioButtonList)control).Items.Add(liFalse);
                            ((RadioButtonList)control).Items.Add(liTrue);
                            ((RadioButtonList)control).SelectedValue = ctrlvalue;
                            ((RadioButtonList)control).RepeatDirection = RepeatDirection.Horizontal;
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
                            control = new DropDownList();

                            try
                            {
                                IDalSession session = NHSessionFactory.CreateSession();

                                if (propinfo.PropertyType == typeof(B4F.TotalGiro.Instruments.IInstrumentExchangeCollection))
                                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList((IList)session.GetList(typeof(B4F.TotalGiro.Instruments.IExchange)), "Key,ExchangeName");
                                else
                                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList((IList)session.GetList(propinfo.PropertyType), "Key,Key");


                                session.Close();
                            }
                            catch (Exception ex)
                            {
                                ((DropDownList)control).Text = ctrlvalue;
                                break;
                            }

                            if (ds != null && ds.Tables.Count > 0)
                            {
                                ((DropDownList)control).DataSource = ds.Tables[0].DefaultView;
                                ((DropDownList)control).DataBind();
                            }
                            if (ctrlvalue != null)
                                ((DropDownList)control).Text = ctrlvalue;
                            break;
                    }

                }

                if (control != null)
                {
                    control.EnableViewState = true;
                    control.ID = String.Concat(propinfo.Name, "_", checktype);

                    cell = new HtmlTableCell();
                    cell.Controls.Add(control);
                    row.Cells.Add(cell);

                    table.Rows.Add(row);
                }
			}

			return table;
		}

        protected DataTable TestGetData()
        {
            DataTable dtAggrOrders;
            DataRow dr;

            dtAggrOrders = new DataTable();

            DataColumn ISIN = new DataColumn();
            ISIN.DataType = System.Type.GetType("System.String");
            ISIN.ColumnName = "ISIN";
            dtAggrOrders.Columns.Add(ISIN);

            dtAggrOrders.Columns.Add(new DataColumn("name", typeof(string)));
            dtAggrOrders.Columns.Add(new DataColumn("company", typeof(string)));
            dtAggrOrders.Columns.Add(new DataColumn("valuta", typeof(string)));
            dtAggrOrders.Columns.Add(new DataColumn("size", typeof(decimal)));
            dtAggrOrders.Columns.Add(new DataColumn("amount", typeof(decimal)));
            dtAggrOrders.Columns.Add(new DataColumn("ordernr", typeof(int)));

            dr = dtAggrOrders.NewRow();

            dr["ISIN"] = "NL0000442077";
            dr["name"] = "Delta Lloyd Mix Fonds";
            dr["company"] = "Delta Lloyd";
            dr["valuta"] = "EUR";
            dr["size"] = "11172,34";
            dr["amount"] = "3019944,66";
            dr["ordernr"] = "1234";
            dtAggrOrders.Rows.Add(dr);

            dr = dtAggrOrders.NewRow();
            dr["ISIN"] = "NL0000280246";
            dr["name"] = "FBTO Euro Mixfonds (NL)";
            dr["company"] = "FBTO";
            dr["valuta"] = "EUR";
            dr["size"] = "302167,22";
            dr["amount"] = "7777777,66";
            dr["ordernr"] = "5678";
            dtAggrOrders.Rows.Add(dr);

            dr = dtAggrOrders.NewRow();
            dr["ISIN"] = "NL0000286243";
            dr["name"] = "Generali Mix Fund (NL)";
            dr["company"] = "Generali";
            dr["valuta"] = "EUR";
            dr["size"] = "923,99";
            dr["amount"] = "92399,00";
            dr["ordernr"] = "19458";
            dtAggrOrders.Rows.Add(dr);

            dr = dtAggrOrders.NewRow();
            dr["ISIN"] = "NL0000287100";
            dr["name"] = "Optimix Mix Fund (NL) ";
            dr["company"] = "Optimix";
            dr["valuta"] = "EUR";
            dr["size"] = "17642,889";
            dr["amount"] = "299940087,29";
            dr["ordernr"] = "206635";
            dtAggrOrders.Rows.Add(dr);

            dr = dtAggrOrders.NewRow();
            dr["ISIN"] = "NL0000288660";
            dr["name"] = "Robeco Growth Mix (NL)  ";
            dr["company"] = "Robeco";
            dr["valuta"] = "EUR";
            dr["size"] = "4677,829";
            dr["amount"] = "2290833,31";
            dr["ordernr"] = "206634";
            dtAggrOrders.Rows.Add(dr);

            dr = dtAggrOrders.NewRow();
            dr["ISIN"] = "NL0000288629";
            dr["name"] = "Robeco Safe Mix (NL) ";
            dr["company"] = "Robeco";
            dr["valuta"] = "EUR";
            dr["size"] = "39956,003";
            dr["amount"] = "295644219,31";
            dr["ordernr"] = "206632";
            dtAggrOrders.Rows.Add(dr);

            return dtAggrOrders;
        }

		#endregion

	}
}