using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace JointPPT.ValueConverter
{
    public class ErrorInfoListStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) return DependencyProperty.UnsetValue;
            if (value == null) return "Done.";
            if (value.GetType() != typeof(List<ErrorInfo>)) return "Done.";
            var list = (List<ErrorInfo>)value;
            if (list.Count == 0) return "Done.";
            var log = $"These files are skipped because:{Environment.NewLine}";
            log += string.Join(Environment.NewLine, list.Select(e => $"{ Path.GetFileName(e.Name)}:{ e.Message}"));
            return log;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
