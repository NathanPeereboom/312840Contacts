/*Nathan Peereboom
 * April 17, 2020
 * Unit 2 Summative, program to store contact information
 */
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _312840Contacts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Lists
        List<Contact> contacts = new List<Contact>();
        List<ComboBoxItem> contactBoxes = new List<ComboBoxItem>();

        public MainWindow()
        {
            InitializeComponent();
            viewDefaultInterface();

            //Read from file
            System.IO.StreamReader sr = new System.IO.StreamReader("contacts.txt");
            while (!sr.EndOfStream)
            {
                string tempFirstName;
                string tempLastName;
                DateTime tempBirthday;
                int tempYear;
                int tempMonth;
                int tempDay;
                string tempEmail;

                string line = sr.ReadLine();
                string[] items = line.Split(new char[] { ',' });
                tempFirstName = items[0];
                tempLastName = items[1];
                int.TryParse(items[2], out tempYear);
                int.TryParse(items[3], out tempMonth);
                int.TryParse(items[4], out tempDay);
                tempBirthday = new DateTime(tempYear, tempMonth, tempDay);
                tempEmail = items[5];
                Contact contact = new Contact(tempFirstName, tempLastName, tempBirthday, tempEmail);
                contacts.Add(contact);
                
            }
            sr.Close();

            //comboContact Setup
            foreach(Contact contact in contacts)
            {
                
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = contact.getFullName();
                contactBoxes.Add(comboBoxItem);
                comboContact.Items.Add(comboBoxItem);
            }

            //comboYear Setup
            for (int i = DateTime.Now.Year; i >= 1900; i--)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.ToString();
                comboYear.Items.Add(comboBoxItem);
            }

            //comboMonth Setup
            for (int i = 1; i <= 12; i++)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.ToString();
                comboMonth.Items.Add(comboBoxItem);
            }

            //comboDay Setup
            for (int i = 1; i <= 31; i++)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = i.ToString();
                comboDay.Items.Add(comboBoxItem);
            }
        }

        //When user selects contact
        private void comboContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Which contact did user select?
            if (comboContact.SelectedValue != null)
            {
                viewContactInterface();
                string[] value = comboContact.SelectedValue.ToString().Split(new char[] { ':' });
                string selectedContact = value[1].Trim();
                foreach (Contact contact in contacts)
                {
                    if (selectedContact == contact.getFullName())
                    {
                        //Pull data from contact class
                        txtFirstName.Text = contact.getFirstName();
                        txtLastName.Text = contact.getLastName();
                        //Year
                        foreach (ComboBoxItem comboBoxItem in comboYear.Items)
                        {
                            string year = comboBoxItem.Content.ToString();
                            if (year == contact.getBirthday().Year.ToString())
                            {
                                int item = DateTime.Now.Year - contact.getBirthday().Year;
                                comboYear.SelectedValue = comboYear.Items[item];
                            }
                        }
                        //Month
                        foreach (ComboBoxItem comboBoxItem in comboMonth.Items)
                        {
                            string month = comboBoxItem.Content.ToString();
                            if (month == contact.getBirthday().Month.ToString())
                            {
                                int item = contact.getBirthday().Month - 1;
                                comboMonth.SelectedValue = comboMonth.Items[item];
                            }
                        }
                        //Day
                        foreach (ComboBoxItem comboBoxItem in comboDay.Items)
                        {
                            string day = comboBoxItem.Content.ToString();
                            if (day == contact.getBirthday().Day.ToString())
                            {
                                int item = contact.getBirthday().Day - 1;
                                comboDay.SelectedValue = comboDay.Items[item];
                            }
                        }
                        txtEmail.Text = contact.getEmail();
                    }
                }
            }
        }

        //Save changes to contact class
        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            
            //Check for errors
            if (allFieldsFilled())
            {
                //Store birthday values
                string[] yearValue = comboYear.SelectedValue.ToString().Split(new char[] { ':' });
                string[] monthValue = comboMonth.SelectedValue.ToString().Split(new char[] { ':' });
                string[] dayValue = comboDay.SelectedValue.ToString().Split(new char[] { ':' });
                int year;
                int month;
                int day;
                int.TryParse(yearValue[1], out year);
                int.TryParse(monthValue[1], out month);
                int.TryParse(dayValue[1], out day);

                if (isValidDate(year, month, day))
                {
                    if (allLegalCharacters())
                    {
                        if (isNotRepeat())
                        {
                            //Save changes
                            DateTime bDay = new DateTime(year, month, day);
                            lblFeedback.Foreground = Brushes.Green;
                            lblFeedback.Content = "Changes saved";
                            lblFeedback.Visibility = Visibility.Visible;
                            saveChanges(bDay);
                        }
                        //error: repeat
                        else
                        {
                            lblFeedback.Foreground = Brushes.Red;
                            lblFeedback.Content = "There is already a contact with that name.";
                            lblFeedback.Visibility = Visibility.Visible;
                        }
                    }
                    //error: illegal characters
                    else
                    {
                        lblFeedback.Foreground = Brushes.Red;
                        lblFeedback.Content = "Please don't ues the following characters: , : ; \\ \"";
                        lblFeedback.Visibility = Visibility.Visible;
                    }
                }
                //error: invalid date
                else
                { 
                    lblFeedback.Foreground = Brushes.Red;
                    lblFeedback.Content = "Invalid date selected";
                    lblFeedback.Visibility = Visibility.Visible;
                }
            }
            //error: empty fields
            else
            {
                lblFeedback.Foreground = Brushes.Red;
                lblFeedback.Content = "Please fill all fields";
                lblFeedback.Visibility = Visibility.Visible;
            }
        }

        //Check if date entered is a real/past date
        public bool isValidDate(int year, int month, int day)
        {
            bool isValidDate;
            switch (month)
            {
                //February (Leap year considered)
                case 2:
                    if (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))
                    {
                        if (day <= 29)
                        {
                            isValidDate = true;
                        }
                        else
                        {
                            isValidDate = false;
                        }
                    }
                    else
                    {
                        if (day <= 28)
                        {
                            isValidDate = true;
                        }
                        else
                        {
                            isValidDate = false;
                        }
                    }
                    break;
                //April
                case 4:
                    if (day <= 30)
                    {
                        isValidDate = true;
                    }
                    else
                    {
                        isValidDate = false;
                    }
                    break;
                //June
                case 6:
                    if (day <= 30)
                    {
                        isValidDate = true;
                    }
                    else
                    {
                        isValidDate = false;
                    }
                    break;
                //September
                case 9:
                    if (day <= 30)
                    {
                        isValidDate = true;
                    }
                    else
                    {
                        isValidDate = false;
                    }
                    break;
                //November
                case 11:
                    if (day <= 30)
                    {
                        isValidDate = true;
                    }
                    else
                    {
                        isValidDate = false;
                    }
                    break;
                //Months with 31 days
                default:
                    isValidDate = true;
                    break;
            }

            if (DateTime.Now.Year == year && (DateTime.Now.Month < month || (DateTime.Now.Month == month && DateTime.Now.Day < day)))
            {
                isValidDate = false;
                MessageBox.Show("Time traveler? Tell us your secrets!");
            }
            if (isValidDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Check if all textboxes and comboboxes are filled
        public bool allFieldsFilled()
        {
            bool isFirstNameFilled;
            bool isLastNameFilled;
            bool isBirthdayFilled;
            bool isEmailFilled;
            if (txtFirstName.Text.Length > 0)
            {
                isFirstNameFilled = true;
            }
            else
            {
                isFirstNameFilled = false;
            }
            if (txtLastName.Text.Length > 0)
            {
                isLastNameFilled = true;
            }
            else
            {
                isLastNameFilled = false;
            }
            if (txtEmail.Text.Length > 0)
            {
                isEmailFilled = true;
            }
            else
            {
                isEmailFilled = false;
            }
            if (comboYear.SelectedValue != null && comboMonth.SelectedValue != null && comboDay.SelectedValue != null)
            {
                isBirthdayFilled = true;
            }
            else
            {
                isBirthdayFilled = false;
            }

            if (isFirstNameFilled && isLastNameFilled && isEmailFilled && isBirthdayFilled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Make sure there are no character which could potentially break the program
        public bool allLegalCharacters()
        {
            bool illegalCharacter = false;
            for (int i = 0; i < txtFirstName.Text.Length; i++)
            {
                char characterCheck = txtFirstName.Text[i];
                if (characterCheck == ',' || characterCheck == ':' || characterCheck == ';' || characterCheck == '\\' || characterCheck == '"')
                {
                    illegalCharacter = true;
                    break;
                }
            }
            for (int i = 0; i < txtLastName.Text.Length; i++)
            {
                char characterCheck = txtLastName.Text[i];
                if (characterCheck == ',' || characterCheck == ':' || characterCheck == ';' || characterCheck == '\\' || characterCheck == '"')
                {
                    illegalCharacter = true;
                    break;
                }
            }
            for (int i = 0; i < txtEmail.Text.Length; i++)
            {
                char characterCheck = txtEmail.Text[i];
                if (characterCheck == ',' || characterCheck == ':' || characterCheck == ';' || characterCheck == '\\' || characterCheck == '"')
                {
                    illegalCharacter = true;
                    break;
                }
            }

            if (illegalCharacter)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Make sure there isn't another contact with the same name
        public bool isNotRepeat()
        {
            bool repeatFound = false;
            foreach (Contact contact in contacts)
            {
                //if there is a match
                if (txtFirstName.Text + " " + txtLastName.Text == contact.getFullName())
                {
                    //if this is a contact being edited
                    if (comboContact.SelectedValue != null)
                    {
                        string[] value = comboContact.SelectedValue.ToString().Split(new char[] { ':' });
                        string selectedContact = value[1].Trim();
                        //Ignore match if it is the one being edited
                        if (contact.getFullName() != selectedContact)
                        {
                            repeatFound = true;
                            break;
                        }
                    }
                    //if this is a new contact being added
                    else
                    {
                        repeatFound = true;
                        break;
                    }
                }
            }

            if (repeatFound)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void saveChanges(DateTime bDay)
        {
            //Which contact is selected?
            string[] value = comboContact.SelectedValue.ToString().Split(new char[] { ':' });
            string selectedContact = value[1].Trim();

            foreach (Contact contact in contacts)
            {
                if (selectedContact == contact.getFullName())
                {
                    //Update combobox item
                    foreach (ComboBoxItem contactBox in contactBoxes)
                    {
                        if (contactBox.Content.ToString() == contact.getFullName())
                        {
                            contactBox.Content = txtFirstName.Text + " " + txtLastName.Text;
                        }
                    }

                    //Update data in contact class
                    contact.saveChanges(txtFirstName.Text, txtLastName.Text, bDay, txtEmail.Text);

                }
            }
        }

        private void btnNewContact_Click(object sender, RoutedEventArgs e)
        {
            //Update interface for adding new contact
            comboContact.SelectedValue = null;
            viewAddContactInterface();
            txtFirstName.Text = "";
            txtLastName.Text = "";
            comboYear.SelectedItem = null;
            comboMonth.SelectedItem = null;
            comboDay.SelectedItem = null;
            lblAge.Content = "";
            txtEmail.Text = "";
        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            

            if (allFieldsFilled())
            {
                //Store birthday
                string[] yearValue = comboYear.SelectedValue.ToString().Split(new char[] { ':' });
                string[] monthValue = comboMonth.SelectedValue.ToString().Split(new char[] { ':' });
                string[] dayValue = comboDay.SelectedValue.ToString().Split(new char[] { ':' });
                int year;
                int month;
                int day;
                int.TryParse(yearValue[1], out year);
                int.TryParse(monthValue[1], out month);
                int.TryParse(dayValue[1], out day);

                //Check for errors
                if (isValidDate(year, month, day))
                {
                    if (allLegalCharacters())
                    {
                        if (isNotRepeat())
                        {
                            DateTime bDay = new DateTime(year, month, day);
                            //Add new contact
                            Contact contact = new Contact(txtFirstName.Text, txtLastName.Text, bDay, txtEmail.Text);
                            contacts.Add(contact);

                            //Add new combobox item
                            ComboBoxItem comboBoxItem = new ComboBoxItem();
                            comboBoxItem.Content = contact.getFullName();
                            contactBoxes.Add(comboBoxItem);
                            comboContact.Items.Add(comboBoxItem);

                            //Update interface
                            viewDefaultInterface();
                            comboContact.SelectedItem = null;
                            txtFirstName.Text = "";
                            txtLastName.Text = "";
                            comboYear.SelectedItem = null;
                            comboMonth.SelectedItem = null;
                            comboDay.SelectedItem = null;
                            txtEmail.Text = "";
                        }
                        //error: repeat
                        else
                        {
                            lblFeedback.Foreground = Brushes.Red;
                            lblFeedback.Content = "There is already a contact with that name.";
                            lblFeedback.Visibility = Visibility.Visible;
                        }
                    }
                    //error: illegal characters
                    else
                    {
                        lblFeedback.Foreground = Brushes.Red;
                        lblFeedback.Content = "Please don't ues the following characters: , : ; \\ \"";
                        lblFeedback.Visibility = Visibility.Visible;
                    }
                }
                //error: invalid date
                else
                {
                    lblFeedback.Foreground = Brushes.Red;
                    lblFeedback.Content = "Invalid date selected";
                    lblFeedback.Visibility = Visibility.Visible;
                }
            }
            //error: empty fields
            else
            {
                lblFeedback.Foreground = Brushes.Red;
                lblFeedback.Content = "Please fill all fields";
                lblFeedback.Visibility = Visibility.Visible;
            }

            
        }

        //Cancel adding new contact
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            viewDefaultInterface();
            comboContact.SelectedItem = null;
        }

        private void btnRemoveContact_Click(object sender, RoutedEventArgs e)
        {
            //Which contact is selected?
            string[] value = comboContact.SelectedValue.ToString().Split(new char[] { ':' });
            string selectedContact = value[1].Trim();

            //Conformation
            MessageBoxResult removeConfirm = MessageBox.Show("Are you sure you want to remove " + selectedContact + " from your contact list?", "",
                MessageBoxButton.YesNo);
            if (removeConfirm == MessageBoxResult.Yes)
            {
                //Update interface
                viewDefaultInterface();
                comboContact.SelectedItem = null;
                txtFirstName.Text = "";
                txtLastName.Text = "";
                comboYear.SelectedItem = null;
                comboMonth.SelectedItem = null;
                comboDay.SelectedItem = null;
                lblAge.Content = "";
                txtEmail.Text = "";
                
                //Find selected contact
                foreach (Contact contact in contacts)
                {
                    if (contact.getFullName() == selectedContact)
                    {
                        //Remove class
                        contacts.Remove(contact);
                        foreach (ComboBoxItem comboBoxItem in contactBoxes)
                        {
                            if (comboBoxItem.Content.ToString() == contact.getFullName())
                            {
                                //Remove comboboxItem
                                contactBoxes.Remove(comboBoxItem);
                                comboContact.Items.Remove(comboBoxItem);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void viewDefaultInterface()
        {
            Application.Current.MainWindow.Height = 250;

            lblHeader.Content = "My Contacts";

            lblChooseContact.Visibility = Visibility.Visible;
            comboContact.Visibility = Visibility.Visible;
            spFirstName.Visibility = Visibility.Collapsed;
            spLastName.Visibility = Visibility.Collapsed;
            spEmail.Visibility = Visibility.Collapsed;
            spDateOfBirth.Visibility = Visibility.Collapsed;
            btnShowAge.Visibility = Visibility.Collapsed;
            lblAge.Visibility = Visibility.Collapsed;
            spContactControls.Visibility = Visibility.Collapsed;
            btnNewContact.Visibility = Visibility.Visible;
            btnAddContact.Visibility = Visibility.Collapsed;
            lblFeedback.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;

            btnNewContact.HorizontalAlignment = HorizontalAlignment.Center;
            btnNewContact.Margin = new Thickness(0,30,0,0);
        }

        public void viewContactInterface()
        {
            Application.Current.MainWindow.Height = 510;

            lblHeader.Content = "Edit Contact";

            lblChooseContact.Visibility = Visibility.Collapsed;
            comboContact.Visibility = Visibility.Visible;
            spFirstName.Visibility = Visibility.Visible;
            spLastName.Visibility = Visibility.Visible;
            spEmail.Visibility = Visibility.Visible;
            spDateOfBirth.Visibility = Visibility.Visible;
            btnShowAge.Visibility = Visibility.Visible;
            lblAge.Visibility = Visibility.Visible;
            lblAge.Content = "";
            spContactControls.Visibility = Visibility.Visible;
            btnNewContact.Visibility = Visibility.Visible;
            btnAddContact.Visibility = Visibility.Collapsed;
            lblFeedback.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;

            btnNewContact.HorizontalAlignment = HorizontalAlignment.Left;
            btnNewContact.Margin = new Thickness(30, 30, 0, 0);
        }

        public void viewAddContactInterface()
        {
            Application.Current.MainWindow.Height = 510;

            lblHeader.Content = "Add Contact";

            lblChooseContact.Visibility = Visibility.Collapsed;
            comboContact.Visibility = Visibility.Collapsed;
            spFirstName.Visibility = Visibility.Visible;
            spLastName.Visibility = Visibility.Visible;
            spEmail.Visibility = Visibility.Visible;
            spDateOfBirth.Visibility = Visibility.Visible;
            btnShowAge.Visibility = Visibility.Collapsed;
            lblAge.Visibility = Visibility.Collapsed;
            spContactControls.Visibility = Visibility.Collapsed;
            btnNewContact.Visibility = Visibility.Collapsed;
            btnAddContact.Visibility = Visibility.Visible;
            lblFeedback.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Visible;
        }

        private void btnShowAge_Click(object sender, RoutedEventArgs e)
        {
            if (comboYear.SelectedValue != null && comboMonth.SelectedValue != null && comboDay.SelectedValue != null)
            {
                int age;
                //Store birthday
                string[] yearValue = comboYear.SelectedValue.ToString().Split(new char[] { ':' });
                string[] monthValue = comboMonth.SelectedValue.ToString().Split(new char[] { ':' });
                string[] dayValue = comboDay.SelectedValue.ToString().Split(new char[] { ':' });
                int year;
                int month;
                int day;
                int.TryParse(yearValue[1], out year);
                int.TryParse(monthValue[1], out month);
                int.TryParse(dayValue[1], out day);

                if (isValidDate(year, month, day))
                {
                    DateTime bDay = new DateTime(year, month, day);

                    //Calculate age
                    age = DateTime.Now.Year - bDay.Year;
                    if (DateTime.Now.Month < bDay.Month)
                    {
                        age -= 1;
                    }
                    else if (DateTime.Now.Month == bDay.Month)
                    {
                        if (DateTime.Now.Day < bDay.Day)
                        {
                            age -= 1;
                        }
                    }
                    lblAge.Content = age;
                }
                else
                {
                    MessageBox.Show("Error: Invalid Date");
                    lblAge.Content = "";
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("contacts.txt");
            foreach(Contact contact in contacts)
            {
                sw.WriteLine(contact.getFirstName() + "," + contact.getLastName() + "," + contact.getBirthday().Year + "," + contact.getBirthday().Month 
                    + "," + contact.getBirthday().Day + "," + contact.getEmail());
            }
            sw.Flush();
            sw.Close();
        }
    }
}
