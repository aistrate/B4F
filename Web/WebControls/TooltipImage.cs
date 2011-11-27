using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;

namespace B4F.Web.WebControls
{
    /// <summary>
    /// This control generates an Image tag with optional attributes that can display a tooltip
    /// </summary>
    [DefaultProperty("ImageUrl"), ToolboxData("<{0}:TooltipImage runat=server></{0}:TooltipImage>")]
    public class TooltipImage : System.Web.UI.WebControls.Image
    {
        #region Constructors

        public TooltipImage()
        {
            _opacity = 100;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Allignment of the title and body text
        /// </summary>
        public enum TextAlign
        {
            /// <summary>
            /// Left alignment
            /// </summary>
            left,
            /// <summary>
            /// Justified
            /// </summary>
            justify,
            /// <summary>
            /// Right alignment
            /// </summary>
            right
        }

        /// <summary>
        /// Sets the default image
        /// </summary>
        public enum DefaultImage
        {
            /// <summary>
            /// No default image
            /// </summary>
            none,
            /// <summary>
            /// A Large Attention sign
            /// </summary>
            Attention_Large,
            /// <summary>
            /// A Small Attention sign
            /// </summary>
            Attention_Small,
            /// <summary>
            /// A Large Balloon
            /// </summary>
            Balloon_Large,
            /// <summary>
            /// A small Balloon
            /// </summary>
            Balloon_Small,
            /// <summary>
            /// An Exclamation Mark
            /// </summary>
            ExclamationMark,
            /// <summary>
            /// An Emergency Exit Sign
            /// </summary>
            EmergencyExit,
            /// <summary>
            /// Inactive sign
            /// </summary>
            InActive,
            /// <summary>
            /// A RIP Cross
            /// </summary>
            RIPCross,
            /// <summary>
            /// Balance
            /// </summary>
            Balance,
            /// <summary>
            /// Coins
            /// </summary>
            Coins,
            /// <summary>
            /// ShoppingCart
            /// </summary>
            ShoppingCart
        }


        /// <summary>
        /// Title. Background color is automatically the same as the border color.
        /// </summary>
        [Category("Tooltip"), Description("Title. Background color is automatically the same as the border color. "), DefaultValue("")]
        public string TooltipTitle
        {
            get { return this._title; }
            set { this._title = value; }
        }

        /// <summary>
        /// The content will be displayed in the body of the tooltip and can be plain text or HTML.
        /// </summary>
        [Category("Tooltip"), Description("The content will be displayed in the body of the tooltip and can be plain text or HTML."), DefaultValue("")]
        public string TooltipContent
        {
            get
            {
                object obj = ViewState["TooltipContent"];
                return (obj == null) ? "" : obj.ToString();
            }
            set
            {
                ViewState["TooltipContent"] = value;
            }
        }

        /// <summary>
        /// Background color of the tooltip.
        /// </summary>
        [Category("Tooltip"), Description("Background color of the tooltip."), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter)), Bindable(true)]
        public Color TooltipBackgroundColor
        {
            get { return this._bgColor; }
            set { this._bgColor = value; }
        }

        /// <summary>
        /// Border Color
        /// </summary>
        [Category("Tooltip"), Description("Border Color"), DefaultValue(typeof(Color), "")]
        public Color TooltipBorderColor
        {
            get { return this._borderColor; }
            set { this._borderColor = value; }
        }

        /// <summary>
        /// Width of tooltip border. May be 0 to hide the border.
        /// </summary>
        [Category("Tooltip"), Description("Width of tooltip border. May be 0 to hide the border."), DefaultValue(typeof(Unit), "")]
        public Unit TooltipBorderWidth
        {
            get { return this._borderWidth; }
            set { this._borderWidth = value; }
        }

        /// <summary>
        /// Specifies the Font family, size and weight
        /// </summary>
        [Category("Tooltip"), Description("Specifies the Font family, size and weight"), NotifyParentProperty(true), DefaultValue((string)null)]
        public Font TooltipFont
        {
            get { return this._font; }
            set { this._font = value; }
        }

        /// <summary>
        /// The color of the content of the tooltip
        /// </summary>
        [Category("Tooltip"), Description("The color of the content of the tooltip"), DefaultValue(typeof(Color), "")]
        public Color TooltipForeColor
        {
            get { return this._foreColor; }
            set { this._foreColor = value; }
        }

        /// <summary>
        /// Transparency of tooltip. Opacity is the opposite of transparency. Value must be a number between 0 (fully transparent) and 100 (opaque, no transparency).
        /// </summary>
        [Category("Tooltip"), Description("Transparency of tooltip. Opacity is the opposite of transparency. Value must be a number between 0 (fully transparent) and 100 (opaque, no transparency)."), DefaultValue("100")]
        public byte TooltipOpacity
        {
            get { return this._opacity; }
            set { this._opacity = ((value > 100) ? (byte)100 : (byte)value); }
        }

        /// <summary>
        /// Creates shadow with the specified width (offset). Shadow color is automatically set to '#cccccc' (light grey) if no shadow color is specified.
        /// </summary>
        [Category("Tooltip"), Description("Creates shadow with the specified width (offset). Shadow color is automatically set to '#cccccc' (light grey) if no shadow color is specified."), DefaultValue(typeof(Color), "")]
        public Color TooltipShadowColor
        {
            get { return this._shadowColor; }
            set { this._shadowColor = value; }
        }

        /// <summary>
        /// Creates shadow with the specified color. Shadow width (strength) is automatically set to 5 (pixels) if no shadow width is specified.
        /// </summary>
        [Category("Tooltip"), Description("Creates shadow with the specified color. Shadow width (strength) is automatically set to 5 (pixels) if no shadow width is specified."), DefaultValue(typeof(Unit), "")]
        public int TooltipShadowWidth
        {
            get { return this._shadowWidth; }
            set { this._shadowWidth = value; }
        }

        /// <summary>
        /// Width of the tooltip.
        /// </summary>
        [Category("Tooltip"), Description("Width of the tooltip."), DefaultValue(typeof(Unit), "")]
        public Unit TooltipWidth
        {
            get { return this._width; }
            set { this._width = value; }
        }

        /// <summary>
        /// Color of title text, default value is white.
        /// </summary>
        [Category("Tooltip"), Description("Color of title text, default value is white."), DefaultValue(typeof(Color), "")]
        public Color TooltipTitleColor
        {
            get { return this._titleColor; }
            set { this._titleColor = value; }
        }

        /// <summary>
        /// Aligns the text of both the title and the body of the tooltip.
        /// </summary>
        [Category("Tooltip"), Description("Aligns the text of both the title and the body of the tooltip."), DefaultValue("1")]
        public TextAlign TooltipTextAlign
        {
            get { return this._textAlign; }
            set { this._textAlign = value; }
        }

        /// <summary>
        /// Inner spacing, i.e. the spacing between border and content, for instance text or image(s).
        /// </summary>
        [Category("Tooltip"), Description("Inner spacing, i.e. the spacing between border and content, for instance text or image(s)."), DefaultValue(typeof(Unit), "")]
        public int TooltipPadding
        {
            get { return this._padding; }
            set { this._padding = value; }
        }

        /// <summary>
        /// Like OS-based tooltips, the tooltip doesn't follow the movements of the mouse-pointer.
        /// </summary>
        [Category("Tooltip"), Description("Like OS-based tooltips, the tooltip doesn't follow the movements of the mouse-pointer."), DefaultValue(false)]
        public bool TooltipStatic
        {
            get { return this._static; }
            set { this._static = value; }
        }

        /// <summary>
        /// The tooltip stays fixed on it's initial position until another tooltip is activated, or the user clicks on the document.
        /// </summary>
        [Category("Tooltip"), Description("The tooltip stays fixed on it's initial position until another tooltip is activated, or the user clicks on the document."), DefaultValue(false)]
        public bool TooltipSticky
        {
            get { return this._sticky; }
            set { this._sticky = value; }
        }

        /// <summary>
        /// Closes the tooltip once the user clicks somewhere inside the tooltip or into the document.
        /// </summary>
        [Category("Tooltip"), Description("Closes the tooltip once the user clicks somewhere inside the tooltip or into the document. Value: true, false."), DefaultValue(false)]
        public bool TooltipClickClose
        {
            get { return this._clickClose; }
            set { this._clickClose = value; }
        }

        /// <summary>
        /// Does the tooltip have a close button.
        /// </summary>
        [Category("Tooltip"), Description("Does the tooltip have a close button."), DefaultValue(false)]
        public bool HasCloseButton
        {
            get { return this._hasCloseButton; }
            set { this._hasCloseButton = value; }
        }

        /// <summary>
        /// Tooltip shows up after the specified timeout (milliseconds). A behavior similar to that of OS based tooltips.
        /// </summary>
        [Category("Tooltip"), Description("Tooltip shows up after the specified timeout (milliseconds). A behavior similar to that of OS based tooltips."), DefaultValue("")]
        public int TooltipDelay
        {
            get { return this._delay; }
            set { this._delay = value; }
        }

        /// <summary>
        /// Tooltip position on the screen.
        /// </summary>
        [Category("Tooltip"), Description("Tooltip position on the screen."), DefaultValue("")]
        public string TooltipPosition
        {
            get { return this._position; }
            set { this._position = value; }
        }

        /// <summary>
        /// Sets a default image for this tooltip image button control. Note that the ImageUrl property will overrule this image.
        /// </summary>
        [Category("Tooltip"), Description("Sets a default image for this tooltip image control. Note that the ImageUrl property will overrule this image."), DefaultValue(DefaultImage.Attention_Small)]
        public DefaultImage TooltipDefaultImage
        {
            get
            {
                object obj = ViewState["TooltipDefaultImage"];
                return (obj == null) ? DefaultImage.Attention_Small : (DefaultImage)obj;
            }
            set
            {
                ViewState["TooltipDefaultImage"] = value;
            }

        }

        /// <summary>
        /// Is the tooltip in the shape of a balloon.
        /// </summary>
        [Category("Tooltip"), Description("Is the tooltip in the shape of a balloon."), DefaultValue("true")]
        public bool IsBalloon
        {
            get { return this._isBalloon; }
            set { this._isBalloon = value; }
        }

        /// <summary>
        /// Is the tooltip above the mouse pointer.
        /// </summary>
        [Category("Tooltip"), Description("Is the tooltip above the mouse pointer."), DefaultValue("true")]
        public bool IsTooltipAbove
        {
            get { return this._above; }
            set { this._above = value; }
        }

        /// <summary>
        /// The offset of the tooltip.
        /// </summary>
        [Category("Tooltip"), Description("The offset of the tooltip.")]
        public int OffSetX
        {
            get { return this._offSetX; }
            set { this._offSetX = value; }
        }

        /// <summary>
        /// The milliseconds to fade in.
        /// </summary>
        [Category("Tooltip"), Description("The milliseconds to fade in."), DefaultValue(600)]
        public int FadeIn
        {
            get { return this._fadeIn; }
            set { this._fadeIn = value; }
        }
        
        /// <summary>
        /// The milliseconds to fade out.
        /// </summary>
        [Category("Tooltip"), Description("The milliseconds to fade out."), DefaultValue(600)]
        public int FadeOut
        {
            get { return this._fadeOut; }
            set { this._fadeOut = value; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.TooltipContent))
            {
                StringBuilder sb = new StringBuilder();
                if (_title != null && _title.Length > 0) sb.Append(string.Format(", TITLE, '{0}'", _title.Replace("'", @"\'")));
                if (_titleColor != Color.Empty) sb.Append(string.Format(", TitleBgColor, {0}", ColorTranslator.ToHtml(_titleColor)));

                sb.Append(string.Format(", BALLOON_IMG_0, '{0}'", getResourceUrl(BALLOON_IMG_0)));
                sb.Append(string.Format(", BALLOON_IMG_1, '{0}'", getResourceUrl(BALLOON_IMG_1)));
                sb.Append(string.Format(", BALLOON_IMG_2, '{0}'", getResourceUrl(BALLOON_IMG_2)));
                sb.Append(string.Format(", BALLOON_IMG_3, '{0}'", getResourceUrl(BALLOON_IMG_3)));
                sb.Append(string.Format(", BALLOON_IMG_4, '{0}'", getResourceUrl(BALLOON_IMG_4)));
                sb.Append(string.Format(", BALLOON_IMG_5, '{0}'", getResourceUrl(BALLOON_IMG_5)));
                sb.Append(string.Format(", BALLOON_IMG_6, '{0}'", getResourceUrl(BALLOON_IMG_6)));
                sb.Append(string.Format(", BALLOON_IMG_7, '{0}'", getResourceUrl(BALLOON_IMG_7)));
                sb.Append(string.Format(", BALLOON_IMG_8, '{0}'", getResourceUrl(BALLOON_IMG_8)));
                sb.Append(string.Format(", BALLOON_IMG_9, '{0}'", getResourceUrl(BALLOON_IMG_9)));
                sb.Append(string.Format(", BALLOON_IMG_10, '{0}'", getResourceUrl(BALLOON_IMG_10)));

                if (_bgColor != Color.Empty) sb.Append(string.Format(", BGCOLOR, {0}", ColorTranslator.ToHtml(_bgColor)));
                if (_borderColor != Color.Empty) sb.Append(string.Format(", BORDERCOLOR, {0}", ColorTranslator.ToHtml(_borderColor)));
                if (!_borderWidth.IsEmpty) sb.Append(string.Format(", BORDERWIDTH, {0}", _borderWidth.ToString()));
                if (_foreColor != Color.Empty) sb.Append(string.Format(", FONTCOLOR, {0}", ColorTranslator.ToHtml(_foreColor)));
                if (_font != null && _font.FontFamily.Name != null && _font.FontFamily.Name.Length > 0) sb.Append(string.Format(", FONTFACE, '{0}'", _font.Name));
                if (_font != null) sb.Append(string.Format(", FONTSIZE, {0}", _font.Size));
                if (_font != null) sb.Append(string.Format(", FONTWEIGHT, {0}", _font.Style));
                if (_opacity < 100) sb.Append(string.Format(", OPACITY, {0}", _opacity));
                if (_shadowColor != Color.Empty) sb.Append(string.Format(", SHADOWCOLOR, {0}", ColorTranslator.ToHtml(_shadowColor)));
                if (_shadowWidth > 0) sb.Append(string.Format(", SHADOWWIDTH, {0}", _shadowWidth.ToString()));
                if (!_width.IsEmpty) sb.Append(string.Format(", WIDTH, {0}", _width.Value));
                if (_textAlign != TextAlign.left) sb.Append(string.Format(", TEXTALIGN, {0}", _textAlign.ToString()));
                if (_static) sb.Append(", STATIC, true");
                if (_sticky) sb.Append(", STICKY, true");
                if (_clickClose) sb.Append(", CLICKCLOSE, true");
                if (_hasCloseButton) sb.Append(", CLOSEBTN, true");
                if (_position != null)
                {
                    if (_position.Length > 0) sb.Append(string.Format(", FIX, {0}", _position.ToString()));
                }
                if (_padding != 0) sb.Append(string.Format(", PADDING, {0}", _padding.ToString()));
                if (_delay != 0) sb.Append(string.Format(", DELAY, {0}", _delay.ToString()));
                if (_isBalloon) sb.Append(", BALLOON, true");
                if (_above) sb.Append(", ABOVE, true");
                if (_offSetX != 0) sb.Append(string.Format(", OFFSETX, {0}", _offSetX.ToString()));
                if (_fadeIn != 0) sb.Append(string.Format(", FADEIN, {0}", _fadeIn.ToString()));
                if (_fadeOut != 0) sb.Append(string.Format(", FADEOUT, {0}", _fadeOut.ToString()));
                
                //if (_content != null && _content.Length > 0) sb.Append(string.Format("return escape('{0}'); ", _content.Replace("'", @"\'")));

                // ABOVE, true, OFFSETX, -17, FADEIN, 600, FADEOUT, 600, PADDING, 8
                string tooltip = string.Format(@"Tip('{0}'{1})", this.TooltipContent, sb.ToString());
                if (tooltip.Length > 0)
                    writer.AddAttribute("onmouseover", tooltip, true);

                writer.AddAttribute("onmouseout", "UnTip()", true);

                // Set a default image if no ImageUrl has been set and a default image was chosen
                if (string.IsNullOrEmpty(this.ImageUrl))
                {
                    string webResourceKey = string.Empty;
                    switch (TooltipDefaultImage)
                    {
                        case DefaultImage.Attention_Large:
                            webResourceKey = ATTENTION_LARGE_KEY;
                            break;
                        case DefaultImage.Attention_Small:
                            webResourceKey = ATTENTION_SMALL_KEY;
                            break;
                        case DefaultImage.Balloon_Large:
                            webResourceKey = BALLOON_LARGE_KEY;
                            break;
                        case DefaultImage.Balloon_Small:
                            webResourceKey = BALLOON_SMALL_KEY;
                            break;
                        case DefaultImage.ExclamationMark:
                            webResourceKey = EXCLAMATION_KEY;
                            break;
                        case DefaultImage.EmergencyExit:
                            webResourceKey = EMERGENCY_EXIT_KEY;
                            break;
                        case DefaultImage.InActive:
                            webResourceKey = INACTIVE_KEY;
                            break;
                        case DefaultImage.RIPCross:
                            webResourceKey = RIP_CROSS_KEY;
                            break;
                        case DefaultImage.Balance:
                            webResourceKey = BALANCE_KEY;
                            break;
                        case DefaultImage.Coins:
                            webResourceKey = COINS_KEY;
                            break;
                        case DefaultImage.ShoppingCart:
                            webResourceKey = SHOPPING_CART_KEY;
                            break;
                    }

                    //// Set a default image if no ImageUrl has been set and a default image was chosen
                    if (string.IsNullOrEmpty(this.ImageUrl))
                        this.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), webResourceKey);
                }
                base.Render(writer);
            }
        }

        protected string getResourceUrl(string image)
        {
            return Page.ClientScript.GetWebResourceUrl(this.GetType(), image);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ClientScriptManager csm = Page.ClientScript;
            csm.RegisterClientScriptResource(this.GetType(), "B4F.Web.WebControls.ClientScript.wz_tooltip.js");
            csm.RegisterClientScriptResource(this.GetType(), "B4F.Web.WebControls.ClientScript.tip_balloon.js");
            csm.RegisterClientScriptResource(this.GetType(), "B4F.Web.WebControls.ClientScript.tip_centerwindow.js");
            csm.RegisterClientScriptResource(this.GetType(), "B4F.Web.WebControls.ClientScript.tip_followscroll.js");
        }

        #endregion

        #region private properties

        private string _title;
        private Color _bgColor;
        private Color _borderColor;
        private Color _foreColor;
        private Color _titleColor;
        private Color _shadowColor;
        private Font _font;
        private Unit _borderWidth;
        private int _shadowWidth;
        private Unit _width;
        private TextAlign _textAlign;
        private byte _opacity;
        private bool _static;
        private bool _sticky;
        private bool _clickClose;
        private bool _hasCloseButton;
        private int _padding;
        private int _delay;
        private string _position;
        private bool _isBalloon = true;
        private bool _above;
        private int _offSetX;
        private int _fadeIn = 600;
        private int _fadeOut = 600;

        private const string ATTENTION_LARGE_KEY = "B4F.Web.WebControls.Images.attention_Large.gif";
        private const string ATTENTION_SMALL_KEY = "B4F.Web.WebControls.Images.attention_Small.gif";
        private const string BALLOON_LARGE_KEY = "B4F.Web.WebControls.Images.balloon_Large.gif";
        private const string BALLOON_SMALL_KEY = "B4F.Web.WebControls.Images.balloon_Small.gif";
        private const string EXCLAMATION_KEY = "B4F.Web.WebControls.Images.exclamation.gif";
        private const string EMERGENCY_EXIT_KEY = "B4F.Web.WebControls.Images.emergency-exit.gif";
        private const string INACTIVE_KEY = "B4F.Web.WebControls.Images.inactive.png";
        private const string RIP_CROSS_KEY = "B4F.Web.WebControls.Images.cross.gif";
        private const string BALANCE_KEY = "B4F.Web.WebControls.Images.balance.png";
        private const string COINS_KEY = "B4F.Web.WebControls.Images.coins.png";
        private const string SHOPPING_CART_KEY = "B4F.Web.WebControls.Images.cart.png";

        private const string BALLOON_IMG_0 = "B4F.Web.WebControls.BalloonImages.background.gif";
        private const string BALLOON_IMG_1 = "B4F.Web.WebControls.BalloonImages.lt.gif";
        private const string BALLOON_IMG_2 = "B4F.Web.WebControls.BalloonImages.t.gif";
        private const string BALLOON_IMG_3 = "B4F.Web.WebControls.BalloonImages.rt.gif";
        private const string BALLOON_IMG_4 = "B4F.Web.WebControls.BalloonImages.r.gif";
        private const string BALLOON_IMG_5 = "B4F.Web.WebControls.BalloonImages.rb.gif";
        private const string BALLOON_IMG_6 = "B4F.Web.WebControls.BalloonImages.b.gif";
        private const string BALLOON_IMG_7 = "B4F.Web.WebControls.BalloonImages.lb.gif";
        private const string BALLOON_IMG_8 = "B4F.Web.WebControls.BalloonImages.l.gif";
        private const string BALLOON_IMG_9 = "B4F.Web.WebControls.BalloonImages.stemt.gif";
        private const string BALLOON_IMG_10 = "B4F.Web.WebControls.BalloonImages.stemb.gif";

        #endregion

    }
}
