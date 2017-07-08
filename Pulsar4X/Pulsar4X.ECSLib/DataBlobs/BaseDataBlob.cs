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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pulsar4X.ECSLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseDataBlob : ICloneable, INotifyPropertyChanged
    {
        [NotNull]
        public virtual Entity OwningEntity { get { return _owningEntity; } internal set { SetField(ref _owningEntity, value); } }
        private Entity _owningEntity = Entity.InvalidEntity;

        public abstract object Clone();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the field's value while firing PropertyChanged events.
        /// </summary>
        /// <remarks>        
        /// This function reduces (but not eliminates) the amount of boilerplate code for INotifyPropertyChanged in every other datablob.
        /// </remarks>
        /// <typeparam name="T">Type of field value</typeparam>
        /// <param name="backingField">Storage location to assign value to.</param>
        /// <param name="value">New value to be assigned.</param>
        /// <param name="propertyName">Automatically resolved propery name.</param>
        [NotifyPropertyChangedInvocator]
        public void SetField<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            // Only attempt to set the field if value is different.
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }

            // Set the new value.
            backingField = value;

            // Fire the PropertyChanged event.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
