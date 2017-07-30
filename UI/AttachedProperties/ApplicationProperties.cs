using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UI.AttachedProperties
{
    /// <summary>
    /// Attached Properties for Application Keybindings
    /// </summary>
    public class ApplicationProperties
    {
        /// <summary>
        /// Dependency property decleration for "KeyBindings"
        /// </summary>
        public static readonly DependencyProperty KeyBindingsProperty;

        /// <summary>
        /// Static constructor
        /// </summary>
        static ApplicationProperties()
        {
          //  KeyBindingsProperty = DependencyProperty.RegisterAttached("KeyBindings", typeof(InputBindingCollection), typeof(EventToCommandBehaviors), new PropertyMetadata(OnKeyBindingsChanged));
        }

        #region Get/Set KeyBindings

        /// <summary>
        /// Get KeyBindings property value
        /// </summary>
        /// <param name="obj">Owner dependency object</param>
        /// <returns>Property value input binding collection</returns>
        public static InputBindingCollection GetKeyBindings(DependencyObject obj)
        {
            return (InputBindingCollection)obj.GetValue(KeyBindingsProperty);
        }

        /// <summary>
        /// Set KeyBindings property value
        /// </summary>
        /// <param name="obj">Owner dependency object</param>
        /// <param name="value">Property value input binding collection</param>
        public static void SetKeyBindings(DependencyObject obj, InputBindingCollection value)
        {
            obj.SetValue(KeyBindingsProperty, value);
        }

        #endregion

        #region Internal Members

        private static void OnKeyBindingsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var element = dependencyObject as UIElement;
            if (element != null)
                element.PreviewKeyDown += OnUiElementPreviewKeyDown;
        }

        private static void OnUiElementPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var element = sender as UIElement;
            if (element == null)
                return;

            var keyBindings = GetKeyBindings(element);
            if (keyBindings == null)
                return;

            foreach (KeyBinding keyBinding in keyBindings)
            {
                if (keyBinding.Key == keyEventArgs.Key && keyBinding.Modifiers == Keyboard.Modifiers && keyBinding.Command != null)
                {
                    if (keyBinding.Command.CanExecute(keyBinding.CommandParameter))
                    {
                        keyBinding.Command.Execute(keyBinding.CommandParameter);
                    }
                    else
                    {
                        var shortCut = keyBinding.Key.ToString();
                        if (keyBinding.Modifiers != ModifierKeys.None)
                            shortCut = string.Format("{0}+{1}", keyBinding.Modifiers, keyBinding.Key);

                        //Notification.NewLocalizedNotification("$ShortcutCannotExecute", shortCut);
                    }

                    break;
                }
            }
        }

        #endregion
    }
}
