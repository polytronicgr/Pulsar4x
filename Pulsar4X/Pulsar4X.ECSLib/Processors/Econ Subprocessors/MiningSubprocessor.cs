using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Pulsar4X.ECSLib
{
    class MiningSubprocessor
    {
        private readonly Game _game;
        internal DateTime NextHaltTime { get; private set; } = DateTime.MaxValue;

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
            double totalProduction = 0;

            foreach (KeyValuePair<Guid, MineralDepositInfo> mineralDepositInfo in parentSystemBodyDB.Minerals)
            {
                MineralDepositInfo depositInfo = mineralDepositInfo.Value;

                float annualProduction = depositInfo.Accessibility * industrialEntity.IndustryDB.industryRates[IndustryType.Mining];
                float itemMultiplier = IndustrySubprocessor.GetIndustrialMultiplier(_game, mineralDepositInfo.Key, industrialEntity.IndustryDB);

                double tickProduction = annualProduction * (_game.Settings.EconomyCycleTime.TotalDays / 365) * itemMultiplier;

                if (tickProduction <= 0)
                {
                    continue;
                }

                if (tickProduction > remainingCapacity)
                {
                    // Fill up cargo, flag event, break to avoid adding more.
                    totalProduction += remainingCapacity;
                    remainingCapacity = 0;
                    MineResource(mineralDepositInfo.Key, depositInfo, industrialEntity.CargoDB, remainingCapacity);
                    var cargoFullEvent = new Event(_game.CurrentDateTime, "Mining failed. Cargo full", EventType.CargoFull, null, industrialEntity.Entity);
                    _game.EventLog.AddEvent(cargoFullEvent);
                    break;
                }

                totalProduction += tickProduction;
                remainingCapacity -= tickProduction;
                MineResource(mineralDepositInfo.Key, depositInfo, industrialEntity.CargoDB, tickProduction);

            }

            if (totalProduction == 0 || remainingCapacity == double.MaxValue || remainingCapacity == 0)
            {
                return;
            }

            // Mining production complete. Estimate date this entity will fill its cargo.
            double ticksToFill = 1 - 1 / totalProduction / remainingCapacity;
            TimeSpan timeUntilFilled = TimeSpan.FromHours(_game.Settings.EconomyCycleTime.TotalHours * ticksToFill);

            DateTime projectedFillDate = _game.CurrentDateTime + timeUntilFilled;

            if (projectedFillDate < NextHaltTime)
            {
                NextHaltTime = projectedFillDate;
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
