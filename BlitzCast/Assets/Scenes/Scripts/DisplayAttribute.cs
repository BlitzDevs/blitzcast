using System;
using UnityEngine;

/// <summary>
/// This class defines a custom C# Attribute.
/// This DisplayAttribute can store the long name, short name, and color of a
/// field of an enum.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DisplayAttribute : Attribute
{
    // FOR REFERENCE:
    // [Example]       Attribute. Part of C#; can mark objects with an Attribute
    //                 and Attributes can be defined to store information in
    //                 relation to an object.
    //
    // [AttributeUsage(AttributeTargets.Field)]
    //                 Specifies the targets of a custom attribute. For this
    //                 attribute, the target is Field, but it could be Class,
    //                 Parameter, etc.
    //                 
    // May be helpful: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/
    //
    // get;
    // private set;    Shorthand way of defining a get-only variable.


    /// <summary>
    /// Initializes a new instance of the <see cref="T:DisplayAttribute"/> class.
    /// </summary>
    /// <param name="longName">Full name.</param>
    /// <param name="shortName">Short, abbreviated name; use 3 letters.</param>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    public DisplayAttribute(string longName, string shortName, int r, int g, int b)
    {
        LongName = longName;
        ShortName = shortName;
        Color = new Color(r, g, b);
    }

    // Properties of this attribute (get only)
    public string LongName
    {
        get;
        private set;
    }

    public string ShortName
    {
        get;
        private set;
    }

    public Color Color
    {
        get;
        private set;
    }
}


/// <summary>
/// A static class to allow the retrieval of DisplayAttribute properties on an
/// enum field.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the color associated with this enum from its Display attribute.
    /// </summary>
    /// <returns>
    /// The predefined display color. Returns magenta if unavailable.
    /// </returns>
    /// <example>
    /// <code>
    /// Color myColor = myEnum.GetDisplayColor();
    /// </code>
    /// </example>
    public static Color GetDisplayColor<T>(this T value) where T : IComparable, IFormattable, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("Value must be enum");
        }

        DisplayAttribute[] attributes = (DisplayAttribute[]) value
           .GetType()
           .GetField(value.ToString())
           .GetCustomAttributes(typeof(DisplayAttribute), false);
        for (int i = 0; i < attributes.Length; i++)
        {
            Debug.Log(i.ToString() + attributes[i].LongName);
            Debug.Log(attributes[0].Color);
        }
        return attributes.Length > 0 ? attributes[0].Color : Color.magenta;
    }

    /// <summary>
    /// Gets the short/abbreviated name associated with this enum from its 
    /// Display attribute.
    /// </summary>
    /// <returns>
    /// The predefined short name. Returns "UNDEFINED" if unavailable.
    /// </returns>
    /// <example>
    /// <code>
    /// string myName = myEnum.GetShortName();
    /// </code>
    /// </example>
    public static string GetLongName<T>(this T value) where T : IComparable, IFormattable, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("Value must be enum");
        }

        DisplayAttribute[] attributes = (DisplayAttribute[])value
           .GetType()
           .GetField(value.ToString())
           .GetCustomAttributes(typeof(DisplayAttribute), false);
        return attributes.Length > 0 ? attributes[0].LongName : "UNDEFINED";
    }

    /// <summary>
    /// Gets the short/abbreviated name associated with this enum from its 
    /// Display attribute.
    /// </summary>
    /// <returns>
    /// The predefined short name. Returns "???" if unavailable.
    /// </returns>
    /// <example>
    /// <code>
    /// string myName = myEnum.GetShortName();
    /// </code>
    /// </example>
    public static string GetShortName<T>(this T value) where T : IComparable, IFormattable, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("Value must be enum");
        }

        DisplayAttribute[] attributes = (DisplayAttribute[])value
           .GetType()
           .GetField(value.ToString())
           .GetCustomAttributes(typeof(DisplayAttribute), false);
        return attributes.Length > 0 ? attributes[0].ShortName : "???";
    }


}