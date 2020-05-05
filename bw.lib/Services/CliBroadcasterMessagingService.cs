using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Models.Domain;

namespace bw.lib.Services
{
    public class CliBroadcasterMessagingService : IMessagingService
    {
        private readonly IBroadcasterService _broadcasterService;

        public CliBroadcasterMessagingService(IBroadcasterService broadcasterService)
        {
            _broadcasterService = broadcasterService;
        }

        public void Send(string subscriber, object arg = null)
        {
            var message = new Message { Command = subscriber, Data = arg };
            _broadcasterService.Send(message);
        }
    }
}
