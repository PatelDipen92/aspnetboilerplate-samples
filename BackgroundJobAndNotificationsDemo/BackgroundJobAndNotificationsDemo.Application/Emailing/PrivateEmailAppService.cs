﻿using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using Abp.UI;
using BackgroundJobAndNotificationsDemo.Emailing.Dto;
using BackgroundJobAndNotificationsDemo.Users;

namespace BackgroundJobAndNotificationsDemo.Emailing
{
    [AbpAuthorize]
    public class PrivateEmailAppService : BackgroundJobAndNotificationsDemoAppServiceBase, IPrivateEmailAppService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public PrivateEmailAppService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public void Send(SendPrivateEmailInput input)
        {
            var targetUser = UserManager.FindByNameAsync(input.UserName);
            if (targetUser == null)
            {
                throw new UserFriendlyException("There is no such a user: " + input.UserName);
            }

            _backgroundJobManager.EnqueueAsync<SendPrivateEmailJob>(
                new SendPrivateEmailJobArgs
                {
                    Subject = input.Subject,
                    Body = input.Body,
                    SenderUserId = AbpSession.GetUserId(),
                    TargetTenantId = AbpSession.TenantId,
                    TargetUserId = targetUser.Id
                });
        }
    }
}