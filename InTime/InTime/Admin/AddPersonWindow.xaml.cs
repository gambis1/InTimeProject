﻿using System;
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
    /// <summary>
    /// Logica di interazione per AddPerson.xaml
    /// </summary>
    public partial class AddPersonWindow : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();

        public AddPersonWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
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
            }
            else
            {
                MessageBox.Show("Il nome della persona non è stato inserito. Inserire un nome di persona valido, quindi riprovare.", "Nome di persona non valido", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AdministratorWindow mainWindow = Application.Current.Windows.OfType<AdministratorWindow>().FirstOrDefault();
            if(mainWindow != null)
            {
                mainWindow.GetDbPerson();
            }
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}