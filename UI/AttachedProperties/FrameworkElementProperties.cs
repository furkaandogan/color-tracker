using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI.AttachedProperties
{    /// <summary>
    /// FrameworkElement attached properties container
    /// </summary>
    public static class FrameworkElementProperties
    {

        /// <summary>
        /// CornerRadius attached property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty;

        /// <summary>
        /// Static constructor
        /// </summary>
        static FrameworkElementProperties()
        {
            CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(FrameworkElementProperties), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsRender));
            HideCloseProperty = DependencyProperty.RegisterAttached("HideClose", typeof(bool), typeof(FrameworkElementProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        }

        /// <summary>
        /// Gets the corner radius.
        /// </summary>
        /// <param name="obj">Owner dependency object</param>
        /// <returns>Corner radius value</returns>
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        /// <summary>
        /// Sets the corner radius.
        /// </summary>
        /// <param name="obj">Owner dependency object</param>
        /// <param name="value">Corner radius value</param>
        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Gets the corner radius.
        /// </summary>
        /// <param name="obj">Owner dependency object</param>
        /// <returns>Corner radius value</returns>
        public static bool GetHideClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(HideCloseProperty);
        }

        /// <summary>
        /// Sets the corner radius.
        /// </summary>
        /// <param name="obj">Owner dependency object</param>
        /// <param name="value">Corner radius value</param>
        public static void SetHideClose(DependencyObject obj, bool value)
        {
            obj.SetValue(HideCloseProperty, value);
        }



        /// <summary>
        /// To hide the close button for notifications
        /// </summary>
        public static readonly DependencyProperty HideCloseProperty;



    }
}
