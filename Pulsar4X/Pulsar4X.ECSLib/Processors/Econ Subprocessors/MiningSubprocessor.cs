using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib.IndustryProcessors
{
    class MiningSubprocessor
    {
        private readonly Game _game;

        internal MiningSubprocessor(Game game)
        {
            _game = game;
        }

        internal void ProcessMining(IndustrialEntity industrialEntity)
        {
            Entity mineableEntity = industrialEntity.MatedToDB?.Parent;
            var parentSystemBodyDB = mineableEntity?.GetDataBlob<SystemBodyDB>();

            if (parentSystemBodyDB == null)
            {
                // Entity not mated to an entity that can be mined.
                return;
            }
            
            double remainingCapacity = CargoHelper.GetFreeCargoSpace(industrialEntity.CargoDB, CargoType.General);

            foreach (KeyValuePair<Guid, MineralDepositInfo> mineralDepositInfo in parentSystemBodyDB.Minerals)
            {
                MineralDepositInfo depositInfo = mineralDepositInfo.Value;

                float annualProduction = depositInfo.Accessibility * industrialEntity.IndustryDB.industryRates[IndustryType.MiningRate];
                float itemMultiplier = IndustrySubprocessor.GetIndustrialMultiplier(_game, mineralDepositInfo.Key, industrialEntity.IndustryDB);

                double tickProduction = annualProduction * (_game.Settings.EconomyCycleTime.TotalDays / 365) * itemMultiplier;

                if (tickProduction <= 0)
                {
                    return;
                }

                if (tickProduction > remainingCapacity)
                {
                    // Fill up cargo, flag event, break to avoid adding more.
                    MineResource(mineralDepositInfo.Key, depositInfo, industrialEntity.CargoDB, remainingCapacity);
                    var cargoFullEvent = new Event(_game.CurrentDateTime, "Mining failed. Cargo full", EventType.CargoFull, null, industrialEntity.Entity);
                    _game.EventLog.AddEvent(cargoFullEvent);
                    break;
                }

                MineResource(mineralDepositInfo.Key, depositInfo, industrialEntity.CargoDB, tickProduction);
            }
        }

        private void MineResource(Guid mineralGuid, MineralDepositInfo depositInfo, CargoDB entityCargoDB, double amount)
        {
            CargoDefinition mineralCargoDef = CargoHelper.GetCargoDefinition(_game, mineralGuid);

            // Update the minging entity cargo.
            entityCargoDB.cargoCarried.SafeValueAdd(mineralCargoDef, amount);

            // Update the deposit info.
            depositInfo.Amount -= (float)amount;
            double accessibility = (float)Math.Pow(depositInfo.Amount / depositInfo.HalfOriginalAmount, 3) * depositInfo.Accessibility;
            depositInfo.Accessibility = (float)GMath.Clamp(accessibility, 0.1, depositInfo.Accessibility);
        }
    }
}
