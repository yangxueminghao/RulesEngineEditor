// Copyright (c) Alex Reich.
// Licensed under the CC BY 4.0 License.

using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace RulesEngineEditorServer.Authentication
{
    public class OperationalStoreOptionsMigrations : IOptions<OperationalStoreOptions>
    {
        public OperationalStoreOptions Value => new OperationalStoreOptions() {
            DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
            EnableTokenCleanup = false,
            PersistedGrants = new TableConfiguration("PersistedGrants"),
            TokenCleanupBatchSize = 100,
            TokenCleanupInterval = 3600,
        };
    }
}
