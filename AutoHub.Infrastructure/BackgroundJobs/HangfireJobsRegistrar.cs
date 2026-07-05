using AutoHub.Application.Interfaces;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.BackgroundJobs;

public static class HangfireJobRegistrar
{
    public static void Register()
    {
        RecurringJob.AddOrUpdate<
            IBackgroundJobService>(
            "expire-reservations",
            x => x.ExpireReservationsAsync(),
            "*/15 * * * *");

        RecurringJob.AddOrUpdate<
            IBackgroundJobService>(
            "recalculate-trending",
            x => x.RecalculateTrendingScoresAsync(),
            "5 * * * *");

        RecurringJob.AddOrUpdate<
            IBackgroundJobService>(
            "flush-analytics",
            x => x.FlushAnalyticsCountersAsync(),
            "0 * * * *");
    }
}
