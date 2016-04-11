using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using Pulsar4X.ViewModel;
using Pulsar4X.ECSLib;

namespace Pulsar4X.CrossPlatformUI.Views
{
    public class CargoView : Panel
    {
        protected GridView ComponentGridView;
        protected GridColumn ItemTypeColumn;
        protected GridColumn ItemCargoTypeColumn;
        protected GridColumn ItemNameColumn;
        protected GridColumn AmountStoredColumn;
        protected GridColumn WeightColumn;
        protected GridColumn SpaceRemainingColumn;


        public CargoView()
        {
            XamlReader.Load(this);

        }
        public void Initialise(CargoVM vm) 
        {
            DataContext = vm;
            ComponentGridView.DataStore = vm.CargoData;
            ItemNameColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.Name) };

            ItemTypeColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, IndustryType>(r => r.IndustryType).Convert(r => r.ToString()) };
            ItemCargoTypeColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, CargoType>(r => r.CargoType).Convert(r=> r.ToString()) };
            AmountStoredColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, double>(r => r.Amount).Convert(r => r.ToString()) };
            //WeightColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.Weight.ToString()) };
            //SpaceRemainingColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.SpaceLeft.ToString()) };

        }

    }
}
