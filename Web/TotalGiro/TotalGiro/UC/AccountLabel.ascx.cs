using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Notifications;

public enum AccountLabelDisplayOptions
{
    DisplayNumber,
    DisplayName,
    DisplayNumberName,
    DisplayNameNumber
}

public enum AccountLabelNavigationOptions
{
    AccountView,
    PortfolioView
}

public partial class AccountLabel : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (RetrieveData)
                getAccountDetails();
        }
    }

    #region Props

    /// <summary>
    /// Gets a value indicating how the label should be navigate.
    /// </summary>
    [Description("Gets a value indicating whether search criterion AccountName should be visible."), DefaultValue(AccountLabelNavigationOptions.AccountView), Category("Behavior")]
    public AccountLabelNavigationOptions NavigationOption
    {
        get
        {
            object b = ViewState["NavigationOption"];
            return ((b == null) ? AccountLabelNavigationOptions.AccountView : (AccountLabelNavigationOptions)b);
        }
        set
        {
            ViewState["NavigationOption"] = value;
            setNavigationURL();
        }
    }

    /// <summary>
    /// Gets a value indicating how the label should be displayed.
    /// </summary>
    [Description("Gets a value indicating whether search criterion AccountName should be visible."), DefaultValue(AccountLabelDisplayOptions.DisplayNumber), Category("Behavior")]
    public AccountLabelDisplayOptions AccountDisplayOption
    {
        get
        {
            object b = ViewState["AccountDisplayOption"];
            return ((b == null) ? AccountLabelDisplayOptions.DisplayNumber : (AccountLabelDisplayOptions)b);
        }
        set
        {
            ViewState["AccountDisplayOption"] = value;
        }
    }

    [Browsable(false)]
    public int AccountID
    {
        get
        {
            object s = ViewState["AccountID"];
            return ((s == null) ? 0 : (int)s);
        }
        set
        {
            bool refresh = (AccountID != value);
            ViewState["AccountID"] = value;
            if (value != 0)
            {
                if (refresh && RetrieveData)
                    getAccountDetails();
                setNavigationURL();
            }
        }
    }

    //[Description("Width of the control."), DefaultValue(70), Category("Behavior")]
    //public Unit Width
    //{
    //    get
    //    {
    //        double width = ViewState["Width"] != null ? (double)ViewState["Width"] : 70;
    //        return new Unit(width);
    //    }
    //    set
    //    {
    //        ViewState["Width"] = value.Value;
    //         Width = value;
    //    }
    //}

    public System.Drawing.Color ForeColor 
    {
        get
        {
            object s = ViewState["ForeColor"];
            return ((s == null) ? System.Drawing.Color.Black : (System.Drawing.Color)s);
        }
        set
        {
            ViewState["ForeColor"] = value;
            lbtnAccount.ForeColor = value;
        }
    }

    public string ToolTip
    {
        get
        {
            object s = ViewState["ToolTip"];
            return ((s == null) ? "" : (string)s);
        }
        set
        {
            ViewState["ToolTip"] = value;
            lbtnAccount.ToolTip = value;
        }
    }

    public string AltText
    {
        get
        {
            object s = ViewState["AltText"];
            return ((s == null) ? "" : (string)s);
        }
        set
        {
            ViewState["AltText"] = value;
        }
    }

    [Browsable(false)]
    public string AccountNumber
    {
        get
        {
            object s = ViewState["AccountNumber"];
            return ((s == null) ? "" : (string)s);
        }
        set
        {
            ViewState["AccountNumber"] = value;
            setAccountLabel();
            setNavigationURL();
        }
    }

    [Browsable(false)]
    public string AccountName
    {
        get
        {
            object s = ViewState["AccountName"];
            return ((s == null) ? "" : (string)s);
        }
        set
        {
            ViewState["AccountName"] = value;
            setAccountLabel();
        }
    }

    [Browsable(false)]
    public string Notification
    {
        get
        {
            object s = ViewState["Notification"];
            return ((s == null) ? "" : (string)s);
        }
        set 
        {
            ViewState["Notification"] = value;
            setNotification();
        }
    }

    [Browsable(false)]
    public NotificationTypes NotificationType
    {
        get
        {
            object s = ViewState["NotificationType"];
            return ((s == null) ? NotificationTypes.Info : (NotificationTypes)s);
        }
        set 
        {
            ViewState["NotificationType"] = value;
            setNotificationType();
        }
    }

    [Browsable(false)]
    public bool AccountIsDeparting
    {
        get
        {
            object s = ViewState["AccountIsDeparting"];
            return ((s == null) ? false : (bool)s);
        }
        set
        {
            ViewState["AccountIsDeparting"] = value;
            setAccountStatus();
        }
    }

    [Browsable(false)]
    public bool AccountIsUnderRebalance
    {
        get
        {
            object s = ViewState["AccountIsUnderRebalance"];
            return ((s == null) ? false : (bool)s);
        }
        set
        {
            ViewState["AccountIsUnderRebalance"] = value;
            setAccountStatus();
        }
    }

    [Browsable(false)]
    public int AccountActiveOrderCount
    {
        get
        {
            object s = ViewState["AccountActiveOrderCount"];
            return ((s == null) ? 0 : (int)s);
        }
        set
        {
            ViewState["AccountActiveOrderCount"] = value;
            setAccountStatus();
        }
    }

    [Browsable(false)]
    public bool AccountIsActive
    {
        get
        {
            object s = ViewState["AccountIsActive"];
            return ((s == null) ? true : (bool)s);
        }
        set
        {
            ViewState["AccountIsActive"] = value;
            setAccountStatus();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the Fee Details should be visible.
    /// </summary>
    [Description("A property indicating whether the Fee Details should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowFeeDetails
    {
        get
        {
            object b = ViewState["ShowFeeDetails"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowFeeDetails"] = value;
            ttiFeeDetails.Visible = value;
        }
    }

    [Description("Flag that determines whether Notification data should be retrieved."), DefaultValue(true), Category("Behavior")]
    public bool RetrieveData
    {
        get
        {
            object s = ViewState["RetrieveData"];
            return ((s == null) ? true : (bool)s);
        }
        set
        {
            ViewState["RetrieveData"] = value;
        }
    }

    #endregion

    #region Methods

    public void GetData()
    {
        getAccountDetails();
    }

    public void Clear()
    {
        AccountNumber = "";
        AccountName = "";
        ttiNotification.TooltipContent = "";
        ttiAccountStatus.TooltipContent = "";
        ttiFeeDetails.TooltipContent = "";
        AccountIsDeparting = false;
        AccountIsUnderRebalance = false;
        AccountIsActive = true;
    }

    #endregion

    #region Helpers

    protected void getAccountDetails()
    {
        Clear();
        if (AccountID != 0)
        {
            AccountDetailsView view = AccountLabelAdapter.GetAccountDetails(AccountID, ShowFeeDetails);
            if (view != null)
            {
                AccountNumber = view.AccountNumber;
                AccountName = view.AccountName;
                Notification = view.Notification;
                NotificationType = view.NotificationType;
                AccountIsActive = (view.Status == B4F.TotalGiro.Accounts.AccountStati.Active);
                AccountIsDeparting = view.IsDeparting;
                AccountIsUnderRebalance = view.IsUnderRebalance;
                AccountActiveOrderCount = view.ActiveOrderCount;
                if (ShowFeeDetails && view.DepositFeeInfo.Length > 0)
                    ttiFeeDetails.TooltipContent = view.DepositFeeInfo;
            }
        }
        else if (!string.IsNullOrEmpty(AltText))
        {
            this.lbtnAccount.Text = AltText;
            this.lbtnAccount.NavigateUrl = "";
        }
    }

    protected void setAccountLabel()
    {
        switch (AccountDisplayOption)
        {
            case AccountLabelDisplayOptions.DisplayName:
                lbtnAccount.Text = AccountName;
                break;
            case AccountLabelDisplayOptions.DisplayNumberName:
                lbtnAccount.Text = string.Format("{0} ({1})", AccountNumber, AccountName);
                break;
            case AccountLabelDisplayOptions.DisplayNameNumber:
                lbtnAccount.Text = string.Format("{0} ({1})", AccountName, AccountNumber);
                break;
            default:
                lbtnAccount.Text = AccountNumber;
                break;
        }
    }

    protected void setNotification()
    {
        ttiNotification.TooltipContent = Notification;
    }

    protected void setNotificationType()
    {
        ttiNotification.TooltipDefaultImage = (NotificationType == NotificationTypes.Warning ? B4F.Web.WebControls.TooltipImage.DefaultImage.ExclamationMark : B4F.Web.WebControls.TooltipImage.DefaultImage.Balloon_Small);
    }

    protected void setAccountStatus()
    {
        if (!AccountIsActive)
        {
            ttiAccountStatus.TooltipContent = string.Format("Account {0} is inactive.", AccountNumber);
            ttiAccountStatus.TooltipDefaultImage = B4F.Web.WebControls.TooltipImage.DefaultImage.InActive;
        }
        else if (AccountIsDeparting)
        {
            ttiAccountStatus.TooltipContent = string.Format("Account {0} is leaving.", AccountNumber);
            ttiAccountStatus.TooltipDefaultImage = B4F.Web.WebControls.TooltipImage.DefaultImage.EmergencyExit;
        }
        else if (AccountIsUnderRebalance)
        {
            ttiAccountStatus.TooltipContent = string.Format("Account {0} is under rebalance.", AccountNumber);
            ttiAccountStatus.TooltipDefaultImage = B4F.Web.WebControls.TooltipImage.DefaultImage.Balance;
        }
        else if (AccountActiveOrderCount > 0)
        {
            ttiAccountStatus.TooltipContent = string.Format("Account {0} has {1} active order(s).", AccountNumber, AccountActiveOrderCount);
            ttiAccountStatus.TooltipDefaultImage = B4F.Web.WebControls.TooltipImage.DefaultImage.ShoppingCart;
        }
        else
        {
            ttiAccountStatus.TooltipContent = "";
        }
    }

    protected void setNavigationURL()
    {
        string qStr;
        switch (this.NavigationOption)
        {
            case AccountLabelNavigationOptions.PortfolioView:
                qStr = QueryStringModule.Encrypt(string.Format("AccountNumber={0}", AccountNumber));
                lbtnAccount.NavigateUrl =  string.Format("~/Portfolio/ClientPortfolio.aspx{0}", qStr);
                break;
            default:
                qStr = QueryStringModule.Encrypt(string.Format("AccountID={0}", AccountID));
                lbtnAccount.NavigateUrl = string.Format("~/DataMaintenance/Accounts/Account.aspx{0}", qStr);
                break;
        }
    }

    #endregion

}
