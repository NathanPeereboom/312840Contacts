/*Nathan Peereboom
 *April 17, 2020
 *Unit 2 Summative, contact class
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _312840Contacts
{
    class Contact
    {
        private string firstName;
        private string lastName;
        private DateTime birthday;
        private string email;

        /// <summary>
        /// Stores data of a contact
        /// </summary>
        /// <param name="fName">First name</param>
        /// <param name="lName">Last name</param>
        /// <param name="bDay">Birthday</param>
        /// <param name="address">Email address</param>
        public Contact(string fName, string lName, DateTime bDay, string address)
        {
            firstName = fName;
            lastName = lName;
            birthday = bDay;
            email = address;
        }

        /// <summary>
        /// Returns the contact's full name
        /// </summary>
        /// <returns></returns>
        public string getFullName()
        {
            return firstName + " " + lastName;
        }

        /// <summary>
        /// Returns the contact's first name
        /// </summary>
        /// <returns></returns>
        public string getFirstName()
        {
            return firstName;
        }

        /// <summary>
        /// Returns the contact's last name
        /// </summary>
        /// <returns></returns>
        public string getLastName()
        {
            return lastName;
        }

        /// <summary>
        /// Returns the contact's birthday as a DateTime
        /// </summary>
        /// <returns></returns>
        public DateTime getBirthday()
        {
            return birthday;
        }

        /// <summary>
        /// Returns the contact's email address
        /// </summary>
        /// <returns></returns>
        public string getEmail()
        {
            return email;
        }

        /// <summary>
        /// Updates the data of the contact class
        /// </summary>
        /// <param name="fName">First name</param>
        /// <param name="lName">Last name</param>
        /// <param name="bDay">Birthday</param>
        /// <param name="address">Email address</param>
        public void saveChanges(string fName, string lName, DateTime bDay, string address)
        {
            firstName = fName;
            lastName = lName;
            birthday = bDay;
            email = address;
        }
    }
}
