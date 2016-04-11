using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public static class ColonyFactory
    {
        /// <summary>
        /// Creates a new colony with zero population.
        /// TODO: Review access control on this function.
        /// </summary>
        [NotNull]
        public static Entity CreateColony([NotNull] Entity factionEntity, [NotNull] Entity speciesEntity, [NotNull] Entity planetEntity)
        {
            if (factionEntity == null || !factionEntity.IsValid)
            {
                throw new ArgumentNullException(nameof(factionEntity));
            }
            if (speciesEntity == null || !speciesEntity.IsValid)
            {
                throw new ArgumentNullException(nameof(speciesEntity));
            }
            if (planetEntity == null || !planetEntity.IsValid)
            {
                throw new ArgumentNullException(nameof(planetEntity));
            }

            var planetNameDB = planetEntity.GetDataBlob<NameDB>();

            if (planetNameDB == null)
            {
                throw new ArgumentException("Provided planet is malformed: Has no NameDB.", nameof(planetEntity));
            }

            var factionInfoDB = factionEntity.GetDataBlob<FactionDB>();

            if (factionInfoDB == null)
            {
                throw new ArgumentException("Provided faction is malformed: Has no FactionInfoDB.", nameof(factionEntity));
            }

            string planetName = planetNameDB.GetName(factionEntity);
            var ownedDB = new OwnedDB(factionEntity);
            var name = new NameDB(planetName + " Colony"); // TODO: Review default name.
            var colonyInfoDB = new ColonyDB(speciesEntity, 0);
            var entityBonuses = new BonusesDB();
            var colonyIndustryDB = new IndustryDB();
            var componentInstanceDB = new ComponentInstancesDB();
            var colonyCargo = new CargoDB(true);
            var matedBlob = new MatedToDB(planetEntity);
            var blobs = new List<BaseDataBlob>
            {
                ownedDB,
                name,
                colonyInfoDB,
                entityBonuses,
                colonyIndustryDB,
                componentInstanceDB,
                colonyCargo,
                matedBlob
            };

            var colonyEntity = new Entity(planetEntity.Manager, blobs);

            factionInfoDB.Colonies.Add(colonyEntity);
            return colonyEntity;
        }
    }
}