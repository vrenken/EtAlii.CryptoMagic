namespace EtAlii.CryptoMagic
{
    using Blazorise;

    public partial class OneOffList 
    {
        private Modal _cancelDialogRef;

        private OneOffAlgorithmRunner _runnerToDelete;

        private void ShowCancelDialog(IAlgorithmRunner<OneOffTransaction, OneOffTrading> runner)
        {
            _runnerToDelete = (OneOffAlgorithmRunner)runner;
            _cancelDialogRef.Show();
        }

        private void CancelCanceled()
        {
            _cancelDialogRef.Hide();
        }

        private void CancelConfirmed()
        {
            _cancelDialogRef.Hide();

            _runnerToDelete.Cancel();
        }
    }
}