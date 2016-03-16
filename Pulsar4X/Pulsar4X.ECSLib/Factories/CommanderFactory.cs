using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public static class CommanderFactory
    {
        public static Entity CreateScientist(EntityManager entityManager, Entity faction)
        {
            //all this stuff needs a proper bit of code to get names from a file or something.
            // TODO: Implement name selection
            CommanderNameSD name;
            name.First = "Augusta";
            name.Last = "King";
            name.IsFemale = true;
            
            // TODO: Randomize starting rank for scientist.
            // Rank determines number of labs.
            int rank = 0;
            // TODO: Determine where this leader starts at.
            Entity parent = Entity.InvalidEntity;

            var newLeaderDB = new LeaderDB(name.IsFemale, rank, CommanderType.Scientist);
            var newMatedDB = new MatedToDB(parent);
            var newNameDB = new NameDB($"{name.First} {name.Last}");
            var BonusesDB = new BonusesDB();

            var blobs = new List<BaseDataBlob>
            {
                newLeaderDB,
                newMatedDB,
                newNameDB,
                BonusesDB,
            };

            return new Entity(entityManager, blobs); ;    
        }
    }
}
