using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace Pulsar4X.CrossPlatformUI.Views.ComponentListView
{
    public class FactionComponentsView : Scrollable
    {
        public FactionComponentsView()
        {
            XamlReader.Load(this);
        }
    }
}
