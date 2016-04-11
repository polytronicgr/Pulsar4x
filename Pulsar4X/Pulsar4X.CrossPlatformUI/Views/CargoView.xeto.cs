using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using Pulsar4X.ViewModel;

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

            ItemTypeColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.IndustryType) };
            ItemCargoTypeColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.CargoType) };
            AmountStoredColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, double>(r => r.Amount).Convert(r => r.ToString()) };
            //WeightColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.Weight.ToString()) };
            //SpaceRemainingColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.SpaceLeft.ToString()) };

        }

    }
}
