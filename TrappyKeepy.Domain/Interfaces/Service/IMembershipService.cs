﻿using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IMembershipService
    {
        Task<IMembershipServiceResponse> Create(IMembershipServiceRequest request);
        Task<IMembershipServiceResponse> DeleteById(IMembershipServiceRequest request);
        Task<IMembershipServiceResponse> DeleteByGroupId(IMembershipServiceRequest request);
        Task<IMembershipServiceResponse> DeleteByUserId(IMembershipServiceRequest request);
    }
}
