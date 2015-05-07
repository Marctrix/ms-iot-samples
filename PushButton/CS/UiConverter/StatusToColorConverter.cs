using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PushButton.UiConverter
{
    public class StatusToColor : IValueConverter
    {
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.DimGray);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return grayBrush;

            return redBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}