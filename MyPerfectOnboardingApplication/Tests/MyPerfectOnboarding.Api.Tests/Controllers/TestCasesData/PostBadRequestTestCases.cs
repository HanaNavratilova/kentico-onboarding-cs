using System.Collections;
using System.Collections.Generic;
using MyPerfectOnboarding.Contracts.Models;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers.TestCasesData
{
    internal class PostBadRequestTestCases : IEnumerable<TestCaseData>
    {
        public IEnumerator<TestCaseData> GetEnumerator()
        {
            yield return new TestCaseData(
                    ViewModelItem.WithEmptyText,
                    new[] { nameof(IListItem.Text) })
                .SetName("Post_ItemOnlyWithEmptyText_BadRequestReturned");

            yield return new TestCaseData(
                    ViewModelItem.WithIdAndText, 
                    new[] { nameof(IListItem.Id) })
                .SetName("Post_ItemWithNonemptyTextAndId_BadRequestReturned");

            yield return new TestCaseData(
                    ViewModelItem.WithEmptyIdTextAndCreationTime,
                    new[] { nameof(IListItem.CreationTime) })
                .SetName("Post_ItemWithNonemptyTextAndCreationTime_BadRequestReturned");

            yield return new TestCaseData(
                    ViewModelItem.WithIdTextAndCreationTime,
                    new[] { nameof(IListItem.Id), nameof(IListItem.CreationTime) })
                .SetName("Post_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned");

            yield return new TestCaseData(
                    ViewModelItem.WithEmptyIdTextAndTimes,
                    new[] { nameof(IListItem.LastUpdateTime), nameof(IListItem.CreationTime) })
                .SetName("Post_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned");

            yield return new TestCaseData(
                    ViewModelItem.WithIdAndEmptyText,
                    new[] { nameof(IListItem.Text), nameof(IListItem.Id) })
                .SetName("Post_ItemWithEmptyTextAndId_BadRequestReturned");
        }

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}
