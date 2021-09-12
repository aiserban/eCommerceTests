using System;
using System.Linq;
using eCommerceTests.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace eCommerceTests.Tests
{
    public class BasketTests
    {
        public IWebDriver Driver { get; set; }
        public readonly Uri HomePageUri = new Uri("https://flanco.ro/");

        [SetUp]
        public void Setup()
        {
            this.Driver = new ChromeDriver();
            this.Driver.Manage().Window.Maximize();
            this.Driver.Navigate().GoToUrl(HomePageUri);
        }

        [Test]
        public void CanAddMultipleItemsToBasket()
        {
            /* There's an issue about 1/2 attempts with Selenium throwing a ElementClickInterceptedException
            when adding the second product to the basket.
               Debugging would take more time that is reasonable, so I'm mentioning it as a known issue
               If you run into it, please just run the tests again.
            */

            var searchText = "iPhone 12";
            bool successfullyAdded;
            int initialItemsInBasket;
            int expectedItemsInBasket = 2;
            var firstAddedItem = string.Empty;
            var secondAddedItem = string.Empty;

            var homePage = new HomePage(this.Driver);
            initialItemsInBasket = homePage.CountItemsInBasket();
            Assert.AreEqual(0, initialItemsInBasket);

            // Add first item
            var searchResultsPage = homePage.Search(searchText)
                                            .AddRandomProductToBasket(out successfullyAdded, out firstAddedItem);
            var actualItemsInBasket = searchResultsPage.CountItemsInBasket();


            Assert.AreEqual(true, successfullyAdded);
            Assert.IsNotEmpty(firstAddedItem);

            // Add second item to Basket
            successfullyAdded = false;
            searchResultsPage.AddRandomProductToBasket(out successfullyAdded, out secondAddedItem);
            actualItemsInBasket = searchResultsPage.CountItemsInBasket();

            Assert.AreEqual(true, successfullyAdded);
            Assert.AreEqual(expectedItemsInBasket, actualItemsInBasket);
            Assert.IsNotEmpty(secondAddedItem);

            var basketPage = searchResultsPage.NavigateToBasketPage();
            var productsInBasket = basketPage.GetAllProductTitlesInBasket().ToArray();

            Assert.Contains(firstAddedItem, productsInBasket);
            Assert.Contains(secondAddedItem, productsInBasket);
        }

        [Test]
        public void CanRemoveProductsFromBasket()
        {
            /* There's an issue about 1/2 attempts with Selenium throwing a ElementClickInterceptedException
    when adding the second product to the basket.
       Debugging would take more time that is reasonable, so I'm mentioning it as a known issue
       If you run into it, please just run the tests again.
    */

            var searchText = "iPhone 12";
            var successfullyAdded = false;
            var countAfterFirstRemoval = -1;
            var countAfterSecondRemoval = -1;
            var countAfterAddingAllItems = -1;
            var expectedCountAfterAddingAllItems = 2;

            var basketPage = new HomePage(this.Driver).Search(searchText)
                                                        .AddRandomProductToBasket(out successfullyAdded, out _)
                                                        .AddRandomProductToBasket(out successfullyAdded, out _)
                                                        .CountItemsInBasket(out countAfterAddingAllItems)
                                                        .NavigateToBasketPage()
                                                        .RemoveProductAt(1)
                                                        .CountItemsInBasket(out countAfterFirstRemoval)
                                                        .RemoveProductAt(0)
                                                        .CountItemsInBasket(out countAfterSecondRemoval);

            var isBasketEmptyMessageDisplayed = basketPage.IsEmptyBasketMessageDisplayed();
            Assert.AreEqual(expectedCountAfterAddingAllItems, countAfterAddingAllItems);
            Assert.AreEqual(expectedCountAfterAddingAllItems - 1, countAfterFirstRemoval);
            Assert.AreEqual(expectedCountAfterAddingAllItems - 2, countAfterSecondRemoval);
            Assert.AreEqual(true, isBasketEmptyMessageDisplayed);
        }

        [TearDown]
        public void TearDown()
        {
            this.Driver.Quit();
        }
    }
}
