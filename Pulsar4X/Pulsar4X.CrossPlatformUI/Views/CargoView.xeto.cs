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

        private CargoVM _vm;

        public CargoView()
        {
            XamlReader.Load(this);
            ComponentGridView.ColumnHeaderClick += ComponentGridView_ColumnHeaderClick;
            ComponentGridView.Columns.Clear();
            ComponentGridView.Columns.Add(new GridColumn
            {
                HeaderText = "Name",  
                DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.Name)  }
                
            });
            
            ComponentGridView.Columns.Add(new GridColumn
            {
                HeaderText = "Item Type",
                DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, IndustryType>(r => r.IndustryType).Convert(r => r.ToString()) }
            });
            ComponentGridView.Columns.Add(new GridColumn
            {
                HeaderText = "Cargo Type",
                DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, CargoType>(r => r.CargoType).Convert(r => r.ToString()) }            
            });
            ComponentGridView.Columns.Add(new GridColumn
            {
                HeaderText = "Amount",
                DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, double>(r => r.Amount).Convert(r => r.ToString()) }
            });

            //ItemNameColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.Name) };

            //ItemTypeColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, IndustryType>(r => r.IndustryType).Convert(r => r.ToString()) };
            //ItemCargoTypeColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, CargoType>(r => r.CargoType).Convert(r=> r.ToString()) };
            //AmountStoredColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, double>(r => r.Amount).Convert(r => r.ToString()) };
            //WeightColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.Weight.ToString()) };
            //SpaceRemainingColumn.DataCell = new TextBoxCell { Binding = Binding.Property<CargoData, string>(r => r.SpaceLeft.ToString()) };

        }

        private void ComponentGridView_ColumnHeaderClick(object sender, GridColumnEventArgs e)
        {
            string header = e.Column.HeaderText;
            SortEnum sort = SortEnum.None;
            switch(header)
            {
                case "Item Type":
                    sort = SortEnum.ItemType;
                    break;
                case "Cargo Type":
                    sort = SortEnum.CargoType;
                    break;
                case "Amount":
                    sort = SortEnum.Amount;
                    break;
                case "Name":
                    sort = SortEnum.ItemName;
                    break;

            }
            _vm.OnReOrder(sort);
            
        }

        public void Initialise(CargoVM vm) 
        {
            _vm = vm;
            DataContext = vm;
            //ComponentGridView.DataStore = vm.CargoData;
        }

    }
}
