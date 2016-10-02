using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using Pulsar4X.ViewModel;

namespace Pulsar4X.CrossPlatformUI.Views.ComponentListView
{
    public class ComponentsByAtbView : Panel
    {
        protected GenericStackControl ComponentDesignsListStack;
        public ComponentsByAtbView()
        {
            XamlReader.Load(this);
            ComponentDesignsListStack.ControlType = typeof(ComponentDesignDetialView);
            DataContextChanged += ComponentsByAtbView_DataContextChanged;
        }

        private void ComponentsByAtbView_DataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is ComponentAtbsListVM)
            {
                var dc = (ComponentAtbsListVM)DataContext;

            }
        }
    }
}
