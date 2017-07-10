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
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class PositionDB : TreeHierarchyDB
    {
        #region Fields
        [JsonProperty]
        private Vector4 _position;

        [JsonProperty]
        private Guid _systemGuid;
        #endregion

        #region Properties
        public override Entity Parent
        {
            get { return base.Parent; }
            set
            {
                if (value != null && !value.HasDataBlob<PositionDB>())
                {
                    throw new Exception("newParent must have a PositionDB");
                }
                Vector4 currentAbsolute = AbsolutePosition;
                Vector4 newRelative;
                if (value == null)
                {
                    newRelative = currentAbsolute;
                }
                else
                {
                    newRelative = currentAbsolute - value.GetDataBlob<PositionDB>().AbsolutePosition;
                }
                base.Parent = value;
                _position = newRelative;
            }
        }

        /// <summary>
        /// The Position as a Vec4, in AU.
        /// </summary>
        public Vector4 AbsolutePosition
        {
            get
            {
                if (Parent == null)
                {
                    return _position;
                }
                if (Parent == OwningEntity)
                {
                    throw new Exception("Infinite loop triggered");
                }

                var parentpos = (PositionDB)ParentDB;
                if (parentpos == this)
                {
                    throw new Exception("Infinite loop triggered");
                }

                return parentpos.AbsolutePosition + _position;
            }
            set
            {
                if (Parent == null)
                {
                    SetField(ref _position, value);
                }
                else
                {
                    var parentpos = (PositionDB)ParentDB;
                    SetField(ref _position, value - parentpos.AbsolutePosition);
                }
            }
        }

        /// <summary>
        /// Get or Set the position relative to the parent Entity's abolutePositon
        /// </summary>
        public Vector4 RelativePosition { get { return _position; } set { SetField(ref _position, value); } }


        /// <summary>
        /// System X coordinate in AU
        /// </summary>
        public double X { get { return AbsolutePosition.X; } set { SetField(ref _position.X, value); } }

        /// <summary>
        /// System Y coordinate in AU
        /// </summary>
        public double Y { get { return AbsolutePosition.Y; } set { SetField(ref _position.Y, value); } }

        /// <summary>
        /// System Z coordinate in AU
        /// </summary>
        public double Z { get { return AbsolutePosition.Z; } set { SetField(ref _position.Z, value); } }

        /// <summary>
        /// Position as a vec4. This is a utility property that converts Position to Km on get and to AU on set.
        /// </summary>
        public Vector4 PositionInKm { get { return new Vector4(Distance.AuToKm(AbsolutePosition.X), Distance.AuToKm(AbsolutePosition.Y), Distance.AuToKm(AbsolutePosition.Z), 0); } set { AbsolutePosition = new Vector4(Distance.KmToAU(value.X), Distance.KmToAU(value.Y), Distance.KmToAU(value.Z), 0); } }

        /// <summary>
        /// System X coordinante. This is a utility property that converts the X Coord. to Km on get and to AU on set.
        /// </summary>
        public double XInKm { get { return Distance.AuToKm(AbsolutePosition.X); } set { _position.X = Distance.KmToAU(value); } }

        /// <summary>
        /// System Y coordinante. This is a utility property that converts the Y Coord. to Km on get and to AU on set.
        /// </summary>
        public double YInKm { get { return Distance.AuToKm(AbsolutePosition.Y); } set { _position.Y = Distance.KmToAU(value); } }

        /// <summary>
        /// System Z coordinate. This is a utility property that converts the Z Coord. to Km on get and to AU on set.
        /// </summary>
        public double ZInKm { get { return Distance.AuToKm(AbsolutePosition.Z); } set { _position.Z = Distance.KmToAU(value); } }

        public Guid SystemGuid
        {
            get { return _systemGuid; }
            set
            {
                SetField(ref _systemGuid, value);
                ;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initialized
        /// .
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        /// <param name="z">Z value.</param>
        public PositionDB(double x, double y, double z, Guid systemGuid, Entity parent = null) : base(parent)
        {
            AbsolutePosition = new Vector4(x, y, z, 0);
            SystemGuid = systemGuid;
        }

        public PositionDB(Vector4 pos, Guid systemGuid, Entity parent = null) : base(parent)
        {
            AbsolutePosition = pos;
            SystemGuid = systemGuid;
        }

        public PositionDB(Guid systemGuid, Entity parent = null) : base(parent)
        {
            Vector4? parentPos = (ParentDB as PositionDB)?.AbsolutePosition;
            AbsolutePosition = parentPos ?? Vector4.Zero;
            SystemGuid = systemGuid;
        }

        public PositionDB(PositionDB positionDB) : base(positionDB.Parent)
        {
            X = positionDB.X;
            Y = positionDB.Y;
            Z = positionDB.Z;
            SystemGuid = positionDB.SystemGuid;
        }

        [UsedImplicitly]
        private PositionDB() : this(Guid.Empty) { }
        #endregion

        #region Interfaces, Overrides, and Operators
        public override object Clone() => new PositionDB(this);

        public static PositionDB operator +(PositionDB posA, PositionDB posB)
        {
            throw new NotSupportedException("Do not add two PositionDBs. See comments in PositonDB.cs");

            /* Operator not supported as it can lead to unintended consequences,
             * especially when trying to do "posA += posB;"
             * Instead of posA += posB, do "posA.Position += posB.Position;"
             * 
             * Datablobs are stored in an entity manager, and contain important metadata.
             * posA += posB evaluates to posA = posA + posB;
             * This operator has to return a "new" datablob. This new datablob is not the
             * one current stored in the EntityManager. Further requests to get the positionDB
             * will return the old positionDB after a += operation.
             * 
             * Ask a senior developer for further clarification if required.
             * 
             * Explicitly thrown to prevent new developers from adding this.
            */
        }
        #endregion

        #region Public Methods
        public void AddMeters(Vector4 addVector) { _position += Distance.MToAU(addVector); }

        /// <summary>
        /// Static function to find the distance between two positions.
        /// </summary>
        /// <returns>AU Distance between posA and posB.</returns>
        public static double GetDistanceBetween(PositionDB posA, PositionDB posB) => (posA.AbsolutePosition - posB.AbsolutePosition).Length();

        /// <summary>
        /// Instance function for those who don't like static functions.
        /// </summary>
        /// <returns>AU distance to otherPos</returns>
        public double GetDistanceTo(PositionDB otherPos) => GetDistanceBetween(this, otherPos);

        /// <summary>
        /// Static Function to find the Distance Squared betweeen two positions.
        /// </summary>
        public static double GetDistanceBetweenSqrd(PositionDB posA, PositionDB posB) => (posA.AbsolutePosition - posB.AbsolutePosition).LengthSquared();

        /// <summary>
        /// Instance function for those who don't like static functions.
        /// </summary>
        public double GetDistanceToSqrd(PositionDB otherPos) => GetDistanceBetweenSqrd(this, otherPos);
        #endregion
    }
}