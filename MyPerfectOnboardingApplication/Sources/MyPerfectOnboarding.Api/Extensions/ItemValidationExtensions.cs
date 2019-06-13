using System;
using System.Web.Http.ModelBinding;
using MyPerfectOnboarding.Api.Models;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Api.Extensions
{
    internal static class ItemValidationExtensions
    {
        public static ModelStateDictionary ValidateBeforeEditing(
            this ModelStateDictionary modelState,
            Guid requestId,
            ListItemViewModel item)
            => modelState
                .ValidateItemExistence(item)
                .ContinueOnlyWithValid(() => modelState
                    .ValidateRequestId(requestId)
                    .ValidateModelId(item, requestId)
                    .ValidateText(item.Text)
                    .ValidateCreationTime(item.CreationTime)
                    .ValidateLastUpdateTime(item.LastUpdateTime));

        public static ModelStateDictionary ValidateBeforeAddition(
            this ModelStateDictionary modelState,
            ListItemViewModel item)
            => modelState
                .ValidateItemExistence(item)
                .ContinueOnlyWithValid(() => modelState
                    .ValidateModelId(item)
                    .ValidateText(item.Text)
                    .ValidateCreationTime(item.CreationTime)
                    .ValidateLastUpdateTime(item.LastUpdateTime));

        private static ModelStateDictionary ContinueOnlyWithValid(
            this ModelStateDictionary modelState,
            Func<ModelStateDictionary> validationFunction)
        {
            if (!modelState.IsValid)
            {
                return modelState;
            }

            return validationFunction();
        }

        private static bool IsIdEmptyGuid(Guid id)
            => id == Guid.Empty;

        private static ModelStateDictionary ValidateModelId(this ModelStateDictionary modelState, IListItem item)
        {
            if (!IsIdEmptyGuid(item.Id))
            {
                AddNonemptyModelIdError(modelState);
            }

            return modelState;
        }

        private static ModelStateDictionary ValidateModelId(this ModelStateDictionary modelState, IListItem item, Guid requestId)
        {
            if (IsIdEmptyGuid(item.Id))
            {
                return modelState;
            }

            if (item.Id == requestId)
            {
                AddNonemptyModelIdError(modelState);
            }
            else
            {
                modelState.AddRequestIdDifferentFromModelIdError();
            }

            return modelState;
        }

        private static void AddRequestIdDifferentFromModelIdError(this ModelStateDictionary modelState)
            => modelState.AddModelError(nameof(IListItem.Id),
                "Identifier is invalid. It should be empty. Warning: You have different id in request and in item.");

        private static void AddNonemptyModelIdError(this ModelStateDictionary modelState)
            => modelState.AddModelError(nameof(IListItem.Id), "Identifier is invalid. It should be empty.");

        public static ModelStateDictionary ValidateRequestId(this ModelStateDictionary modelState, Guid id)
        {
            if (IsIdEmptyGuid(id))
            {
                modelState.AddModelError(nameof(IListItem.Id),
                    "Identifier in request URI is invalid. It should not be empty.");
            }

            return modelState;
        }

        private static ModelStateDictionary ValidateTime(
            this ModelStateDictionary modelState,
            DateTime time,
            string name,
            string errorMessage)
        {
            if (time != DateTime.MinValue)
            {
                modelState.AddModelError(name, errorMessage);
            }

            return modelState;
        }

        private static ModelStateDictionary ValidateCreationTime(
            this ModelStateDictionary modelState,
            DateTime time)
            => modelState.ValidateTime(
                time,
                nameof(IListItem.CreationTime),
                errorMessage: "Creation time should be DateTime.MinValue.");

        private static ModelStateDictionary ValidateLastUpdateTime(
            this ModelStateDictionary modelState,
            DateTime time)
            => modelState.ValidateTime(
                time,
                nameof(IListItem.LastUpdateTime),
                errorMessage: "Last update time should be DateTime.MinValue.");

        private static ModelStateDictionary ValidateText(this ModelStateDictionary modelState, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                modelState.AddModelError(nameof(IListItem.Text), "Text was empty.");
            }

            return modelState;
        }

        private static ModelStateDictionary ValidateItemExistence(this ModelStateDictionary modelState, ListItemViewModel item)
        {
            if (item == null)
            {
                modelState.AddModelError(nameof(IListItem), "Item should not be null.");
            }

            return modelState;
        }
    }
}