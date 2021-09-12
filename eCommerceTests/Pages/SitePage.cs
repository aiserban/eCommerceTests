using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace eCommerceTests.Pages
{
    public abstract class SitePage
    {
        public IWebDriver Driver { get; set; }
        private const string SearchFieldId = "searchingfield";
        private const string ProductsInBasketCountCssSelector = ".shopping-cart .qty-mc";
        private const string BasketButtonId = "carted";

        public IWebElement SearchField { get { return this.Driver.FindElement(By.Id(SearchFieldId)); } }
        public IWebElement ProductsInBasketCount { get { return this.Driver.FindElement(By.CssSelector(ProductsInBasketCountCssSelector)); } }

        public SitePage(IWebDriver driver)
        {
            this.Driver = driver;
        }

        public SearchResultsPage Search(string searchText)
        {
            SearchField.SendKeys(searchText);
            SearchField.SendKeys(Keys.Enter);
            return new SearchResultsPage(this.Driver);
        }

        public int CountItemsInBasket()
        {
            var wait = new WebDriverWait(this.Driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(ProductsInBasketCountCssSelector)));
            var count = int.Parse(ProductsInBasketCount.Text);
            return count;
        }

        public BasketPage NavigateToBasketPage()
        {
            this.Driver.FindElement(By.Id(BasketButtonId)).Click();
            return new BasketPage(this.Driver);
        }
    }
}
