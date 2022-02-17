using System;
using System.Linq;
using System.Collections;
using DatabaseClassLibrary;
using System.Windows.Forms;

namespace IS318_CPM3_Schneiter
{
    class DatabaseSearch : BaseDatabase 
    {
        //Search by Pet Name 
        public string SearchPetNames(string searchQuery)
        {
            string results = "";
            string gendertext;
            string addon;

            var matchingPets =
                from pet in dbcontext.Pets
                join owner in dbcontext.Owners
                on pet.Owner_ID equals owner.Owner_ID
                where pet.Pet_Name == searchQuery
                select new
                {
                    pet.Pet_ID,
                    pet.Pet_Name,
                    pet.Species,
                    pet.Breed,
                    pet.Gender,
                    owner.First_Name,
                    owner.Last_Name
                };

            if (matchingPets.Any())
            {
                foreach (var element in matchingPets)
                {
                    if (element.Gender == true)
                        gendertext = "Female";
                    else
                        gendertext = "Male";

                    addon = String.Format("\r\n\r\n {0, -5} -- {1, -25} \t ({2} {3}, {4}) \t {5, 20} {6} {7}", element.Pet_ID, element.Pet_Name, gendertext, element.Species, element.Breed, "Owner: ", element.First_Name, element.Last_Name);

                    results += addon;
                }
            }
            else
            {
                results = String.Format("No pets found with the name '{0}'.", searchQuery);
            }

            return results;
        }

        //Search by Pet Species
        public string SearchPetSpecies(string searchQuery)
        {
            string results = "";
            string gendertext;
            string addon;

            var matchingPets =
                from pet in dbcontext.Pets
                join owner in dbcontext.Owners
                on pet.Owner_ID equals owner.Owner_ID
                where pet.Species == searchQuery
                select new
                {
                    pet.Pet_ID,
                    pet.Pet_Name,
                    pet.Species,
                    pet.Breed,
                    pet.Gender,
                    owner.First_Name,
                    owner.Last_Name
                };

            if (matchingPets.Any())
            {
                foreach (var element in matchingPets)
                {
                    if (element.Gender == true)
                        gendertext = "Female";
                    else
                        gendertext = "Male";

                    addon = String.Format("\r\n\r\n {0, -5} -- {1, -25} \t ({2} {3}, {4}) \t {5, 20} {6} {7}", element.Pet_ID, element.Pet_Name, gendertext, element.Species, element.Breed, "Owner: ", element.First_Name, element.Last_Name);

                    results += addon;
                }
            }
            else
            {
                results = String.Format("No pets found with the name '{0}'.", searchQuery);
            }


            return results;
        }

        //Search by Owner Name
        public string SearchOwners(string searchQuery)
        {
            string results = "";
            string gendertext;
            string addon;

            var matchingFName =
                from owner in dbcontext.Owners
                join pet in dbcontext.Pets
                on owner.Owner_ID equals pet.Owner_ID
                where owner.First_Name == searchQuery
                select new
                {
                    pet.Pet_ID,
                    pet.Pet_Name,
                    pet.Species,
                    pet.Breed,
                    pet.Gender,
                    owner.First_Name,
                    owner.Last_Name
                };

            if (matchingFName.Any())
            {
                foreach (var element in matchingFName)
                {
                    if (element.Gender == true)
                        gendertext = "Female";
                    else
                        gendertext = "Male";

                    addon = String.Format("\r\n\r\n {0, -5} -- {1, -25} \t ({2} {3}, {4}) \t {5, 20} {6} {7}", element.Pet_ID, element.Pet_Name, gendertext, element.Species, element.Breed, "Owner: ", element.First_Name, element.Last_Name);

                    results += addon;
                }
            }
            else
            {
                results += String.Format("\r\n\r\nNo owner with the first name '{0}' found.", searchQuery);
            }

            var matchingLName =
            from owner in dbcontext.Owners
            join pet in dbcontext.Pets
            on owner.Owner_ID equals pet.Owner_ID
            where owner.Last_Name == searchQuery
            select new
            {
                pet.Pet_ID,
                pet.Pet_Name,
                pet.Species,
                pet.Breed,
                pet.Gender,
                owner.First_Name,
                owner.Last_Name
            };

            if (matchingLName.Any())
            {
                foreach (var element in matchingLName)
                {
                    if (element.Gender == true)
                        gendertext = "Female";
                    else
                        gendertext = "Male";

                    addon = String.Format("\r\n\r\n {0, -5} -- {1, -25} \t ({2} {3}, {4}) \t {5, 20} {6} {7}", element.Pet_ID, element.Pet_Name, gendertext, element.Species, element.Breed, "Owner: ", element.First_Name, element.Last_Name);

                    results += addon;
                }
            }
            else
            {
                results += String.Format("\r\n\r\nNo owner with the last name '{0}' found.", searchQuery);
            }

            return results;
        }

        //Search by Pet ID
        public string SearchPetID(string searchQuery)
        {
            string results = "";
            string gendertext;
            string addon;

            var matchingPets =
                from pet in dbcontext.Pets
                join owner in dbcontext.Owners
                on pet.Owner_ID equals owner.Owner_ID
                where pet.Pet_ID == searchQuery
                select new
                {
                    pet.Pet_ID,
                    pet.Pet_Name,
                    pet.Species,
                    pet.Breed,
                    pet.Gender,
                    owner.First_Name,
                    owner.Last_Name
                };
            
            if (matchingPets.Any())
            {
                foreach (var element in matchingPets)
                {
                    if (element.Gender == true)
                        gendertext = "Female";
                    else
                        gendertext = "Male";

                    addon = String.Format("\r\n\r\n {0, -5} -- {1, -25} \t ({2} {3}, {4}) \t {5, 20} {6} {7}", element.Pet_ID, element.Pet_Name, gendertext, element.Species, element.Breed, "Owner: ", element.First_Name, element.Last_Name);

                    results += addon;
                }
            }
            else
            {
                results = String.Format("No pets found with the ID '{0}'.", searchQuery);
            }


            return results;
        }

        //A method to list all pets in the database.
        public string ListAll()
        {
            string results = String.Format("All Pets and their Owners . . . \r\n");
            string addon = "";
            string gendertext;

            var petsAndOwners =
                from pet in dbcontext.Pets
                join owner in dbcontext.Owners
                on pet.Owner_ID equals owner.Owner_ID
                select new
                {
                    pet.Pet_ID,
                    pet.Pet_Name,
                    pet.Gender,
                    pet.Species,
                    pet.Owner_ID,
                    owner.First_Name,
                    owner.Last_Name
                };

            foreach (var element in petsAndOwners)
            {
                if (element.Gender == true)
                    gendertext = "Female";
                else
                    gendertext = "Male";

                addon = String.Format("\r\n\r\n {0, -5} -- {1, -25} \t ({2} {3}) \t {4, 20} {5} {6}", element.Pet_ID, element.Pet_Name, gendertext, element.Species, "Owner: ", element.First_Name, element.Last_Name);

                results += addon;
            }

            return results;
        }

        //A method to list all owners. 
        public string ListOwners()
        {
            string results = "";
            string addon;

            var ownersList =
                from owner in dbcontext.Owners
                select owner;

            if (ownersList.Any())
            {
                foreach (var element in ownersList)
                {
                    addon = String.Format("\r\n\r\n {0} \t {1} {2} \t {3}", element.Owner_ID, element.First_Name, element.Last_Name, element.Phone_Number);
                    results += addon;
                }
            }

            return results;
        }




        //A method to retrieve the pet info for the pet records page. 
        public Stack GetAllPetInfo(string petID)
        {
            Stack petInfoStack = new Stack();

            //First I check to see if the pet is sterilized or not, we use this last so it goes in first. 
            if (GetPetFixed(petID) == true)
            {
                petInfoStack.Push("*");
            }
            else
                petInfoStack.Push("");

            //Now I can add in the rest of the info. 
            var petInfo =
                from pet in dbcontext.Pets
                where pet.Pet_ID == petID
                select new
                {
                    pet.Pet_Name,
                    pet.Species,
                    pet.Breed,
                    pet.DOB,
                };

            foreach (var element in petInfo)
            {
                petInfoStack.Push(element.Pet_Name);
                petInfoStack.Push(element.Breed);
                petInfoStack.Push(element.Species);
                petInfoStack.Push(element.DOB);
            }

            //Now we have to get the gender and add it to the stack, which is done separately. 
            if (GetPetGender(petID) == true)
                petInfoStack.Push("Female");
            else
                petInfoStack.Push("Male");

            return petInfoStack;
        }

        //Retrieve medications for the pet. 
        public string GetMedications(string petID)
        {
            string results = "";

            var petMedications =
                from medications in dbcontext.Medications
                where medications.Pet_ID == petID
                select medications;

            if (petMedications.Any())
            {
                foreach (var element in petMedications)
                {
                    results += String.Format("{0, 30}: Take {1} for {2}\r\n",
                        element.Medication_Name, element.Dose, element.Usage);
                }
            }
            else
                results = "No medications found for this pet.";

            return results;
        }

        //Retrieve vaccinations for the pet (cats and dogs only)
        public string GetVaccinations(string petID)
        {
            string results;

            if (CheckSpecies(petID) == "Cat")
            {
                results = GetCatVaccines(petID);
            }
            else if (CheckSpecies(petID) == "Dog")
            {
                results = GetDogVaccines(petID);
            }
            else
                results = "There are no required vaccinations for pets of this species.";

            return results;
        }



        //returns false for male and true for female.
        public Boolean GetPetGender(string petID)
        {
            Boolean gender = true;

            var petGender =
                from pet in dbcontext.Pets
                where pet.Pet_ID == petID
                select new
                {
                    pet.Gender
                };

            foreach (var element in petGender)
            {
                if (element.Gender == false)
                    gender = false;
                else
                    gender = true;
            }

            return gender;
        }

        //returns false for unfixed and true for sterile
        public Boolean GetPetFixed(string petID)
        {
            Boolean sterilized = true;

            var petSterile =
                from pet in dbcontext.Pets
                where pet.Pet_ID == petID
                select new
                {
                    pet.Sterilized
                };

            foreach (var element in petSterile)
            {
                if (element.Sterilized == false)
                    sterilized = false;
                else
                    sterilized = true;
            }

            return sterilized;
        }





        //Returns a string representation of the cat Vaccines
        private string GetCatVaccines(string petID)
        {
            string results = "";

            var catVaccines =
                from vaccines in dbcontext.Vaccines_Cat
                where vaccines.Pet_ID == petID
                select vaccines;

            foreach (var element in catVaccines)
            {
                results += String.Format("Feline HerpvesVirus: {0} \r\nFeline Calcivirus: {1} \r\nFeline Panleukopenia: {2} \r\n Rabies: {3} \r\n Feline Leukemia: {4}",
                    element.Feline_Herpesvirus_1, element.Feline_Calicivirus, element.Feline_Panleukopenia, element.Rabies, element.Feline_Leukemia);
            }

            return results;
        }

        private string GetDogVaccines(string petID)
        {
            string results = "";

            var dogVaccines =
                from vaccines in dbcontext.Vaccines_Dog
                where vaccines.Pet_ID == petID
                select vaccines;

            foreach (var element in dogVaccines)
            {
                results += String.Format("Canine Parvovirus: {0} \r\nDistemper: {1} \r\nCanine Hepatitus: {2} \r\n Rabies: {3}",
                    element.Canine_Parvovirus, element.Distemper, element.Canine_Hepatitis, element.Rabies);
            }


            return results;
        }



        //Loading the Owner Info for the alter page.
        public Stack GetOwnerInfo(string ownerID)
        {
            Stack ownerInfoStack = new Stack(); 

            try
            {
                var ownerInfo =
                        from owner in dbcontext.Owners
                        where owner.Owner_ID == ownerID
                        select owner;

                foreach (var owner in ownerInfo)
                {
                    ownerInfoStack.Push(owner.Phone_Number);
                    ownerInfoStack.Push(owner.Last_Name);
                    ownerInfoStack.Push(owner.First_Name);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error retreiving this owner from the database.");
            }

            return ownerInfoStack;
        }

        //Loading the Pet Info for the alter Page.
        public Stack GetPetInfo(string petID)
        {
            Stack petInfoStack = new Stack();

            try
            {
                var petInfo =
                        from pet in dbcontext.Pets
                        where pet.Pet_ID == petID
                        select pet;

                foreach (var pet in petInfo)
                {
                    petInfoStack.Push(pet.DOB);
                    petInfoStack.Push(pet.Breed);
                    petInfoStack.Push(pet.Species);
                    petInfoStack.Push(pet.Owner_ID);
                    petInfoStack.Push(pet.Pet_Name);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading your pet from the database.");
            }

            return petInfoStack;
        }






        //Loading vaccination info for cat. 
        public Stack ReturnCatVaccinations(string petID)
        {
            Stack petVacs = new Stack();

            var catVaccines =
                from vaccines in dbcontext.Vaccines_Cat
                where vaccines.Pet_ID == petID
                select vaccines;

            if (catVaccines.Any())
            {
                foreach (var element in catVaccines)
                {
                    petVacs.Push(element.Feline_Herpesvirus_1);
                    petVacs.Push(element.Feline_Calicivirus);
                    petVacs.Push(element.Feline_Panleukopenia);
                    petVacs.Push(element.Rabies);
                    petVacs.Push(element.Feline_Leukemia);
                }
            }
            else
            {
                DatabaseAlter createCatVac = new DatabaseAlter();
                petVacs = createCatVac.CreateCatVaccines(petID);
            }

            return petVacs;
        }

        //Loading vaccination info for dog.
        public Stack ReturnDogVaccinations(string petID)
        {
            Stack petVacs = new Stack();

            var dogVaccines =
                from vaccines in dbcontext.Vaccines_Dog
                where vaccines.Pet_ID == petID
                select vaccines;

            if (dogVaccines.Any())
            {
                foreach (var element in dogVaccines)
                {
                    petVacs.Push(element.Canine_Parvovirus);
                    petVacs.Push(element.Distemper);
                    petVacs.Push(element.Canine_Hepatitis);
                    petVacs.Push(element.Rabies);
                }
            }
            else
            {
                DatabaseAlter createDogVac = new DatabaseAlter();
                petVacs = createDogVac.CreateDogVaccines(petID);
            }

            return petVacs;
        }




    }
}
