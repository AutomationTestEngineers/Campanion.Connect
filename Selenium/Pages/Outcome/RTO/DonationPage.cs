﻿using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Pages.Outcome
{
    public class DonationPage : BasePage
    {
        public DonationPage(IWebDriver driver) : base(driver) { }

        [FindsBy]
        private IWebElement inputvalue = null, nextButton = null;

        
        public PaymentPage EnterDonation()
        {
            inputvalue.ClearAndPaste("10", driver);
            ClickWithLoop(nextButton.GetLocator());
            return new PaymentPage(driver);
        }

    }
}
