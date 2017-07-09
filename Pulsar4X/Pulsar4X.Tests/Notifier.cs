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
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Pulsar4X.ECSLib;

namespace Pulsar4X.Tests
{
    /// <summary>
    /// Mock datablob to test the syncing of both collection and simple data through INotifyPropertyChanged and INotifySubCollectionChanged
    /// </summary>
    public class SyncMockDB : BaseDataBlob
    {
        public override object Clone() => throw new NotImplementedException();

        private ObservableCollection<int> _numberList = new ObservableCollection<int>();
        private ObservableDictionary<Guid, string> _heroNames = new ObservableDictionary<Guid, string>();
        private string _stringData;

        public ObservableCollection<int> NumberList { get { return _numberList; } set { SetField(ref _numberList, value); } }
        public ObservableDictionary<Guid, string> HeroNames { get { return _heroNames; } set { SetField(ref _heroNames, value); } }

        public string StringData { get { return _stringData; } set { SetField(ref _stringData, value);} }

        public SyncMockDB()
        {
            // Hook up CollectionChanged events to INotifySubCollectionChanged
            NumberList.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(NumberList), args);
            HeroNames.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(HeroNames), args);
        }

        public void AddName()
        {
            HeroNames.Add(Guid.NewGuid(), "Tracer");
        }

        public void AddNumber(int number)
        {
            NumberList.Add(number);
        }
    }

    [TestFixture]
    class NotifierTest
    {
        [Test]
        public void SyncMockTest()
        {
            // Create a dataBlob in the "ECSLib"
            var ECSLibDataBlob = new SyncMockDB();
            
            // Create a dataBlob in the "ViewModel"
            var ViewModelDataBlob = new SyncMockDB();

            // Directly sync the datablobs after every change.
            // Note, in the future, we'll catch this event and serialize/deserialize it to send from the ECSLib to the ViewModel
            ECSLibDataBlob.SubCollectionChanged += (sender, args) => SubCollectionSyncHelper.SyncCollection(ViewModelDataBlob, args);

            // Directly sync simple/primitive property changes.
            // Again, this can be easily serialized.
            ECSLibDataBlob.PropertyChanged += (sender, args) =>
                                              {
                                                  PropertyInfo changedProperty = ViewModelDataBlob.GetType().GetProperty(args.PropertyName);
                                                  object VMValue = changedProperty.GetValue(ViewModelDataBlob);
                                                  object ECSLibValue = changedProperty.GetValue(ECSLibDataBlob);

                                                  // This sets the VM value to the new value.
                                                  // Note, this fires INotifyPropertyChanged in the VM's datablob. We can databind directly to datablobs now.
                                                  changedProperty.SetValue(ViewModelDataBlob, ECSLibValue);
                                              };

            // Modify the data in the ECSLib.
            ECSLibDataBlob.AddNumber(new Random(DateTime.Now.Millisecond).Next());
            ECSLibDataBlob.AddName();
            ECSLibDataBlob.StringData = "New Data.";

            Assert.AreEqual(ECSLibDataBlob.NumberList[0], ViewModelDataBlob.NumberList[0]);
            Assert.AreEqual(ECSLibDataBlob.HeroNames.First(), ViewModelDataBlob.HeroNames.First());
            Assert.AreEqual(ECSLibDataBlob.StringData, ViewModelDataBlob.StringData);
        }
    }
}
