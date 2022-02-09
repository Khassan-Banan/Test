using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guide.Client.Services
{
    public class TopBarService
    {
        private bool enableRight = true;
        private bool openLeft = false;
        private bool openRight =  true;

        public bool OpenLeft
        {
            get
            {
                return openLeft;
            }
            set
            {
                openLeft = value;
                NotifyStateChanged();
            }
        }
        public bool OpenRight
        {
            get
            {
                return openRight;
            }
            set
            {
                
                openRight = value && enableRight;
                NotifyStateChanged();
            }
        }
       
        public void LockRight()
        {
            enableRight = false;
            NotifyStateChanged();
        }  
        public void UnLockRight()
        {
            enableRight = true;
            NotifyStateChanged();
        }
        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

    }
}
