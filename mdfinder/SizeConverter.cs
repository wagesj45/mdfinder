using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace mdfinder
{
    public class SizeConverter : IValueConverter
    {
        /// <summary> Suffix for size. </summary>
        private static readonly string[] SUFFIX = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        /// <summary> Converts a value. </summary>
        /// <param name="value">      The value produced by the binding source. </param>
        /// <param name="targetType"> The type of the binding target property. </param>
        /// <param name="parameter">  The converter parameter to use. </param>
        /// <param name="culture">    The culture to use in the converter. </param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is
        /// used.
        /// </returns>
        /// <remarks>
        /// Thanks to user deepee1 on stackoverflow for the implementation.
        /// https://stackoverflow.com/a/4975942/1210377.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var size = (long)value;

                if (size == 0)
                {
                    return "0" + SUFFIX[0];
                }

                int place = System.Convert.ToInt32(Math.Floor(Math.Log(size, 1024)));
                double num = Math.Round(size / Math.Pow(1024, place), 1);
                return (Math.Sign(size) * num).ToString() + SUFFIX[place];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary> Converts a value. </summary>
        /// <param name="value">      The value that is produced by the binding target. </param>
        /// <param name="targetType"> The type to convert to. </param>
        /// <param name="parameter">  The converter parameter to use. </param>
        /// <param name="culture">    The culture to use in the converter. </param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is
        /// used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
