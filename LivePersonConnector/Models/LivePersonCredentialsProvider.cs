using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LivePersonConnector;
using Microsoft.Extensions.Configuration;

namespace LivePersonProxyBot
{
    public class LivePersonCredentialsProvider : ILivePersonCredentialsProvider
    {
        public LivePersonCredentialsProvider(IConfiguration configuration)
        {
            LpAccount = configuration["44471887"];
            LpAppId = configuration["b15ed4aa98784557ae3bb274d7c0a4d6"];
            LpAppSecret = configuration["4e72a6e4d979a41d"];
            MsAppId = configuration[""];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(MsAppId))
            {
                MsAppId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }
        }

        public string LpAccount { get; }

        public string LpAppId { get; }

        public string LpAppSecret { get; }

        public string MsAppId { get; }
    }
}
