#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Info about the ships crew and command centres (bridge/flag bridge/Automatic computer) on a ship.
    /// </summary>
    public class CrewDB : BaseDataBlob
    {
        private double _deploymentTime;
        private int _crewBerths;
        private int _requiredCrew;
        private int _currentCrew;
        private int _spareBerths;
        private int _cryoCrewberths;
        private int _crewInCryo;
        private int _crewGrade;
        private bool _hasBridge;
        private bool _hasFlagBridge;

        public double DeploymentTime { get { return _deploymentTime; } set { SetField(ref _deploymentTime, value); } } // in months
        public int CrewBerths { get { return _crewBerths; } set { SetField(ref _crewBerths, value); } }
        public int RequiredCrew { get { return _requiredCrew; } set { SetField(ref _requiredCrew, value); } }
        public int CurrentCrew { get { return _currentCrew; } set { SetField(ref _currentCrew, value); } }
        public int SpareBerths { get { return _spareBerths; } set { SetField(ref _spareBerths, value); } }
        public int CryoCrewberths { get { return _cryoCrewberths; } set { SetField(ref _cryoCrewberths, value); } }
        public int CrewInCryo { get { return _crewInCryo; } set { SetField(ref _crewInCryo, value); } }

        public int CrewGrade { get { return _crewGrade; } set { SetField(ref _crewGrade, value); } }

        public bool HasBridge { get { return _hasBridge; } set { SetField(ref _hasBridge, value); } }
        public bool HasFlagBridge { get { return _hasFlagBridge; } set { SetField(ref _hasFlagBridge, value); } }

        public CrewDB()
        {
        }

        public CrewDB(CrewDB crewDB)
        {
            DeploymentTime = crewDB.DeploymentTime;
            CrewBerths = crewDB.CrewBerths;
            RequiredCrew = crewDB.RequiredCrew;
            CurrentCrew = crewDB.CurrentCrew;
            SpareBerths = crewDB.SpareBerths;
            CryoCrewberths = crewDB.CryoCrewberths;
            CrewInCryo = crewDB.CrewInCryo;
            CrewGrade = crewDB.CrewGrade;

            //Not sure
            HasBridge = crewDB.HasBridge;
            HasFlagBridge = crewDB.HasFlagBridge;
        }

        public override object Clone()
        {
            return new CrewDB(this);
        }
    }
}