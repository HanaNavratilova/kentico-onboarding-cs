using System;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Tests.Utils.Builders
{
    public static class ListItemBuilder
    {
        public static ListItem CreateItem(
            string id,
            string text,
            string creationTime,
            string lastUpdateTime = null,
            bool isActive = false)
        {
            var typedId = Guid.Parse(id);

            var typedCreationTime = DateTime.Parse(creationTime);
            var typedLastUpdateTime = DateTime.Parse(lastUpdateTime ?? creationTime);

            return CreateItem(typedId, text, isActive, typedCreationTime, typedLastUpdateTime);
        }

        public static ListItem CreateItem(string id, string text)
            => CreateItem(id, text, "240-02-29", "872-01-31", true);

        public static ListItem CreateItem(string id)
            => CreateItem(id, "arbitrary text", "3240-02-29", "4872-01-31", true);

        private static ListItem CreateItem(
            Guid id,
            string text,
            bool isActive,
            DateTime creationTime,
            DateTime lastUpdateTime)
        => new ListItem()
                .With(item => item.Id, id)
                .With(item => item.Text, text)
                .With(item => item.CreationTime, creationTime)
                .With(item => item.LastUpdateTime, lastUpdateTime)
                .With(item => item.IsActive, isActive);
    }
}
