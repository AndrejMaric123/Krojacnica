using Krojacnica.Views;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Krojacnica
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = Krojacnica.Properties.Settings.Default;

            if (settings.RememberMe && settings.LoggedUserId > 0)
            {
                using var db = DbContextFactory.Create();
                var zaposleni = db.zaposlenis
                    .Include(z => z.admin)
                    .FirstOrDefault(z => z.osoba_id == settings.LoggedUserId);

                if (zaposleni != null)
                {
                    if (settings.LoggedUserRole == "Admin")
                        new AdminWindow(zaposleni).Show();
                    

                    return;
                }
            }

            // Ako nije logovan, pokaži login prozor
            new LoginWindow().Show();
        }

    }

}
