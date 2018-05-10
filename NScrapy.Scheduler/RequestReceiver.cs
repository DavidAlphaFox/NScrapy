﻿using NScrapy.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace NScrapy.Scheduler
{
    public static class RequestReceiver
    {
        private static Queue<IRequest> queue = new Queue<IRequest>();
        private static object lockObj = new object();
        public const string REQUESTTHREADNAME = "ReceiverThread";
        public static Queue<IRequest> RequestQueue
        {
            get { return queue; }
        }
        public static void StartReceiver()
        {            
            Thread thread = new Thread(ListenToQueue)
            {
                Name = "ReceiverThread"
            };
            thread.Start();           
        }

        private static void ListenToQueue()
        {
            while (true)
            {
                lock (lockObj)
                {
                    if (queue.Count > 0)
                    {
                        var request = queue.Dequeue();
                        if (request == null)
                        {
                            continue;
                        }
                        if(NScrapyContext.CurrentContext.CurrentScheduler.UrlFilter.IsUrlVisited(request.URL).Result)
                        {
                            NScrapyContext.CurrentContext.Log.Info($"{request.URL} already visited");
                            continue;                            
                        }
                        NScrapyContext.CurrentContext.VisitedUrl++;
                        var result = NScrapyContext.CurrentContext.CurrentEngine.ProcessRequestAsync(request);

                        result.ContinueWith(u =>
                        {
                            NScrapyContext.CurrentContext.CurrentScheduler.SendResponseToDistributer(u.Result);
                            NScrapyContext.CurrentContext.Log.Info($"Sending request to {request.URL} success!");
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion);
                        result.ContinueWith(u => NScrapyContext.CurrentContext.Log.Info($"Sending request to {request.URL} failed", result.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
            }
        }
    }
}
