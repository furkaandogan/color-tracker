using System.Windows;
using UI.Views;
using WindowsFormsApplication1;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            StartScreen MVview = new StartScreen();
            MVview.Show();
            base.OnStartup(e);
        }
    }
}
