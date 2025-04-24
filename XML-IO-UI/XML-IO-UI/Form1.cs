using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace XML_IO_UI
{
    public partial class Form1 : Form
    {
        private List<Profile> allProfiles = new List<Profile>();

        public Form1()
        {
            InitializeComponent();
            profileListBox.DrawMode = DrawMode.OwnerDrawFixed;
            profileListBox.ItemHeight = 50;
            LoadProfiles();
            DisplayProfiles(allProfiles);
            nameTextBox.TextChanged += filterTextBox_TextChanged;
            ageTextBox.TextChanged += filterTextBox_TextChanged;
            emailTextBox.TextChanged += filterTextBox_TextChanged;
            addressTextBox.TextChanged += filterTextBox_TextChanged;
            profileListBox.DrawItem += profileListBox_DrawItem;
        }

        private void LoadProfiles()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("./profiles.xml");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Error: profiles.xml not found!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (XmlException ex)
            {
                MessageBox.Show(string.Format("Error loading XML: {0}", ex.Message), "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XmlNodeList profileNodes = doc.GetElementsByTagName("profile");

            foreach (XmlNode profileNode in profileNodes)
            {
                string name = profileNode["name"] != null ? profileNode["name"].InnerText : "";
                string ageText = profileNode["age"] != null ? profileNode["age"].InnerText : "";
                int age;
                if (!int.TryParse(ageText, out age))
                {
                    age = 0;
                }
                string address = profileNode["address"] != null ? profileNode["address"].InnerText : "";
                string email = profileNode["email"] != null ? profileNode["email"].InnerText : "";

                allProfiles.Add(new Profile { Name = name, Age = age, Address = address, Email = email });
            }
        }

        private void DisplayProfiles(List<Profile> profilesToDisplay)
        {
            profileListBox.Items.Clear();
            foreach (Profile profile in profilesToDisplay)
            {
                profileListBox.Items.Add(string.Format("Name: {0}\nAge: {1}\nAddress: {2}\nEmail: {3}",
                                                     profile.Name, profile.Age, profile.Address, profile.Email));
            }
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            string nameFilter = nameTextBox.Text.ToLower();
            string ageFilterText = ageTextBox.Text;
            string addressFilter = addressTextBox.Text.ToLower();
            string emailFilter = emailTextBox.Text.ToLower();

            List<Profile> filteredProfiles = new List<Profile>();
            foreach (Profile profile in allProfiles)
            {
                // Debugging output to the console:
                System.Diagnostics.Debug.WriteLine("Profile:");
                System.Diagnostics.Debug.WriteLine("  Name: " + profile.Name + ", Address: " + profile.Address + ", Email: " + profile.Email);
                System.Diagnostics.Debug.WriteLine("Filters:");
                System.Diagnostics.Debug.WriteLine("  NameFilter: " + nameFilter + ", AddressFilter: " + addressFilter + ", EmailFilter: " + emailFilter);

                bool nameMatch = profile.Name.ToLower().Contains(nameFilter);
                bool ageMatch = string.IsNullOrEmpty(ageFilterText) || profile.Age.ToString() == ageFilterText;
                bool addressMatch = profile.Address.ToLower().Contains(addressFilter);
                bool emailMatch = profile.Email.ToLower().Contains(emailFilter);

                System.Diagnostics.Debug.WriteLine("  NameMatch: " + nameMatch + ", AgeMatch: " + ageMatch + ", AddressMatch: " + addressMatch + ", EmailMatch: " + emailMatch);

                if (nameMatch && ageMatch && addressMatch && emailMatch)
                {
                    filteredProfiles.Add(profile);
                }
            }

            DisplayProfiles(filteredProfiles);
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        public class Profile
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
        }

        private void profileListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0 && e.Index < profileListBox.Items.Count)
            {
                string itemText = profileListBox.Items[e.Index].ToString();
                string[] lines = itemText.Split('\n');

                using (SolidBrush textBrush = new SolidBrush(e.ForeColor))
                {
                    float y = e.Bounds.Y;
                    foreach (string line in lines)
                    {
                        e.Graphics.DrawString(line, e.Font, textBrush, e.Bounds.X, y);
                        y += e.Font.GetHeight();
                    }
                }
            }
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }
        }
    }
}
