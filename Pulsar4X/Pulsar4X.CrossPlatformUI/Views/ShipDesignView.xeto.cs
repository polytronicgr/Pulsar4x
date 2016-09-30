using Eto.Forms;
using Eto.Serialization.Xaml;
using Pulsar4X.ViewModel;


namespace Pulsar4X.CrossPlatformUI.Views
{
    public class ShipDesignView : Panel
    {
        protected GenericStackControl ComponentListStack; 

        public ShipDesignView()
        {
            XamlReader.Load(this);
            DataContextChanged += ShipDesignView_DataContextChanged;

        }

        private void ShipDesignView_DataContextChanged(object sender, System.EventArgs e)
        {
            if (DataContext is ShipDesignVM)
            {
            }
        }

    }
}
