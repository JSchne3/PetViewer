using System;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace IS318_CPM3_Schneiter
{
    class TextFileClass
    {
        private string fileName; 

        //Basic constructor for the TextFileClass. The only variable needed is the pet ID code.
        public TextFileClass(string userFile)
        {
            SetFileName(userFile); 
        }


        //checks the validity of the userFile and then uses a string builder to create the file string.
        public void SetFileName( string userFile)
        {
            StringBuilder buffer = new StringBuilder();
            string linkStart = @".\";
            string linkEnd = @".txt"; 

            if (CheckValid(userFile) == true)
            {
                buffer.Append(linkStart);
                buffer.Append(userFile);
                buffer.Append(linkEnd);
                fileName = buffer.ToString(); 
            }
        }


        //This checks to make sure that the input is an integer. 
        public Boolean CheckValid(string userFile)
        {
            try
            {
                if (userFile.Length == 4)
                {
                    int tester = int.Parse(userFile);
                    return true;
                }
                else
                {
                    MessageBox.Show("Please enter a 4 digit number.");
                    return false; 
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a 4 digit number."); 
                return false; 
            }
            catch (OverflowException)
            {
                MessageBox.Show("Please enter a 4 digit number.");
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong, please try again."); 
                return false; 
            }
        }


        //The method for loading and returning the text file. 
        public string LoadFile()
        {
            string results; 
            
            //Check if file exists.
            if (File.Exists (fileName))
            {
                StreamReader stream;

                //Display the File contents. 
                try
                {
                    using (stream = new StreamReader (fileName))
                    {
                        results = stream.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    results = "There was an error displaying the pet's history records.";
                }
            }
            else
            {
                results = "No patient History recorded for the specified ID. Please erase this before saving if creating a new file.";
            }

            results += String.Format("\r\n\r\n ===== {0} ===== \r\n\r\n", DateTime.Now);
            return results;
        }


        //This is for saving any changes made to the pet history text files.
        public void SaveFile(string recordText)
        {
            if (File.Exists (fileName))
            {
                try
                {
                    using (StreamWriter fileWriter = new StreamWriter(fileName))
                    {
                        fileWriter.Write(recordText); 
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("There was an error saving your file.");
                }
            }
            else
            {
                using (StreamWriter fileWriter = File.AppendText(fileName))
                {
                    fileWriter.Write(recordText); 
                }
            }
        }


        //Remove the file if the pet is deleted from the database
        public void RemoveFile()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

    }
}
