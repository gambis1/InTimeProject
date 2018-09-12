using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InTime.User
{
    /// <summary>
    /// Interaction logic for FirstLogin.xaml
    /// </summary>
    public partial class FirstLogin : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();

        public FirstLogin()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DbSet<Person> personDbList = intimeDb.People;

            try
            {
                Guid uniqueidInput = new Guid(UniqueIdInput.Text);

                try
                {
                    Person user = (from Person in personDbList
                                   where Person.AccessCode == uniqueidInput
                                   select Person).Single();

                    Properties.Settings.Default.UniqueIdentifier = uniqueidInput;
                    Properties.Settings.Default.Save();

                    MessageBox.Show("Hai effettuato l'accesso come " + user.PersonName, "Successo");
                    this.Close();
                }
                catch (Exception a)
                {
                    MessageBox.Show("Nessun utente associato a questo codice.", "Errore");
                }
            }
            catch (Exception a)
            {
                MessageBox.Show("Inserisci un codice utente valido.", "Errore");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
