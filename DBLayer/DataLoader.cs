using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public abstract class  DataLoader
    {
        // Events
        public event OnCompleteHandler Complete;

        // Methods
        protected DataLoader()
        {
        }

        protected void OnComplete(object sender, DataLoaderEventArgs e)
        {
            if (this.Complete != null)
            {
                this.Complete(sender, e);
            }
        }

        public abstract void StartLoadingData();

        // Properties
        public abstract int ExitCode { get; }

        public abstract string ExitMessage { get; }

        // Nested Types
        public delegate void OnCompleteHandler(object sender, DataLoaderEventArgs e);

    }
}
