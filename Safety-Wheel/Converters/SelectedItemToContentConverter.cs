using MahApps.Metro.Controls;
using Safety_Wheel.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Safety_Wheel.Converters
{
    public class SelectedItemToContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Converter вызван. values.Length={values?.Length}, 0-й={values?[0]?.GetType().Name}");

            if (values?[0] is MenuItemViewModel vm)   // ← проверяем наш класс
            {
                System.Diagnostics.Debug.WriteLine(
                    $"  возвращаю Tag = {vm.Tag?.GetType().Name}");
                return vm.Tag;                        // ← это и есть SelectedDateViewModel
            }

            System.Diagnostics.Debug.WriteLine("  возвращаю null");
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}
