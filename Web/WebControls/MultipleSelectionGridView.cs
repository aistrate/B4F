using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

namespace B4F.Web.WebControls
{
    /// <summary>
    /// A GridView which allows multiple-record selection by using a CheckBox on every row.
    /// </summary>
    [ToolboxData("<{0}:MultipleSelectionGridView runat=server></{0}:MultipleSelectionGridView>")]
    [Designer("System.Web.UI.Design.WebControls.GridViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class MultipleSelectionGridView : GridView
    {
        /// <summary>
        /// Indicates whether multiple-record selection is enabled.
        /// </summary>
        [Description("Indicates whether multiple-record selection should be enabled."),
         DefaultValue(true),
         Category("Behavior")]
        public bool MultipleSelection
        {
            get
            {
                object b = ViewState["MultipleSelection"];
                return (b == null ? true : (bool)b);
            }
            set { ViewState["MultipleSelection"] = value; }
        }

        /// <summary>
        /// Gets the id's of all selected records.
        /// </summary>
        [Browsable(false)]
        public int[] GetSelectedIds()
        {
            if (MultipleSelection)
                return SelectionBuffer.GetSelectedIds();
            else
                return new int[] { };
        }

        /// <summary>
        /// Selects the records whose id's are passed in.
        /// </summary>
        [Browsable(false)]
        public void SetSelectedIds(int[] data)
        {
            SelectionBuffer.CopyFrom(data);
        }

        /// <summary>
        /// Unselects all records in all pages.
        /// </summary>
        public void ClearSelection()
        {
            SelectAllChecked = false;
            SelectAllIds(false);
        }

        /// <summary>
        /// Selects/unselects all records on all pages.
        /// </summary>
        /// <param name="selected">Whether all records will be selected (true) or unselected (false).</param>
        public void SelectAllIds(bool selected)
        {
            foreach (GridViewRow row in Rows)
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                    chkSelect.Checked = (selected && chkSelect.Visible && chkSelect.Enabled);
                }

            if (selected)
                GetData().Select(CreateDataSourceSelectArguments(), new DataSourceViewSelectCallback(copyAllIdsToSelectionBuffer));
            else
                SelectionBuffer.Clear();
        }

        /// <summary>
        /// Indicates whether the Select All checkbox (on the header row) should be checked on.
        /// </summary>
        [Browsable(false), DefaultValue(false)]
        public bool SelectAllChecked
        {
            get { return (SelectAllCheckBox != null ? SelectAllCheckBox.Checked : false); }
            set { setSelectAll(value); }
        }

        [Browsable(false)]
        public int DataRowCount
        {
            get
            {
                object i = ViewState["DataRowCount"];
                return (i == null) ? 0 : (int)i;
            }
            private set { ViewState["DataRowCount"] = value; }
        }

        [Description("The name of a boolean field (from the data source) whose value will decide whether the selection check box on the current row is visible."),
         DefaultValue(""),
         Category("Behavior")]
        public string SelectionBoxVisibleBy
        {
            get
            {
                object s = ViewState["SelectionBoxVisibleBy"];
                return (s == null ? "" : (string)s);
            }
            set { ViewState["SelectionBoxVisibleBy"] = value; }
        }

        [Description("The name of a boolean field (from the data source) whose value will decide whether the selection check box on the current row is enabled."),
         DefaultValue(""),
         Category("Behavior")]
        public string SelectionBoxEnabledBy
        {
            get
            {
                object s = ViewState["SelectionBoxEnabledBy"];
                return (s == null ? "" : (string)s);
            }
            set { ViewState["SelectionBoxEnabledBy"] = value; }
        }

        protected SelectionBuffer SelectionBuffer
        {
            get
            {
                string uniqueName = getUniqueName("selectionBuffer");

                if (Page.Session[uniqueName] == null)
                    Page.Session[uniqueName] = new SelectionBuffer();

                return (SelectionBuffer)Page.Session[uniqueName];
            }
        }

        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            base.InitializeRow(row, fields);

            if (!DesignMode)
            {
                if (row.RowType == DataControlRowType.Header || row.RowType == DataControlRowType.DataRow)
                    createCheckBox(row);

                if (row.RowType == DataControlRowType.Header)
                    createOldSelectAllHiddenField(row);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!DesignMode)
            {
                if (!Page.IsPostBack)
                    SelectionBuffer.Clear();

                TemplateField templateField = new TemplateField();
                Columns.Insert(0, templateField);
            }

            base.OnInit(e);
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            if (!DesignMode)
                persistSelection();

            base.OnPagePreLoad(sender, e);
        }
        
        protected override void OnDataBinding(EventArgs e)
        {
            if (!DesignMode)
                Columns[0].Visible = MultipleSelection;

            base.OnDataBinding(e);
        }

        protected override void PerformDataBinding(IEnumerable data)
        {
            if (!DesignMode)
            {
                bool hasChanged = SelectionBuffer.IntersectWith(getAllDataKeys(data));
                if (data != null)
                    DataRowCount = ((DataView)data).Count;
                else
                    DataRowCount = 0;
            }
            
            base.PerformDataBinding(data);
        }

        protected override void OnRowDataBound(GridViewRowEventArgs e)
        {
            base.OnRowDataBound(e);
            
            if (!DesignMode)
            {
                CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
                if (chkSelect != null)
                {
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        HiddenField hdnOldSelectAll = (HiddenField)e.Row.FindControl("hdnOldSelectAll");
                        setSelectAll(oldSelectAllValue, chkSelect, hdnOldSelectAll);
                    }
                    else if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        int id = (int)DataKeys[e.Row.RowIndex].Value;
                        chkSelect.Visible = getBooleanFieldValue(e.Row, SelectionBoxVisibleBy);
                        chkSelect.Enabled = getBooleanFieldValue(e.Row, SelectionBoxEnabledBy);
                        chkSelect.Checked = (chkSelect.Visible && chkSelect.Enabled && SelectionBuffer.GetSelected(id));
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            addJsEventHandler();
        }

        private void addJsEventHandler()
        {
            if (SelectAllCheckBox != null && SelectAllCheckBox.Visible)
            {
                string jsFunc_checkAllBoxes_name = "checkAllBoxes_" + this.ClientID;

                StringBuilder jsFunc_checkAllBoxes =
                    new StringBuilder(string.Format("\nfunction {0}(checkedOn) {{\n", jsFunc_checkAllBoxes_name));
                foreach (GridViewRow row in Rows)
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                        if (chkSelect != null && chkSelect.Visible && chkSelect.Enabled)
                            jsFunc_checkAllBoxes.AppendFormat(
                                "document.getElementById('{0}').checked = checkedOn;\n",
                                chkSelect.ClientID);
                    }

                jsFunc_checkAllBoxes.Append("}");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), jsFunc_checkAllBoxes_name,
                                                            jsFunc_checkAllBoxes.ToString(), true);


                string jsFunc_handleSelectAll_name = "handleSelectAll_" + this.ClientID;

                string jsFunc_handleSelectAll = string.Format(@"
                    function {0}(oEvent) {{
                        if (oEvent.ctrlKey)
                            {{ var cb = document.getElementById('{1}');
                               {2}(cb.checked);
                               
                               var parentCell = cb.parentNode;
                               while (parentCell.tagName != 'TH') {{ parentCell = parentCell.parentNode; }}
                               parentCell.style.backgroundColor = '#AAB9C2';
                               
                               var hdn = document.getElementById('{3}');
                               hdn.value = cb.checked;
                            }}
                        else
                            {{ javascript:setTimeout('{4}', 0); }}
                    }}",
                    jsFunc_handleSelectAll_name,
                    SelectAllCheckBox.ClientID,
                    jsFunc_checkAllBoxes_name,
                    OldSelectAllHiddenField.ClientID,
                    Page.ClientScript.GetPostBackEventReference(SelectAllCheckBox, "").Replace("'", @"\'"));

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), jsFunc_handleSelectAll_name,
                                                            jsFunc_handleSelectAll.ToString(), true);

                SelectAllCheckBox.InputAttributes.Add("onclick", string.Format("{0}(event)", jsFunc_handleSelectAll_name));
            }
        }

        private void copyAllIdsToSelectionBuffer(IEnumerable data)
        {
            SelectionBuffer.CopyFrom(getAllDataKeys(data));
        }

        private ArrayList getAllDataKeys(IEnumerable data)
        {
            ArrayList keys = new ArrayList();

            if (DataKeyNames.Length == 0)
                throw new ApplicationException("Property DataKeyNames needs to be filled for control MultipleSelectionGridView.");

            // for now, this only works with a DataSet as data source
            if (data != null)
            {
                foreach (DataRowView row in data)
                    if (getBooleanFieldValue(row, SelectionBoxVisibleBy) && getBooleanFieldValue(row, SelectionBoxEnabledBy))
                        keys.Add(row[DataKeyNames[0]]);
            }
            return keys;
        }


        #region SelectAll

        // SelectAll lifecycle:
        //   1. InitializeRow: HiddenField created (automatic ViewState -> HiddenField value)
        //   2. OnPagePreLoad: HiddenField value -> oldSelectAllValue
        //                     check if changed (compare with CheckBox)
        //                     new CheckBox value -> HiddenField & oldSelectAllValue
        //   3. OnRowDataBound: oldSelectAllValue -> CheckBox & HiddenField
        //   4. OnPreRender: JavaScript event handler created for CheckBox
        //                   (will change HiddenField value only on CTRL-click)
        private bool oldSelectAllValue = false;

        protected CheckBox SelectAllCheckBox
        {
            get { return (HeaderRow != null ? (CheckBox)HeaderRow.FindControl("chkSelect") : null); }
        }

        protected HiddenField OldSelectAllHiddenField
        {
            get { return (HeaderRow != null ? (HiddenField)HeaderRow.FindControl("hdnOldSelectAll") : null); }
        }

        private void createCheckBox(GridViewRow row)
        {
            CheckBox chkSelect = new CheckBox();
            chkSelect.ID = "chkSelect";
            row.Cells[0].Controls.Add(chkSelect);
            row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            row.Cells[0].CssClass = "aligncenter checkbox-col";
            row.Cells[0].Width = Unit.Pixel(18);
        }

        private void createOldSelectAllHiddenField(GridViewRow row)
        {
            HiddenField hdnOldSelectAll = new HiddenField();
            hdnOldSelectAll.ID = "hdnOldSelectAll";
            row.Cells[0].Controls.Add(hdnOldSelectAll);
        }

        private bool selectAllChanged()
        {
            bool val;
            if (bool.TryParse(OldSelectAllHiddenField.Value, out val))
                oldSelectAllValue = val;

            bool changed = oldSelectAllValue != SelectAllCheckBox.Checked;

            setSelectAll(SelectAllCheckBox.Checked);

            return changed;
        }

        private void setSelectAll(bool newValue)
        {
            setSelectAll(newValue, SelectAllCheckBox, OldSelectAllHiddenField);
        }

        private void setSelectAll(bool newValue, CheckBox selectAllCheckBox, HiddenField oldSelectAllHiddenField)
        {
            if (selectAllCheckBox != null)
                selectAllCheckBox.Checked = newValue;
            if (oldSelectAllHiddenField != null)
                oldSelectAllHiddenField.Value = newValue.ToString();
            oldSelectAllValue = newValue;
        }

        #endregion


        private bool getBooleanFieldValue(GridViewRow row, string fieldName)
        {
            // for now, this only works with a DataSet as data source
            return getBooleanFieldValue((DataRowView)row.DataItem, fieldName);
        }

        private bool getBooleanFieldValue(DataRowView dataRow, string fieldName)
        {
            if (fieldName == "")
                return true;
            else
                return (dataRow[fieldName] != System.DBNull.Value ? (bool)dataRow[fieldName] : false);
        }

        private string getUniqueName(string localName)
        {
            return string.Format("{0}${1}${2}", this.Page.GetType(), this.ID, localName);
        }

        private void persistSelection()
        {
            if (Columns[0].Visible && Rows.Count > 0)
            {
                if (selectAllChanged())
                    SelectAllIds(SelectAllCheckBox.Checked);
                else if (DataKeys.Count > 0)
                    foreach (GridViewRow row in Rows)
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                            int id = (int)DataKeys[row.RowIndex].Value;

                            SelectionBuffer.SetSelected(id, chkSelect.Checked);
                        }
            }
        }
    }
}
