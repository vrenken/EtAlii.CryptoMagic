namespace EtAlii.BinanceMagic.Service
{
    using Blazorise;

    public abstract partial class ViewBase<TTransaction, TTrading, TRunner> 
        where TTransaction: TransactionBase<TTrading>
        where TTrading : TradingBase, new()
        where TRunner : IAlgorithmRunner<TTransaction, TTrading>
    {

        protected Modal DeleteDialogRef;
        
        protected void ShowDeleteDialog()
        {
            DeleteDialogRef.Show();
        }
        
        protected void DeletionCanceled()
        {
            DeleteDialogRef.Hide();
        }
        
        protected void DeletionConfirmed()
        {
            DeleteDialogRef.Hide();
            
            AlgorithmManager.Remove(Model);
            
            var navigationUrl = GetListUrl();
            NavigationManager.NavigateTo(navigationUrl);
        }
    }
}