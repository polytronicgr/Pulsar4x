using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using Pulsar4X.ECSLib;
using Pulsar4X.ViewModel;

namespace Pulsar4X.CrossPlatformUI.Views
{
    public class IndustryView : Scrollable
    {
        public GridView ComponentGridView;
        protected GridColumn ItemNameColumn;
        protected GridColumn NumberCompletedColumn;
        protected GridColumn NumberOrderedColumn;
        protected GridColumn IndustrialUtilizationColumn;
        protected GridColumn BPPerItemColumn;
        protected GridColumn BPAppliedColumn;
        protected GridColumn TotalBPRemainingColumn;
        protected GridColumn AutoRepeatColumn;
        protected GridColumn ProjectCompletionColumn;

        private readonly IndustrialEntityVM _industrialEntityVM;

        private IndustryView()
        {
            XamlReader.Load(this);
        }

        public IndustryView(Entity entity) : this()
        {
            _industrialEntityVM = new IndustrialEntityVM(entity);
            InitializeDataBinding();
        }

        public IndustryView(IndustrialEntityVM industrialEntityVM) : this()
        {
            _industrialEntityVM = industrialEntityVM;
            InitializeDataBinding();
        }

        private void InitializeDataBinding()
        { 
            ComponentGridView.DataStore = _industrialEntityVM.Jobs[IndustryType.ComponentConstruction];
            ItemNameColumn.DataCell = new TextBoxCell("ItemName");
            NumberCompletedColumn.DataCell = new TextBoxCell("NumberCompleted");
            IndustrialUtilizationColumn.DataCell = new TextBoxCell("IndustrialUtilization");
            BPPerItemColumn.DataCell = new TextBoxCell("BPPerItem");
            BPAppliedColumn.DataCell = new TextBoxCell("BPApplied");
            TotalBPRemainingColumn.DataCell = new TextBoxCell("TotalBPRemaining");
            AutoRepeatColumn.DataCell = new CheckBoxCell("AutoRepeat");
            ProjectCompletionColumn.DataCell = new TextBoxCell("ProjectCompletionDate");
        }
    }
}
