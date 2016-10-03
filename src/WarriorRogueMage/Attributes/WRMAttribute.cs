//-----------------------------------------------------------------------------
// <copyright file="WRMAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using System;
    using WheelMUD.Core;

    /// <summary>The attribute that represents the Warrior, Rogue, and Mage player roles.</summary>
    public class WRMAttribute : GameAttribute, IConvertible
    {
        /// <summary>Initializes a new instance of the <see cref="WRMAttribute"/> class.</summary>
        /// <param name="name">The name of this attribute.</param>
        /// <param name="abbreviation">The abbreviation for this attribute.</param>
        /// <param name="formula">The formula that this attribute will use, if any.</param>
        /// <param name="startValue">The start value for this attribute.</param>
        /// <param name="minValue">The minimum value for this attribute.</param>
        /// <param name="maxValue">The maximum value for this attribute.</param>
        public WRMAttribute(string name, string abbreviation, string formula, int startValue, int minValue, int maxValue)
            : base(null, name, abbreviation, formula, startValue, minValue, maxValue, true)
        {
        }

        /// <summary>Returns the <see cref="TypeCode"/> for this instance.</summary>
        /// <returns>The enumerated constant that is the <see cref="TypeCode"/> of the class or value type that implements this interface.</returns>
        public TypeCode GetTypeCode()
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to a boolean value using the specified formatting information.</summary>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public bool ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent Unicode character using the specified formatting information.</summary>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public char ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified formatting information.</summary>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified formatting information.</summary>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public byte ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified formatting information.</summary>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public short ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified formatting information.</summary>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified formatting information.</summary>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public int ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified formatting information.</summary>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public uint ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified formatting information.</summary>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public long ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified formatting information.</summary>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the specified formatting information.</summary>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public float ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the specified formatting information.</summary>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public double ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="Decimal"/> number using the specified formatting information.</summary>
        /// <returns>A <see cref="Decimal"/> number equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="DateTime"/> using the specified formatting information.</summary>
        /// <returns>A <see cref="DateTime"/> instance equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>Converts the value of this instance to an equivalent <see cref="String"/> using the specified formatting information.</summary>
        /// <returns>A <see cref="String"/> instance equivalent to the value of this instance.</returns>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public string ToString(IFormatProvider provider)
        {
            return this.Name;
        }

        /// <summary>Converts the value of this instance to an <see cref="Object"/> of the specified <see cref="Type"/> that has an equivalent value, using the specified formatting information.</summary>
        /// <returns>An <see cref="Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.</returns>
        /// <param name="conversionType">The <see cref="Type"/> to which the value of this instance is converted.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation supplying culture-specific formatting information.</param>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return this;
        }
    }
}