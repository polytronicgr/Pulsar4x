using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace Pulsar4X.CrossPlatformUI.Views.ComponentListView
{
    public class ComponentDesignDetialView : Panel
    {
        protected GenericStackControl ComponentAttributesStack;
        public ComponentDesignDetialView()
        {
            XamlReader.Load(this);
            ComponentAttributesStack.ControlType = typeof(ComponentDesignAttributeView);
            DataContextChanged += ComponentDesignDetialView_DataContextChanged;
        }

        private void ComponentDesignDetialView_DataContextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
