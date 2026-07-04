using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.Interfaces;

public interface IBackgroundJobService
{
    Task ExpireReservationsAsync();

    Task RecalculateTrendingScoresAsync();

    Task FlushAnalyticsCountersAsync();
}