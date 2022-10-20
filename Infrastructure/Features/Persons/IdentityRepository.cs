using Core.Common.Exceptions;
using Core.Features.Persons.Interfaces;
using Core.Features.Persons.ResponseModels;
using Microsoft.Graph;

namespace Infrastructure.Features.Persons
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

                var azureInvitationResponse = await graphClient
                    .Invitations
                    .Request()
                    .AddAsync(invitation);

                var invitedUser = await graphClient
                    .Users[azureInvitationResponse.InvitedUser.Id]
                    .Request()
                    .GetAsync();

                var identitySummaryResponse = new IdentitySummaryResponse(invitedUser.Mail, invitedUser.DisplayName);

                return identitySummaryResponse;
            }
            catch (ServiceException ex)
            {
                throw new CoreException($"Problem occurred retrieving requested user identity. [{ex.Error.Message}]", ex.StatusCode);
            }

        }
    }
}