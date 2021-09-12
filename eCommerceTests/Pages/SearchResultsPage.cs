using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace eCommerceTests.Pages
{
    public class SearchResultsPage : SitePage
    {
        private const string ProductCssSelector = ".product-item-info";
        private const string ProductTitleCssSelector = ".product-item-name";
        private const string AddToBasketCssSelector = ".product-addtocart-button";
        private const string AddToBasketSuccessModalCssSelector = ".add-to-cart-popup";
        private const string CloseSuccessModalCssSelector = ".action-close";
        private const string ProductsInBasketCountCssSelector = ".shopping-cart .qty-mc";
        private const string OverlayCssSelector = ".modals-overlay";

        public SearchResultsPage(IWebDriver driver) : base(driver)
        {
        }

        public SearchResultsPage AddRandomProductToBasket(out bool success, out string addedItem)
        {
            var allProducts = this.Driver.FindElements(By.CssSelector(ProductCssSelector));
            var productCount = allProducts.Count();

            if (productCount == 0)
            {
                throw new Exception("No products displayed on the page");
            }

            var rand = new Random().Next(productCount);
            var selectedProduct = allProducts.ElementAt(rand);
            this.AddProductToBasket(selectedProduct, out success);

            addedItem = selectedProduct.FindElement(By.CssSelector(ProductTitleCssSelector)).Text;
            return new SearchResultsPage(this.Driver);
        }


        public IList<IWebElement> GetAllProductElements()
        {
            var allProducts = this.Driver.FindElements(By.CssSelector(ProductCssSelector));
            return allProducts;
        }


        public void AddProductToBasket(IWebElement product, out bool success, bool dismissModal = true)
        {
            success = false;
            var initialProductCount = -1;
            var wait = new WebDriverWait(this.Driver, TimeSpan.FromSeconds(10));


            var productsInBasketCountElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(ProductsInBasketCountCssSelector)));

            while (productsInBasketCountElement.Text == null)
            {
                // I wouldn't usually use Thread.Sleep, but I will for the purpose of advancing in this exercise in a timely fashion
                System.Threading.Thread.Sleep(1000); 
            }
            initialProductCount = int.Parse(productsInBasketCountElement.Text);

            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(OverlayCssSelector)));
            System.Threading.Thread.Sleep(1000);

            // Following line can throw an exception. Takes too much to debug
            // Please run the tests again if it fails with ElementClickInterceptedException
            product.FindElement(By.CssSelector(AddToBasketCssSelector)).Click();


            var modal = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(AddToBasketSuccessModalCssSelector)));

            if (dismissModal)
            {
                modal.FindElement(By.CssSelector(CloseSuccessModalCssSelector)).Click();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(AddToBasketSuccessModalCssSelector)));
            }


            var expectedProductCount = initialProductCount + 1;
            wait.Until(ExpectedConditions.TextToBePresentInElement(productsInBasketCountElement, expectedProductCount.ToString()));

            success = true;
        }

        public SearchResultsPage CountItemsInBasket(out int count)
        {
            var wait = new WebDriverWait(this.Driver, TimeSpan.FromSeconds(10));
            var text = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(ProductsInBasketCountCssSelector))).Text;
            count = int.Parse(ProductsInBasketCount.Text);
            return new SearchResultsPage(this.Driver);
        }
    }
}
