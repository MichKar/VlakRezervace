using cviko;
using System;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTrain {

    public partial class MainWindow : Window {

        Train train;
        public MainWindow() {
            InitializeComponent();

            Person per = new Person("Petr", "Novák");
            Locomotive loc1 = new Locomotive(new Engine("diesel"), per);
            train = new Train(loc1);

            EconomyWagon eco1 = new EconomyWagon(40);
            NightWagon night1 = new NightWagon(50, 10);
            Hopper hop = new Hopper(180);
            EconomyWagon eco2 = new EconomyWagon(10);

            //eco1.ConnectWagon(train);
            //night1.ConnectWagon(train);
            //hop.ConnectWagon(train);
            //eco2.ConnectWagon(train);
            this.DataContext = train;

            cbWagonType.Items.Add("Business Wagon");
            cbWagonType.Items.Add("Economy Wagon");
            cbWagonType.Items.Add("Night Wagon");
            cbWagonType.Items.Add("Hopper");

            HandleChangeVisibility();
        }


        private void BtnAddWagon_Click(object sender, RoutedEventArgs e) {
            wagontype = cbWagonType.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine(wagontype);

            switch (wagontype) {
                case "Economy Wagon":
                    int numberOfChairs;
                    if (int.TryParse(tbFirst.Text, out numberOfChairs)) {
                        new EconomyWagon(numberOfChairs).ConnectWagon(train);
                    }
                    else {
                        MessageBox.Show("Incorrect typed number of chairs");
                    }
                    break;
                case "Hopper":
                    int tonnage;
                    if (int.TryParse(tbFirst.Text, out tonnage)) {
                        new Hopper(tonnage).ConnectWagon(train);
                    }
                    else {
                        MessageBox.Show("Incorrect tonnage");
                    }
                    break;
                case "Business Wagon":
                    if (int.TryParse(tbFirst.Text, out numberOfChairs)) {
                        string[] name = tbSecond.Text.Split(" ");
                
                        if (char.IsLetter(name[0][0]) && name.Length == 2) {
                            new BusinessWagon(new Person(name[0], name[1]), numberOfChairs).ConnectWagon(train);
                        }
                        else {
                            MessageBox.Show("The name must start with a letter and the name must contains first name and last name separated by a space.");
                        }

                    }
                    else {
                        MessageBox.Show("Incorrectly typed number of seats");
                    }
                    break;
                case "Night Wagon":
                    int numberOfBeds;
                    if (int.TryParse(tbFirst.Text, out numberOfChairs) && int.TryParse(tbSecond.Text, out numberOfBeds)) {
                        new NightWagon(numberOfChairs, numberOfBeds).ConnectWagon(train);
                    }
                    else {
                        MessageBox.Show("Incorrectly typed number of beds");
                    }
                    break;
            }
            ClearTextBoxes();
            HandleChangeVisibility();
        }

        string wagontype;

        private void CbWagonType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            wagontype = cbWagonType.SelectedItem.ToString();

            switch (wagontype) {
                case "Economy Wagon":
                    ChangeFirstLabelVisibility(Visibility.Visible);
                    ChangeSecondLabelVisibility(Visibility.Collapsed);
                    lbTbFirst.Content = "Number of seats";
                    break;
                case "Hopper":
                    ChangeFirstLabelVisibility(Visibility.Visible);
                    ChangeSecondLabelVisibility(Visibility.Collapsed);
                    lbTbFirst.Content = "Tonnage";
                    break;
                case "Business Wagon":
                    ChangeFirstLabelVisibility(Visibility.Visible);
                    ChangeSecondLabelVisibility(Visibility.Visible);
                    lbTbFirst.Content = "Number of seats";
                    lbTbSecond.Content = "Steward name";

                    break;
                case "Night Wagon":
                    ChangeFirstLabelVisibility(Visibility.Visible);
                    ChangeSecondLabelVisibility(Visibility.Visible);
                    lbTbFirst.Content = "Number of seats";
                    lbTbSecond.Content = "Number of beds";
                    break;
            }
            btnAddWagon.Visibility = Visibility.Visible;
            //HandleChangeVisibility();



        }

        private void ClearTextBoxes() {
            tbFirst.Text = "";
            tbSecond.Text = "";
        }
        private void ChangeFirstLabelVisibility(Visibility visibility) {
            tbFirst.Visibility = visibility;
            lbTbFirst.Visibility = visibility;
        }

        private void ChangeSecondLabelVisibility(Visibility visibility) {
            tbSecond.Visibility = visibility;
            lbTbSecond.Visibility = visibility;
        }

        private void ChangeReservationVisibility(Visibility visibility) {
            lbTbReserveSeat.Visibility = visibility;
            tbReserveSeat.Visibility = visibility;
            btnReserve.Visibility = visibility;
            lbSelectedSeat.Visibility = visibility;
        }

        private void BtnDeleteWagon_Click(object sender, RoutedEventArgs e) {
            if ((IConnection)listBoxWagons.SelectedItem != null) {
                btnDeleteWagon.Visibility = Visibility.Visible;
                ((IConnection)listBoxWagons.SelectedItem).DisconnectWagon(train);
                ClearTextBoxes();
                if (listBoxWagons.Items.Count == 0) {

                    btnDeleteWagon.Visibility = Visibility.Collapsed;

                }
            }
            HandleChangeVisibility();
            ChangeReservationVisibility(Visibility.Collapsed);


        }

        private void HandleChangeVisibility() {
            listBoxWagons.Visibility = listBoxWagons.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                btnDeleteWagon.Visibility = listBoxWagons.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

        }

        private void ListBoxWagons_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (listBoxWagons.SelectedItem != null) {
                //btnDeleteWagon.Visibility = Visibility.Visible;

                string selectedWagonClassName = listBoxWagons.SelectedItem.GetType().Name;
                lbSelectedSeat.Content = "Selected wagon nr. " + (listBoxWagons.SelectedIndex + 1);
                if (selectedWagonClassName != nameof(Hopper)) {
                    ChangeReservationVisibility(Visibility.Visible);
                }
                else {
                    ChangeReservationVisibility(Visibility.Collapsed);
                }


            }
            //HandleChangeVisibility();

        }

        private void BtnReserve_Click(object sender, RoutedEventArgs e) {
            if (int.TryParse(tbReserveSeat.Text, out int seatToReserved)) {
                MessageBox.Show(train.ReserveSeat((listBoxWagons.SelectedIndex + 1), seatToReserved));
            }
            else {
                MessageBox.Show("Invalid input for seat number. Please enter a valid number.");
            }
            tbReserveSeat.Text = "";
        }
    }
}