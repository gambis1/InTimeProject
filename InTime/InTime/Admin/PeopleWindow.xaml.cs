using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace InTime.Admin
{
    public partial class PeopleWindow : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();

        public PeopleWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            DbSet<Person> personDbList = intimeDb.People;
            List<Person> personList = (from Person in personDbList
                          orderby Person.PersonName
                          select Person).ToList();

            // BINDING
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = personList;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string name = PersonName.Text;

            if (name != "")
            {
                InTime.Person newPerson = new Person();
                newPerson.PersonName = name;
                newPerson.AccessCode = Guid.NewGuid();

                intimeDb.People.Add(newPerson);
                intimeDb.SaveChanges();

                PopulateDataGrid();
            }
            else
            {
                MessageBox.Show("Il nome della persona non è stato inserito. Inserire un nome di persona valido, quindi riprovare.", "Nome di persona non valido", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}