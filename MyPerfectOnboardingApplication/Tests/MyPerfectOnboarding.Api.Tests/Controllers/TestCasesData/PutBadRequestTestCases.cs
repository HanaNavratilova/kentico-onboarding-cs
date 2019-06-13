using System;
using System.Collections;
using System.Collections.Generic;
using MyPerfectOnboarding.Contracts.Models;
using NUnit.Framework;

namespace MyPerfectOnboarding.Api.Tests.Controllers.TestCasesData
{
    internal class PutBadRequestTestCases : IEnumerable<TestCaseData>
    {
        public IEnumerator<TestCaseData> GetEnumerator()
        {
            yield return new TestCaseData(
                    Guid.Empty,
                    ViewModelItem.WithText,
                    new[] { nameof(IListItem.Id) })
                .SetName("Put_InvalidId_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("858D8817-CB6B-449E-AD1E-A5F27366F846"),
                    null,
                    new[] { nameof(IListItem) })
                .SetName("Put_NullItem_BadRequest");

            yield return new TestCaseData(
                    Guid.Parse("CDC5076D-237C-48DC-9078-969BA7789D72"),
                    ViewModelItem.WithEmptyText,
                    new[] { nameof(IListItem.Text) })
                .SetName("Put_ItemOnlyWithEmptyText_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("CDC5076D-237C-48DC-9078-969BA7789D72"),
                    ViewModelItem.WithIdAndText,
                    new[] { nameof(IListItem.Id) })
                .SetName("Put_ItemWithNonemptyTextAndId_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("E7B3EEAE-5618-4F57-ABA5-E958362DC119"),
                    ViewModelItem.WithEmptyIdTextAndCreationTime,
                    new[] { nameof(IListItem.CreationTime) })
                .SetName("Put_ItemWithNonemptyTextAndCreationTime_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("FCC43E93-4750-416A-9C83-F8304F20DB91"),
                    ViewModelItem.WithIdTextAndCreationTime,
                    new[] { nameof(IListItem.Id), nameof(IListItem.CreationTime) })
                .SetName("Put_ItemWithNonemptyTextAndCreationTimeAndId_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("D7ACFA1C-9EB0-4F9B-9FC4-F680FE1CDB79"),
                    ViewModelItem.WithEmptyIdTextAndTimes,
                    new[] { nameof(IListItem.LastUpdateTime), nameof(IListItem.CreationTime) })
                .SetName("Put_ItemWithNonemptyTextAndCreationTimeAnLastUpdateTime_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("4B346C12-FEF7-417D-A945-60A663C8F1C1"),
                    ViewModelItem.WithIdAndEmptyText,
                    new[] { nameof(IListItem.Id),  nameof(IListItem.Text) })
                .SetName("Put_ItemWithEmptyTextAndId_BadRequestReturned");

            yield return new TestCaseData(
                    Guid.Parse("E9D1B9CD-A799-4B2C-8513-38C04564CF03"),
                    ViewModelItem.WithIdAndText,
                    new[] { nameof(IListItem.Id) })
                .SetName("Put_ItemWithTextAndIdAndDifferentIdInUrl_BadRequestReturned");
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
