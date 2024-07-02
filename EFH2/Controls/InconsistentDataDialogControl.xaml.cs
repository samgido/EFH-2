using Microsoft.UI.Xaml.Controls;

namespace EFH2
{
	public sealed partial class InconsistentDataDialogControl : UserControl
    {
        public InconsistentDataDialogControl()
        {
            this.InitializeComponent();
        }

        public void SetValues(double basicArea, double basicCurveNumber, double rcnArea, double rcnCurveNumber)
        {
            this.BasicArea.Text = Convert(basicArea);
            this.BasicCurveNumber.Text = Convert(basicCurveNumber);

            this.RcnArea.Text = Convert(rcnArea);
            this.RcnCurveNumber.Text = Convert(rcnCurveNumber);
        }

        private string Convert(double d)
        {
            if (double.IsNormal(d)) return d.ToString();
            else return string.Empty;
        }
    }
}
