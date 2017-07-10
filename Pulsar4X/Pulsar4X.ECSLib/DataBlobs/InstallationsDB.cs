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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// this is used to turn installations on and off, 
    /// and also used by the Processor to check pop requirements.
    /// </summary>
    public struct InstallationEmployment
    {
        public Guid Type;
        public bool Enabled;
    }

    public class InstallationsDB : BaseDataBlob
    {
        /// <summary>
        /// a dictionary of installationtype, and the number of that specific type including partial installations.
        /// </summary>
        public ObservableDictionary<Guid, float> Installations { get; set; } = new ObservableDictionary<Guid, float>();

        public ObservableDictionary<Guid,int> WorkingInstallations { get; set; } = new ObservableDictionary<Guid, int>();

        public ObservableCollection<InstallationEmployment> EmploymentList { get; set; } = new ObservableCollection<InstallationEmployment>();
        /// <summary>
        /// list of ConstructionJob Structs.
        /// </summary>
        public InstallationsDB()
        {
            Installations.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(Installations), args);
            WorkingInstallations.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(WorkingInstallations), args);
            EmploymentList.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(EmploymentList), args);

        }

        public InstallationsDB(InstallationsDB db) : this()
        {
            Installations.Merge(db.Installations);
            WorkingInstallations.Merge(db.WorkingInstallations);
            foreach (InstallationEmployment installationEmployment in db.EmploymentList)
            {
                EmploymentList.Add(installationEmployment);
            }
        }

        public override object Clone()
        {
            return new InstallationsDB(this);
        }
    }
}