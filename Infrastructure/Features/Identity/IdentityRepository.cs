using Core.Common.Exceptions;
using Core.Features.Identity.Interfaces;
using Core.Features.Identity.ResponseModels;
using Microsoft.Graph;
using System.Net;

namespace Infrastructure.Features.Identity
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly GraphServiceClient graphClient;

        public IdentityRepository(GraphServiceClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public async Task<IdentitySummaryResponse> SendUserInviteAsync(string userEmail, string applicationUrl)
        {
            try
            {
                var invitation = new Invitation
                {
                    InvitedUserEmailAddress = userEmail,
                    InviteRedirectUrl = applicationUrl,
                    SendInvitationMessage = true
                };

                var azureInvitationResponse = await graphClient.Invitations.Request().AddAsync(invitation);

                var invitedUser = await graphClient.Users[azureInvitationResponse.InvitedUser.Id].Request().GetAsync();

                var identitySummary = new IdentitySummaryResponse(invitedUser.Mail, invitedUser.DisplayName);

                return identitySummary;
            }
            catch(ServiceException ex)
            {
                throw new CoreException($"Problem occurred retrieving requested user identity. [{ex.Error.Message}]", ex.StatusCode);
            }

        }
    }
}