using System;
using System.Linq;
using DatabaseClassLibrary;
using System.Windows.Forms;
using System.Collections;

namespace IS318_CPM3_Schneiter
{
    class DatabaseAlter : BaseDatabase
    {

        //A method for removing a pet by the specified ID. Also removes all medication and vaccine entries for said pet. 
        //Should also remove any relevant files.
        public void RemovePet(string deleteQuery)
        {
            string confirmMessage = "";

            //First we get the info from the pet
            var confirmPet =
                from pet in dbcontext.Pets
                join owner in dbcontext.Owners
                on pet.Owner_ID equals owner.Owner_ID
                where pet.Pet_ID == deleteQuery
                select new
                {
                    pet.Pet_Name,
                    owner.First_Name,
                    owner.Last_Name
                };

            foreach (var element in confirmPet)
                confirmMessage = String.Format("Are you sure you want to remove {0} {1}'s pet, {2} from the database?", element.First_Name, element.Last_Name, element.Pet_Name);

            var confirmResult = MessageBox.Show(confirmMessage, "Confirm Delete!", MessageBoxButtons.YesNo);

            //Once we confirm we want to delete this pet, we move on.
            if (confirmResult == DialogResult.Yes)
            {
                //First we remove medications. 
                var deleteMeds =
                from medication in dbcontext.Medications
                where medication.Pet_ID == deleteQuery
                select medication;

                if (deleteMeds.Any())
                {
                    foreach (var medication in deleteMeds)
                    {
                        dbcontext.Medications.Remove(medication);
                    }
                }

                //Then we remove vaccinations. 
                var deleteIfCat =
                from vaccines in dbcontext.Vaccines_Cat
                where vaccines.Pet_ID == deleteQuery
                select vaccines;

                if (deleteIfCat.Any())
                {
                    foreach (var vaccines in deleteIfCat)
                    {
                        dbcontext.Vaccines_Cat.Remove(vaccines);
                    }
                }

                var deleteIfDog =
                from vaccines in dbcontext.Vaccines_Dog
                where vaccines.Pet_ID == deleteQuery
                select vaccines;

                if (deleteIfDog.Any())
                {
                    foreach (var vaccines in deleteIfDog)
                    {
                        dbcontext.Vaccines_Dog.Remove(vaccines);
                    }
                }

                //Next, we remove all text files associated with this pet. 
                TextFileClass petFiles = new TextFileClass(deleteQuery);
                petFiles.RemoveFile();

                //Then we remove the pet. 
                var deletePet =
                from pet in dbcontext.Pets
                where pet.Pet_ID == deleteQuery
                select pet;

                if (deletePet.Any())
                {
                    foreach (var pet in deletePet)
                    {
                        dbcontext.Pets.Remove(pet);
                    }
                }

                try
                {
                    dbcontext.SaveChanges();
                    MessageBox.Show("Pet successfully removed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //A method for removing an owner by the Specified ID. Will fail if any pets remain under that owner.
        public void RemoveOwner(string deleteQuery)
        {
            string confirmMessage = "";

            if (CheckForPets(deleteQuery) == false)
            {
                var confirmDeleteOwner =
                    from owner in dbcontext.Owners
                    where owner.Owner_ID == deleteQuery
                    select owner;

                //We use the owner's name to confirm this is the owner file we want to remove.
                foreach (var owner in confirmDeleteOwner)
                {
                    confirmMessage = String.Format("Are you sure you want to remove {0} {1} from the database?", owner.First_Name, owner.Last_Name);
                }

                var confirmResult = MessageBox.Show(confirmMessage, "Confirm Delete!!", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    foreach (var owner in confirmDeleteOwner)
                        dbcontext.Owners.Remove(owner);
                }

                try
                {
                    dbcontext.SaveChanges();
                    MessageBox.Show("Owner successfully removed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("There are still pets under this owner, please alter the pets.");

        }

        //A method for adding a new owner to the database
        public void AddOwner(string fName, string lName, string phoneNumber)
        {
            Boolean validEntries = ValidateOwnerText(fName, lName, phoneNumber);

            if (validEntries)
            {
                string newID = NewOwnerID();

                Owner newOwner = new Owner
                {
                    Owner_ID = newID,
                    First_Name = fName,
                    Last_Name = lName,
                    Phone_Number = phoneNumber
                };

                dbcontext.Owners.Add(newOwner);

                try
                {
                    dbcontext.SaveChanges();
                    MessageBox.Show(String.Format("{0} {1} successfully added to the database!", fName, lName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //A method for adding a new pet to the database. 
        public void AddPet(string name, string ownerID, string species, string breed, string yearOfBirth, Boolean gender, Boolean sterilized)
        {
            //If all of these return true, insert pet.
            if (OwnerExists(ownerID) && ValidateDOB(yearOfBirth) && ValidatePetText(name, species))
            {
                string newID = NewPetID();

                Pet newPet = new Pet
                {
                    Pet_ID = newID,
                    Pet_Name = name,
                    Owner_ID = ownerID,
                    Species = species,
                    Breed = breed,
                    DOB = yearOfBirth,
                    Gender = gender,
                    Sterilized = sterilized
                };

                dbcontext.Pets.Add(newPet);

                try
                {
                    dbcontext.SaveChanges();
                    MessageBox.Show(String.Format("{0} the {1} successfully added to the database!", name, species));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Add Pet failed, please make sure all entries are valid.");
            }

        }

        //A method for updating an owner's info. 
        public void UpdateOwner(Stack ownerStack)
        {
            string phoneNumber = String.Format("{0}", ownerStack.Pop());
            string ownerLName = String.Format("{0}", ownerStack.Pop());
            string ownerFName = String.Format("{0}", ownerStack.Pop());
            string ownerID = String.Format("{0}", ownerStack.Pop());

            var ownerUpdate =
                from owner in dbcontext.Owners
                where owner.Owner_ID == ownerID
                select owner;

            foreach (Owner owner in ownerUpdate)
            {
                owner.First_Name = ownerFName;
                owner.Last_Name = ownerLName;
                owner.Phone_Number = phoneNumber;
            }

            try
            {
                dbcontext.SaveChanges();
                MessageBox.Show("Database Successfully Updated!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //A method for updating a Pet's info
        public void UpdatePet(Stack petStack, Boolean gender, Boolean sterile)
        {
            string dob = String.Format("{0}", petStack.Pop());
            string breed = String.Format("{0}", petStack.Pop());
            string species = String.Format("{0}", petStack.Pop());
            string ownerID = String.Format("{0}", petStack.Pop());
            string name = String.Format("{0}", petStack.Pop());
            string petID = String.Format("{0}", petStack.Pop());
            Boolean petGender = gender;
            Boolean petSterile = sterile;

            var petUpdate =
                from pet in dbcontext.Pets
                where pet.Pet_ID == petID
                select pet;

            if (OwnerExists(ownerID))
            {
                foreach (Pet pet in petUpdate)
                {
                    pet.Pet_Name = name;
                    pet.Owner_ID = ownerID;
                    pet.Species = species;
                    pet.Breed = breed;
                    pet.DOB = dob;
                    pet.Gender = petGender;
                    pet.Sterilized = petSterile;
                }

                try
                {
                    dbcontext.SaveChanges();
                    MessageBox.Show("Database Successfully Updated!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("Invalid Owner, changes discarded.");
        }




        //Additional Methods for addOwner
        //counts and checks to see if any ID spots are free. This way, instead of simply counting the total number of owners going off that, it goes for the first free slot and ensures the new ID does not match one already in the database.
        private string NewOwnerID()
        {
            string stringID = "0001";
            int intID = 1;

            var ownerIDs =
                from owner in dbcontext.Owners
                select owner; 

            try
            {
                foreach (var owner in ownerIDs)
                {
                    if (owner.Owner_ID == stringID)
                    {
                        intID += 1;

                        if (intID >= 0 && intID <= 9)
                        {
                            stringID = String.Format("000{0}", intID.ToString());
                        }
                        else if (intID >= 10 && intID <= 99)
                        {
                            stringID = String.Format("00{0}", intID.ToString());
                        }
                        else if (intID >= 100 && intID <= 999)
                        {
                            stringID = String.Format("0{0}", intID.ToString());
                        }
                        else
                        {
                            stringID = String.Format("{0}", intID.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return stringID;
        }

        private Boolean ValidateOwnerText(string fName, string lName, string phoneNumber)
        {
            if (fName != "" && lName != "" && phoneNumber != "")
                return true;
            else
                return false;
        }




        //Additional methods for addPet. 

        private Boolean ValidateDOB(string DOB)
        {
            if (DOB.Length == 4)
            {
                try
                {
                    int dob = int.Parse(DOB);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
                return false;
        }

        private Boolean ValidatePetText(string name, string species)
        {
            if (name != "" && species != "")
                return true;
            else
                return false;
        }

        private string NewPetID()
        {
            string stringID = "0001";
            int intID = 1;

            var petIDs =
                from pet in dbcontext.Pets
                select pet;

            try
            {
                foreach (var pet in petIDs)
                {
                    if (pet.Pet_ID == stringID)
                    {
                        intID += 1;

                        if (intID >= 0 && intID <= 9)
                        {
                            stringID = String.Format("000{0}", intID.ToString());
                        }
                        else if (intID >= 10 && intID <= 99)
                        {
                            stringID = String.Format("00{0}", intID.ToString());
                        }
                        else if (intID >= 100 && intID <= 999)
                        {
                            stringID = String.Format("0{0}", intID.ToString());
                        }
                        else
                        {
                            stringID = String.Format("{0}", intID.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return stringID;
        }



        //Additional methods for remove Owner.
        private Boolean CheckForPets(string ownerID)
        {
            var checkPets =
                    from pet in dbcontext.Pets
                    where pet.Owner_ID == ownerID
                    select pet;

            if (checkPets.Any())
                return true;
            else
                return false; 

        }




        //Add cat vac record, set to false automatically.
        public Stack CreateCatVaccines(string petID)
        {
            Stack newVacs = new Stack();

            newVacs.Push(false);
            newVacs.Push(false);
            newVacs.Push(false);
            newVacs.Push(false);
            newVacs.Push(false);

            Vaccines_Cat vacRecord = new Vaccines_Cat
            {
                Pet_ID = petID,
                Feline_Herpesvirus_1 = false,
                Feline_Calicivirus = false,
                Feline_Panleukopenia = false,
                Rabies = false,
                Feline_Leukemia = false
            };
            dbcontext.Vaccines_Cat.Add(vacRecord);

            try
            {
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return newVacs;
        }

        //Add dog vac record, set to false automatically. 
        public Stack CreateDogVaccines(string petID)
        {
            Stack newVacs = new Stack();

            newVacs.Push(false);
            newVacs.Push(false);
            newVacs.Push(false);
            newVacs.Push(false);

            Vaccines_Dog vacRecord = new Vaccines_Dog
            {
                Pet_ID = petID,
                Canine_Hepatitis = false,
                Canine_Parvovirus = false,
                Rabies = false,
                Distemper = false
            };
            dbcontext.Vaccines_Dog.Add(vacRecord);

            try
            {
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return newVacs;
        }


        //Update Cat Vaccines
        public void UpdateCatVacs(string petID, bool fleuk, bool rabies, bool panleuk, bool fcalci, bool fherpes)
        {
            var vacUpdate =
                from vaccine in dbcontext.Vaccines_Cat
                where vaccine.Pet_ID == petID
                select vaccine;

            foreach (Vaccines_Cat vaccine in vacUpdate)
            {
                vaccine.Feline_Leukemia = fleuk;
                vaccine.Rabies = rabies;
                vaccine.Feline_Panleukopenia = panleuk;
                vaccine.Feline_Calicivirus = fcalci;
                vaccine.Feline_Herpesvirus_1 = fherpes; 
            }

            try
            {
                dbcontext.SaveChanges();
                MessageBox.Show("Database Successfully Updated!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Update Dog Vaccines
        public void UpdateDogVacs(string petID, bool parvo, bool distemper, bool hepa, bool rabies)
        {
            var vacUpdate = 
                from vaccine in dbcontext.Vaccines_Dog
                where vaccine.Pet_ID == petID
                select vaccine;

            foreach (Vaccines_Dog vaccine in vacUpdate)
            {
                vaccine.Canine_Parvovirus = parvo;
                vaccine.Distemper = distemper;
                vaccine.Canine_Hepatitis = hepa;
                vaccine.Rabies = rabies;
            }

            try
            {
                dbcontext.SaveChanges();
                MessageBox.Show("Database Successfully Updated!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
