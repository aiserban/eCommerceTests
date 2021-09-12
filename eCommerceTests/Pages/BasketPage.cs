using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace eCommerceTests.Pages
{
    public class BasketPage : SitePage
    {
        private const string ProductCssSelector = ".item-info";
        private const string ProductDeleteButtonCssSelector = ".action-delete";
        private const string EmptyBasketMessageCssSelector = ".emptycart-welcome-text h2";
        private const string ProductTitlesCssSelector = "#shopping-cart-table > div.ncw-box__content > div > div > div > div > div > div.ncw-pprod-row > div > div.col.item.ncw-pprod__description > h3 > a";

        public BasketPage(IWebDriver driver) : base(driver)
        {
        }

        public IEnumerable<IWebElement> GetAllProductElementsInBasket()
        {
            var products = this.Driver.FindElements(By.CssSelector(ProductCssSelector)).ToList();
            return products;
        }

        public IEnumerable<string> GetAllProductTitlesInBasket()
        {
            var allTitles = this.Driver.FindElements(By.CssSelector(ProductTitlesCssSelector)).Select(e => e.Text);
            return allTitles;
        }

        public BasketPage RemoveProductAt(int index)
        {
            var productElements = this.GetAllProductElementsInBasket();
            var foundProduct = productElements.ElementAt(index);
            var deleteButtonForFoundProduct = foundProduct.FindElement(By.CssSelector(ProductDeleteButtonCssSelector));
            deleteButtonForFoundProduct.Click();

            return new BasketPage(this.Driver);
        }

        public BasketPage CountItemsInBasket(out int count)
        {
            count = GetAllProductElementsInBasket().Count();

            return new BasketPage(this.Driver);
        }

        public bool IsEmptyBasketMessageDisplayed()
        {
            var expectedMessage = "Cosul de cumparaturi este gol";
            var message = this.Driver.FindElement(By.CssSelector(EmptyBasketMessageCssSelector)).Text;
            if (message == expectedMessage)
            {
                return true;
            }
            return false;
        }
    }
}
