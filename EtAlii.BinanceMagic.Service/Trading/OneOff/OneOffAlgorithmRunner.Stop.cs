namespace EtAlii.BinanceMagic.Service
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public partial class OneOffAlgorithmRunner 
    {
        public async Task Stop()
        {
            await _timer.DisposeAsync();
            Changed?.Invoke(this);
        }

        public void Cancel()
        {
            var task = Stop();
            task.Wait();
            
            using var data = new DataContext();

            Context.Trading.IsCancelled = true;
            data.OneOffTradings.Attach(Context.Trading).State = EntityState.Modified;
            data.SaveChanges();
            
            Changed?.Invoke(this);
        }
    }
}