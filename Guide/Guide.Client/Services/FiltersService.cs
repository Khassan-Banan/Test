using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guide.Client.Services
{
    public class FiltersService
    {
        private FilterParams filters = new();

        public FilterParams Filters
        {
            get => filters;
            private set
            {
                filters = value;
                Console.WriteLine("Set Filters");
                NotifyStateChanged();
            }
        }


        public void ChangeFilters( Action<FilterParams> action)
        {
            action(Filters);
            NotifyStateChanged();
        }
        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
