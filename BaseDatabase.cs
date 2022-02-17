using System;
using System.Linq;
using DatabaseClassLibrary;

namespace IS318_CPM3_Schneiter
{
    abstract class BaseDatabase
    {
        protected readonly DatabaseClassLibrary.Pet_Browser_DatabaseEntities dbcontext = new DatabaseClassLibrary.Pet_Browser_DatabaseEntities();

        //Verify pet ID exists 
        public Boolean PetExists(string petID)
        {
            var petExists =
                from pet in dbcontext.Pets
                where pet.Pet_ID == petID
                select pet;

            if (petExists.Any())
                return true;
            else
                return false;
        }

        //Verify owner ID exists
        public Boolean OwnerExists(string ownerID)
        {
            var ownerExists =
                from owner in dbcontext.Owners
                where owner.Owner_ID == ownerID
                select owner;

            if (ownerExists.Any())
                return true;
            else
                return false;
        }

        //Returns Species
        public string CheckSpecies(string petID)
        {
            string species = "";

            var animalSpeciesCheck =
                from pet in dbcontext.Pets
                where pet.Pet_ID == petID
                select new
                {
                    pet.Species
                };

            foreach (var element in animalSpeciesCheck)
            {
                species = element.Species;
            }

            return species;
        }

    }
}
