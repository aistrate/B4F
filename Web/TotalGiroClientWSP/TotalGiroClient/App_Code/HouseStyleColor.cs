using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

/// <summary>
/// Colors used for Series in Dundas Charts. 
/// </summary>
public static class HouseStyleColor
{
    public static Color PaerelDarkBlue { get { return Color.FromArgb(0x31, 0x34, 0x57); } }     // color of the official Paerel logo
    public static Color DarkBlue { get { return Color.FromArgb(0x1A, 0x3B, 0x69); } }           // close to Color.Navy
    public static Color LightBlue { get { return Color.FromArgb(0x41, 0x8C, 0xF0); } }          // close to Color.CornflowerBlue
    
    public static Color LightGray { get { return Color.FromArgb(0xD8, 0xD8, 0xD8); } }
    public static Color VeryLightGray { get { return Color.FromArgb(0xEC, 0xEC, 0xEC); } }

    public static Color Red { get { return Color.FromArgb(0xE0, 0x40, 0x0A); } }

    public static Color Yellow { get { return Color.FromArgb(0xFC, 0xB4, 0x41); } }
}
