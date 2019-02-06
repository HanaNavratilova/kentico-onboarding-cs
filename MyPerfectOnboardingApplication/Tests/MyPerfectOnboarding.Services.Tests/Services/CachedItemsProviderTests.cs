﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Services.Services;
using MyPerfectOnboarding.Tests.Utils.Builders;
using NSubstitute;
using NUnit.Framework;

namespace MyPerfectOnboarding.Services.Tests.Services
{
    [TestFixture]
    internal class CachedItemsProviderTests
    {
        private readonly ListItem[] _items =
        {
            ListItemBuilder.CreateItem("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC", "aaaaa", "1589-12-03", "1896-04-07"),
            ListItemBuilder.CreateItem("11AC59B7-9517-4EDD-9DDD-EB418A7C1644", "dfads", "4568-06-23", "8569-08-24")
        };

        private IListRepository _listRepository;
        private CachedItemsProvider _cachedItemsProvider;

        [SetUp]
        public void Init()
        {
            _listRepository = Substitute.For<IListRepository>();
            

            _cachedItemsProvider = new CachedItemsProvider(_listRepository);
        }

        [Test]
        public async Task ExecuteOnItems_Function_FunctionWasCalled()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var function = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, ListItem>>();

            await _cachedItemsProvider.ExecuteOnItems(function);

            Assert.Multiple(() =>
            {
                function.Received(1);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ExecuteOnItemsTwice_Function_FunctionWasCalledTwiceRepositoryWasCalledOnce()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var function = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, ListItem>>();

            await _cachedItemsProvider.ExecuteOnItems(function);
            await _cachedItemsProvider.ExecuteOnItems(function);

            Assert.Multiple(() =>
            {
                function.Received(2);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ExecuteOnItemsTwice_TwoFunctions_FunctionWereCalledRepositoryWasCalledOnce()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var firstFunction = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, ListItem>>();
            var secondFunction = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, ListItem>>();

            await _cachedItemsProvider.ExecuteOnItems(firstFunction);
            await _cachedItemsProvider.ExecuteOnItems(secondFunction);

            Assert.Multiple(() =>
            {
                firstFunction.Received(1);
                secondFunction.Received(1);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ExecuteOnItemsAsync_Function_FunctionWasCalled()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var function = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>();

            await _cachedItemsProvider.ExecuteOnItemsAsync(function);

            Assert.Multiple(() =>
            {
                function.Received(1);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ExecuteOnItemsAsync_Function_FunctionWasCalledTwiceRepositoryWasCalledOnce()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var function = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>();

            await _cachedItemsProvider.ExecuteOnItemsAsync(function);
            await _cachedItemsProvider.ExecuteOnItemsAsync(function);

            Assert.Multiple(() =>
            {
                function.Received(2);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ExecuteOnItemsAsync_TwoFunctions_FunctionWereCalledRepositoryWasCalledOnce()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var firstFunction = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>();
            var secondFunction = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>();

            await _cachedItemsProvider.ExecuteOnItemsAsync(firstFunction);
            await _cachedItemsProvider.ExecuteOnItemsAsync(secondFunction);

            Assert.Multiple(() =>
            {
                firstFunction.Received(1);
                secondFunction.Received(1);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

        [Test]
        public async Task ExecuteOnItemsAsyncExecuteOnItems_TwoFunctions_FunctionWereCalledRepositoryWasCalledOnce()
        {
            _listRepository.GetAllItemsAsync().Returns(_items);
            var firstFunction = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, ListItem>>();
            var secondFunction = Substitute.For<Func<ConcurrentDictionary<Guid, ListItem>, Task<ListItem>>>();

            await _cachedItemsProvider.ExecuteOnItems(firstFunction);
            await _cachedItemsProvider.ExecuteOnItemsAsync(secondFunction);

            Assert.Multiple(() =>
            {
                firstFunction.Received(1);
                secondFunction.Received(1);
                _listRepository.Received(1).GetAllItemsAsync();
            });
        }

    }
}
