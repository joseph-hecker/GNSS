using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace GNSS.Workflows;

public class SkadiEPMWorkflow(Func<CancellationToken, Task> waitingForRequirements) : GNSSWorkflow(waitingForRequirements)
{
    public new bool RequireDGPS => true;
    public new bool RequireRTKFix => true;

    protected override bool CheckRequirements(GPSData data)
    {
        ArgumentNullException.ThrowIfNull(data.EnavData, nameof(data.EnavData));
        return base.CheckRequirements(data)
            && data.EnavData.ValidCompData;
    }

    public override async Task<GPSData> CollectVertexAsync()
    {
        CancellationToken ct = GPS.WaitForRequirements(CheckRequirements);
        await WaitingForRequirementsAsync(ct);

        ct = GPS.WaitForSkadiTriggerPressed();
        TaskCompletionSource<GPSData> source = new();
        var registration = ct.UnsafeRegister(async (state, ct) =>
        {
            source.SetResult(await GPS.GetNextLocationAsync());
        }, null);

        GPSData data = await source.Task;
        registration.Unregister();
        if (!CheckRequirements(data))
        {
            return await CollectVertexAsync();
        }

        LogCollection(data);
        return data;
    }
}
