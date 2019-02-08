using System;
using MyPerfectOnboarding.Api.Models;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Api.Tests.Builders
{
    internal static class ListItemViewModelBuilder
    {
        public static ListItemViewModel CreateItem(string text)
            => CreateItem(null, text);

        public static ListItemViewModel CreateItem(string id, string text, string creationTime = null, string lastUpdateTime = null, bool isActive = false)
        {
            var typedId = string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);

            var typedCreationTime = string.IsNullOrEmpty(creationTime) ? DateTime.MinValue : DateTime.Parse(creationTime);
            var typedLastUpdateTime = string.IsNullOrEmpty(lastUpdateTime) ? DateTime.MinValue : DateTime.Parse(lastUpdateTime);

            return CreateItem(typedId, text, isActive, typedCreationTime, typedLastUpdateTime);
        }

        private static ListItemViewModel CreateItem(Guid id,
            string text,
            bool isActive,
            DateTime creationTime,
            DateTime lastUpdateTime)
            => new ListItemViewModel
            {
                Id = id,
                Text = text,
                IsActive = isActive,
                CreationTime = creationTime,
                LastUpdateTime = lastUpdateTime
            };
    }
}