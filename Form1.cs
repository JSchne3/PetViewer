using System;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using System.Data.Entity;

namespace IS318_CPM3_Schneiter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //For the search.
        private void SDSearchButton_Click(object sender, EventArgs e)
        {
            DatabaseSearch searchDB = new DatabaseSearch();

            if (SDPetNameRadio.Checked) 
                SDSearchResults.Text = searchDB.SearchPetNames(SDSearchText.Text);
            else if (SDPetTypeRadio.Checked) 
                SDSearchResults.Text = searchDB.SearchPetSpecies(SDSearchText.Text);
            else if (SDOwnerNameRadio.Checked) 
                SDSearchResults.Text = searchDB.SearchOwners(SDSearchText.Text);
            else if (SDPetIDRadio.Checked)
                SDSearchResults.Text = searchDB.SearchPetID(SDSearchText.Text);
            else 
                SDSearchResults.Text = "Please click one of the search options.";


        }

        //Search all pets.
        private void SDSearchAllButton_Click(object sender, EventArgs e)
        {
            DatabaseSearch listDB = new DatabaseSearch();
            SDSearchResults.Text = listDB.ListAll();
        }

        //Search All Owners
        private void SearchAllOwners_Click(object sender, EventArgs e)
        {
            DatabaseSearch listOwnersDB = new DatabaseSearch();
            SDSearchResults.Text = listOwnersDB.ListOwners();
        }




        //this Button is from the Patient Records Tab. It loads in the information from the database and from the file records for each pet. 
        private void PRLoadButton_Click(object sender, EventArgs e)
        {
            DatabaseSearch petLoader = new DatabaseSearch();
            
            //this makes sure we validate the pet exists before continuing with anything else. 
            if (petLoader.PetExists(PRSearchText.Text))
            {
                //Load the file.
                TextFileClass loadHistory = new TextFileClass(PRSearchText.Text);
                PRHistoryText.Text = loadHistory.LoadFile();
                //Enable the ability to save the file. 
                PRSaveFileChangesButton.Enabled = true;

                //Load the basic pet data.
                ChangeLabels(petLoader.GetAllPetInfo(PRSearchText.Text));

                //Load the medicines for that pet. 
                PRMedicationsText.Text = petLoader.GetMedications(PRSearchText.Text);

                //load the vaccines for that pet if dog or cat. 
                PRVacText.Text = petLoader.GetVaccinations(PRSearchText.Text);

            }
            else
            {
                PRSaveFileChangesButton.Enabled = false; 
                MessageBox.Show("Pet Not Found. Please try again."); 
                ClearLabelsAndText(); 
            }
        }




        //This is for the save changes to history file. It is enabled after the database has been properly loaded. 
        //Even if a history file does not exist for the pet, as long as there is a database entry for it, one can be created. 
        //Otherwise this will remain disabled. 
        private void PRSaveFileChangesButton_Click(object sender, EventArgs e)
        {
            TextFileClass saveHistory = new TextFileClass(PRSearchText.Text);
            saveHistory.SaveFile(PRHistoryText.Text); 
        }

        //This is for removing a pet from the database
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            DatabaseAlter removingPet = new DatabaseAlter();

            if (removingPet.PetExists(PetText.Text) == true)
                removingPet.RemovePet(PetText.Text);
            else
                MessageBox.Show("No pet exists with this ID.");

            ClearPetDetails();
        }

        private void RemoveOwnerButton_Click(object sender, EventArgs e)
        {
            DatabaseAlter removingOwner = new DatabaseAlter();

            if (removingOwner.OwnerExists(OwnerText.Text) == true)
                removingOwner.RemoveOwner(OwnerText.Text);
            else
                MessageBox.Show("No owner exists with this ID.");

            ClearOwnerDetails();
        }

        //Adds a new owner into the database.
        private void AddOwnerButton_Click(object sender, EventArgs e)
        {
            DatabaseAlter addingOwner = new DatabaseAlter();
            addingOwner.AddOwner(firstNameText.Text, lastNameText.Text, addOwnerPhoneText.Text);

            //then clear all details.
            ClearOwnerDetails();
        }

        //adds a new pet into the database.
        private void AddPetButton_Click(object sender, EventArgs e)
        {
            Boolean gender;
            Boolean sterilized;

            if (femaleRadioButton.Checked)
                gender = true;
            else
                gender = false;

            if (sterileYesRadio.Checked)
                sterilized = true;
            else
                sterilized = false;

            DatabaseAlter addingPet = new DatabaseAlter();
            addingPet.AddPet(addPetNameText.Text, petOwnerID.Text, addSpeciesText.Text, addBreedText.Text, addBirthYearText.Text, gender, sterilized);

            //Clear all text inputs now.
            ClearPetDetails();
        }

        //Loads an Owner's info for editing. Enables the relevant clear and update buttons. 
        private void LoadOwnerButton_Click(object sender, EventArgs e)
        {
            DatabaseSearch loadOwner = new DatabaseSearch();

            if (loadOwner.OwnerExists(OwnerText.Text) == true)
            {
                Stack ownerInfoStack = loadOwner.GetOwnerInfo(OwnerText.Text);
                firstNameText.Text = String.Format("{0}", ownerInfoStack.Pop());
                lastNameText.Text = String.Format("{0}", ownerInfoStack.Pop());
                addOwnerPhoneText.Text = String.Format("{0}", ownerInfoStack.Pop());

                ClearOwner.Enabled = true;
                UpdateOwnerButton.Enabled = true;
                AddOwnerButton.Enabled = false;
                OwnerText.ReadOnly = true; 
            }
            else
                MessageBox.Show("No owner exists with this ID.");
        }

        //Clears the owner text fields and relocks the clear and update buttons while enabling the add button. 
        private void ClearOwner_Click(object sender, EventArgs e)
        {
            ClearOwnerDetails();
        }

        //Updates the loaded owner. 
        private void UpdateOwnerButton_Click(object sender, EventArgs e)
        {
            Stack updateOwnerInfo = new Stack();

            updateOwnerInfo.Push(OwnerText.Text);
            updateOwnerInfo.Push(firstNameText.Text);
            updateOwnerInfo.Push(lastNameText.Text);
            updateOwnerInfo.Push(addOwnerPhoneText.Text);

            DatabaseAlter updateOwner = new DatabaseAlter(); 

            if (updateOwner.OwnerExists(OwnerText.Text))
            {
                updateOwner.UpdateOwner(updateOwnerInfo);
            }
            else
            {
                MessageBox.Show("An Owner with the specified ID does not exist.");
            }

            ClearOwnerDetails();
        }

        //Loads the pet info.
        private void LoadPetButton_Click(object sender, EventArgs e)
        {
            DatabaseSearch loadPet = new DatabaseSearch();

            if (loadPet.PetExists(PetText.Text) == true)
            {
                Stack petInfoStack = loadPet.GetPetInfo(PetText.Text);

                addPetNameText.Text = String.Format("{0}", petInfoStack.Pop());
                petOwnerID.Text = String.Format("{0}", petInfoStack.Pop());
                addSpeciesText.Text = String.Format("{0}", petInfoStack.Pop());
                addBreedText.Text = String.Format("{0}", petInfoStack.Pop());
                addBirthYearText.Text = String.Format("{0}", petInfoStack.Pop());

                if (loadPet.GetPetGender(PetText.Text) == true)
                    femaleRadioButton.Checked = true;
                else
                    maleRadioButton.Checked = true;

                if (loadPet.GetPetFixed(PetText.Text) == true)
                    sterileYesRadio.Checked = true;
                else
                    sterileNoRadio.Checked = true;


                ClearPet.Enabled = true;
                UpdatePetButton.Enabled = true;
                AddPetButton.Enabled = false;
                PetText.ReadOnly = true;
            }
            else
                MessageBox.Show("No pet exists with this ID.");
        }

        //Clears and resets the pet info.
        private void ClearPet_Click(object sender, EventArgs e)
        {
            ClearPetDetails();
        }

        //updates the specified pet.
        private void UpdatePetButton_Click(object sender, EventArgs e)
        {
            Stack updatePetInfo = new Stack();

            updatePetInfo.Push(PetText.Text);
            updatePetInfo.Push(addPetNameText.Text);
            updatePetInfo.Push(petOwnerID.Text);
            updatePetInfo.Push(addSpeciesText.Text);
            updatePetInfo.Push(addBreedText.Text);
            updatePetInfo.Push(addBirthYearText.Text);

            DatabaseAlter updatePet = new DatabaseAlter();

            if (updatePet.PetExists(PetText.Text))
            {
                updatePet.UpdatePet(updatePetInfo, GetGender(), GetSterilized());
            }
            else
            {
                MessageBox.Show("No pet exists with this ID.");
            }

            ClearPetDetails();
        }






        //Additional methods for Loading Pet Info. 
        private void ChangeLabels(Stack petInfo)
        {
            PRPetGenderLabel.Text = String.Format("{0}", petInfo.Pop());
            PRPetDOBLabel.Text = String.Format("{0}", petInfo.Pop());
            PRSpeciesLabel.Text = String.Format("{0}", petInfo.Pop());
            PRBreedLabel.Text = String.Format("{0}", petInfo.Pop()); 
            PRPetNameLabel.Text = String.Format("{0}", petInfo.Pop());

            PRPetNameLabel.Text += String.Format("{0}", petInfo.Pop());
        }

        private void ClearLabelsAndText()
        {
            PRPetGenderLabel.Text = "Gender";
            PRPetDOBLabel.Text = "DOB";
            PRSpeciesLabel.Text = "Species";
            PRBreedLabel.Text = "Breed";
            PRPetNameLabel.Text = "Pet Name";

            PRMedicationsText.Text = "";
            PRVacText.Text = "";
            PRHistoryText.Text = "";
        }




        //Additional method for clearing owner info updates. 
        private void ClearOwnerDetails()
        {
            firstNameText.Text = "";
            lastNameText.Text = "";
            addOwnerPhoneText.Text = "";
            OwnerText.Text = "";

            ClearOwner.Enabled = false;
            UpdateOwnerButton.Enabled = false;
            AddOwnerButton.Enabled = true;
            OwnerText.ReadOnly = false;
        }

        private void ClearPetDetails()
        {
            addPetNameText.Text = "";
            petOwnerID.Text = "";
            addSpeciesText.Text = "";
            addBreedText.Text = "";
            addBirthYearText.Text = "";
            PetText.Text = "";
            femaleRadioButton.Checked = true;
            sterileYesRadio.Checked = true;

            ClearPet.Enabled = false;
            UpdatePetButton.Enabled = false;
            AddPetButton.Enabled = true;
            PetText.ReadOnly = false;
        }




        //Methods for determing the value of male or female/ sterilized or not. 
        private Boolean GetGender()
        {
            if (femaleRadioButton.Checked == true)
                return true;
            else
                return false;
        }

        private Boolean GetSterilized()
        {
            if (sterileYesRadio.Checked == true) 
                return true; 
            else 
                return false; 
        }




        //Methods for vaccinations. 
        private void LoadPetMedsButton_Click(object sender, EventArgs e)
        {
            //This method loads the vaccines
            GetPetVaccines();

            //This method loads the medications
            LoadPetMeds();
        }

        private void ClearPetMedsButton_Click(object sender, EventArgs e)
        {
            DisableAllChecks();
        }

        private void UpdateVaccines_Click(object sender, EventArgs e)
        {
            DatabaseAlter updateVacs = new DatabaseAlter();

            if (updateVacs.PetExists(loadPetMedsText.Text) == true && updateVacs.CheckSpecies(loadPetMedsText.Text) == "Cat")
            {
                updateVacs.UpdateCatVacs(loadPetMedsText.Text, FelineLeukemiaCheck.Checked, CatRabiesCheck.Checked, FelinePanleukCheck.Checked, FelineCalcivirusCheck.Checked, FelineHerpesVirusCheck.Checked);
            }
            else if (updateVacs.PetExists(loadPetMedsText.Text) == true && updateVacs.CheckSpecies(loadPetMedsText.Text) == "Dog")
            {
                updateVacs.UpdateDogVacs(loadPetMedsText.Text, CanineParvoCheck.Checked, DistemperCheck.Checked, CanineHepCheck.Checked, DogRabiesCheck.Checked);
            }
            else
                MessageBox.Show("No animal besides cats and dogs have any vaccination records at this moment in time.");

            DisableAllChecks();
        }




        private void ActiveCatChecks()
        {
            FelineCalcivirusCheck.Enabled = true;
            FelineHerpesVirusCheck.Enabled = true;
            FelineLeukemiaCheck.Enabled = true;
            FelinePanleukCheck.Enabled = true;
            CatRabiesCheck.Enabled = true;
            UpdateVaccines.Enabled = true;

            loadPetMedsText.ReadOnly = true;
            LoadPetMedsButton.Enabled = false;

            ClearPetMedsButton.Enabled = true;
        }

        private void ActiveDogChecks()
        {
            CanineHepCheck.Enabled = true;
            CanineParvoCheck.Enabled = true;
            DogRabiesCheck.Enabled = true;
            DistemperCheck.Enabled = true;
            UpdateVaccines.Enabled = true;

            loadPetMedsText.ReadOnly = true;
            LoadPetMedsButton.Enabled = false;

            ClearPetMedsButton.Enabled = true;
        }

        private void DisableAllChecks()
        {
            FelineCalcivirusCheck.Enabled = false;
            FelineHerpesVirusCheck.Enabled = false;
            FelineLeukemiaCheck.Enabled = false;
            FelinePanleukCheck.Enabled = false;
            CatRabiesCheck.Enabled = false;

            CanineHepCheck.Enabled = false;
            CanineParvoCheck.Enabled = false;
            DogRabiesCheck.Enabled = false;
            DistemperCheck.Enabled = false; 

            UpdateVaccines.Enabled = false;

            loadPetMedsText.ReadOnly = false;
            LoadPetMedsButton.Enabled = true;

            ClearPetMedsButton.Enabled = false; 
        }




        private void GetPetVaccines()
        {
            DatabaseSearch getPetVacs = new DatabaseSearch();

            if (getPetVacs.PetExists(loadPetMedsText.Text) == true && getPetVacs.CheckSpecies(loadPetMedsText.Text) == "Cat")
            {
                ActiveCatChecks();

                Stack vacInfo = getPetVacs.ReturnCatVaccinations(loadPetMedsText.Text);

                if (vacInfo.Count > 0)
                {
                    string fLEU = String.Format("{0}", vacInfo.Pop());
                    string rabies = String.Format("{0}", vacInfo.Pop());
                    string fPANLEU = String.Format("{0}", vacInfo.Pop());
                    string fCALI = String.Format("{0}", vacInfo.Pop());
                    string fHERPES = String.Format("{0}", vacInfo.Pop());

                    if (fLEU == "True")
                        FelineLeukemiaCheck.Checked = true;

                    if (rabies == "True")
                        CatRabiesCheck.Checked = true;

                    if (fPANLEU == "True")
                        FelinePanleukCheck.Checked = true;

                    if (fCALI == "True")
                        FelineCalcivirusCheck.Checked = true;

                    if (fHERPES == "True")
                        FelineHerpesVirusCheck.Checked = true;

                }
                else
                {
                    MessageBox.Show("Vaccine records unavailable.");
                }

            }
            else if (getPetVacs.PetExists(loadPetMedsText.Text) == true && getPetVacs.CheckSpecies(loadPetMedsText.Text) == "Dog")
            {
                ActiveDogChecks();

                Stack vacInfo = getPetVacs.ReturnDogVaccinations(loadPetMedsText.Text);

                if (vacInfo.Count > 0)
                {
                    string rabies = String.Format("{0}", vacInfo.Pop());
                    string canineHep = String.Format("{0}", vacInfo.Pop());
                    string distemper = String.Format("{0}", vacInfo.Pop());
                    string canineParvo = String.Format("{0}", vacInfo.Pop());

                    if (rabies == "True")
                        DogRabiesCheck.Checked = true;

                    if (distemper == "True")
                        DistemperCheck.Checked = true;

                    if (canineHep == "True")
                        CanineHepCheck.Checked = true;

                    if (canineParvo == "True")
                        CanineParvoCheck.Checked = true;

                }
                else
                {
                    MessageBox.Show("Vaccine records unavailable.");
                }
            }
            else
            {
                MessageBox.Show("Only Cats and Dogs have Vaccine Records at this time.");
            }
        }





        //Following methods below are for the data source object and methods as a different way of altering the database. 
        private DatabaseClassLibrary.Pet_Browser_DatabaseEntities dbcontext = null;

        //fill our source with all rows, ordered by pet_ID
        private void RefreshMedications()
        {
            if (dbcontext != null)
                dbcontext.Dispose();

            dbcontext = new DatabaseClassLibrary.Pet_Browser_DatabaseEntities();

            dbcontext.Medications
                .OrderBy(entry => entry.Pet_ID)
                .ThenBy(entry => entry.Medication_ID)
                .Load();

            medicationBindingSource.DataSource = dbcontext.Medications.Local;
            medicationBindingSource.MoveFirst();
            loadPetMedsText.Clear();
        }

        //Loads all medications into view when the application is started.
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshMedications();
        }

        //Enables saving medication alterations and additions.
        private void MedicationBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            Validate();
            medicationBindingSource.EndEdit();

            try
            {
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}", ex.InnerException));
            }

            RefreshMedications(); 
        }

        //Loads meds with specified pet ID
        private void LoadPetMeds()
        {
            var petIDQuery =
                from medications in dbcontext.Medications
                where medications.Pet_ID == loadPetMedsText.Text
                orderby medications.Pet_ID, medications.Medication_ID
                select medications;

            medicationBindingSource.DataSource = petIDQuery.ToList();
            medicationBindingSource.MoveFirst();
        }

    }
}
