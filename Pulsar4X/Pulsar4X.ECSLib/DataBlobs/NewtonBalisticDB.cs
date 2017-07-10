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

namespace Pulsar4X.ECSLib
{
    public class NewtonBalisticDB : BaseDataBlob
    {
        #region Fields
        private DateTime _collisionDate;
        private Vector4 _currentSpeed;
        private Guid _targetGuid;
        #endregion

        #region Properties
        public Vector4 CurrentSpeed { get { return _currentSpeed; } set { SetField(ref _currentSpeed, value); } }
        public Guid TargetGuid { get { return _targetGuid; } set { SetField(ref _targetGuid, value); } }
        public DateTime CollisionDate { get { return _collisionDate; } set { SetField(ref _collisionDate, value); } }
        #endregion

        #region Constructors
        /// <summary>
        /// necessary for serializer
        /// </summary>
        public NewtonBalisticDB() { }

        public NewtonBalisticDB(Guid TgtGuid, DateTime cDate)
        {
            TargetGuid = TgtGuid;
            CollisionDate = cDate;
        }


        public NewtonBalisticDB(NewtonBalisticDB db)
        {
            CurrentSpeed = db.CurrentSpeed;
            TargetGuid = db.TargetGuid;
            CollisionDate = db.CollisionDate;
        }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new NewtonBalisticDB(this);
        #endregion
    }
}