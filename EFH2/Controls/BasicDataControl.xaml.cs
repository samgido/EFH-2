using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	public sealed partial class BasicDataControl : UserControl
    {
        public BasicDataControl()
        {
            this.InitializeComponent();
        }

        public void SetDataContext()
        {
            if (DataContext is BasicDataViewModel model)
            {
                DrainageAreaControls.DataContext = model.drainageAreaEntry;
                RunoffCurveNumberControls.DataContext = model.runoffCurveNumberEntry;
                WatershedLengthControls.DataContext = model.watershedLengthEntry;
                WatershedSlopeControls.DataContext = model.watershedSlopeEntry;
                TimeOfConcentrationControls.DataContext = model.timeOfConcentrationEntry;
            }
        }
    }
}
