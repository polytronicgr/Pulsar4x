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
    /// stores the Design entity for specific instances of a ship, component, or other designed entity.
    /// </summary>
    public class DesignInfoDB : BaseDataBlob
    {
        #region Fields
        private Entity _designEntity;
        #endregion

        #region Properties
        public Entity DesignEntity { get { return _designEntity; } set { SetField(ref _designEntity, value); } }
        #endregion

        #region Constructors
        public DesignInfoDB() { }

        public DesignInfoDB(Entity designEntity) { DesignEntity = designEntity; }

        public DesignInfoDB(DesignInfoDB db) { DesignEntity = db.DesignEntity; }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new DesignInfoDB(this);
        #endregion
    }
}