﻿using Core.Features.Interns.Entities;

namespace Core.Features.Interns.RequestModels
{
    public record AddStateRequest(
        Guid InternId, 
        Guid CampaignId, 
        StatusId StatusId, 
        string Justification,
        string? WorkEmail = null);
}
