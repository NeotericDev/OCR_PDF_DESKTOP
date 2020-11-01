using Akka.Actor;
using OCR_PDF.Actors.UI;
using OCR_PDF.Core;
using OCR_PDF.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OCR_PDF.Actors.Workers
{
    public class CancellableStashingActor : ReceiveActor, IWithUnboundedStash
    {
        private Dictionary<object, CancellationTokenSource> _cancelTokens;

        public CancellableStashingActor()
        {
            InitNewCancellationToken();
        }

        public IStash Stash 
        { get ; set; }


        protected Task ProcessAsync(object id, Action action, Action<Task> OnCancelled)
        {
            CancellationTokenSource cancelToken = new CancellationTokenSource();
             object _id = id;
            _cancelTokens.Add(id, cancelToken);
            return Task.Run(action, cancelToken.Token).ContinueWith((t)=> { 
                if((t.IsCompleted && t.IsCompletedSuccessfully) || t.IsCanceled || t.IsFaulted)
                {
                    _cancelTokens.Remove(_id);
                }
            }, TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith(OnCancelled==null?(t)=>{ }:OnCancelled, TaskContinuationOptions.ExecuteSynchronously);
        }

        protected void InitNewCancellationToken()
        {
            _cancelTokens = new Dictionary<object, CancellationTokenSource>();
        }

        protected bool CancelToken(object id)
        {
            if (id != null && _cancelTokens.ContainsKey(id))
            {
                CancellationTokenSource token = _cancelTokens.GetValueOrDefault(id);
                token.Cancel();
                return true;
            }
            return false;
            
        }
        protected bool CancelTokenForTask(object id)
        {
            var taskKey = _cancelTokens.Keys.First((key) =>
            {
                if (key is MsgTaskUnitIds ids)
                {
                    return ids.TaskEquals(ids);
                }
                else
                {
                    return ((Guid)key).Equals(id);
                }
            });
            return CancelToken(taskKey);
        }
        protected void CancelTokenForBatchId(Guid batchId)
        {
            var batchKey = _cancelTokens.Keys.First((key) =>
            {
                if (key is MsgTaskUnitIds ids)
                {
                    return ids.BatchEquals(batchId);
                }
                else
                {
                    return ((Guid)key).Equals(batchId);
                }
            });
            CancelToken(batchKey);    
        }
    }
}
