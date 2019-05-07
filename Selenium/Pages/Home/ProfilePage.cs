﻿using Configuration;
using OpenQA.Selenium;
using Selenium.Pages.Outcome;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Pages
{
    public class ProfilePage : BasePage
    {
        public ProfilePage(IWebDriver driver) : base(driver) { }

        [FindsBy]
        private IWebElement microchipNumber = null, issuer=null, animalAltered=null, btnSave=null,
             btnClose=null, NewOutcome=null;

        //[FindsBy(How = How.CssSelector, Using = "span[data-target='#collapseHold']")]
        //private IWebElement animalCurrentHolds = null;

        [FindsBy(How = How.XPath, Using = "//form[@name='animalHold']/div[6]/div/div[1]/button")]
        private IWebElement releaseHoldBtn = null;

        [FindsBy(How = How.XPath, Using = "(//button[@id='ViewHoldHistory'])[1]")]
        private IWebElement ViewHoldHistory = null;

        [FindsBy(How = How.XPath, Using = "//form[@id='viewAnimalProfileForm']//div[@id='animalDetail']/div/label/span/span")]
        private IWebElement animalDetails = null;

        [FindsBy(How = How.XPath, Using = "//form[@id='viewAnimalProfileForm']//div[@id='currenthold']/div/label/span/span")]
        private IWebElement animalCurrentHolds = null;


        public void EnterMicroChipDetails()
        {
            Parameter.Add<string>("MicroChipNumber",FakeData.Number(1,10000));
            microchipNumber.SendKeysWrapper(Parameter.Get<string>("MicroChipNumber"),driver);
            issuer.SelectByIndex(driver);            
        }

        public void EnterAnimalDetails()
        {
            Sleep(500);
            driver.ScrollPage(0, 1000);
            Sleep(500);
            actions.MoveToElement(animalDetails).Click().Build().Perform();
            Sleep(1000);
            if (FindBy(By.XPath("(//div[7]//*[@id='headingAction']/label/span)[1][@class='expandable collapsed']"), 2, true) != null)
                animalDetails.ClickCustom(driver);

            animalAltered.SelectByIndex(driver, 2);
            btnSave.ClickCustom(driver);
        }

        public void ReleaseHolds()
        {            
            Sleep(500);
            ScreenBusy();
            driver.ScrollPage(0, 1000);
            Sleep(500);
            Wait(ExpectedConditions.PresenceOfAllElementsLocatedBy(animalCurrentHolds.GetLocator()),10);
            bool state = true;
            while (state)
            {
                try
                {
                    driver.FindElement(animalCurrentHolds.GetLocator()).Click();
                    state = false;
                }
                catch(Exception e) { state = true; }
            }
            //actions.MoveToElement(FindBy(animalCurrentHolds.GetLocator())).Click().Build().Perform();
            Sleep(500);
            if (FindBy(By.XPath("(//div[6]//*[@id='headingAction']/label/span)[1][@class='expandable ng-scope collapsed']"), 1, true) != null)
                animalCurrentHolds.ClickCustom(driver);

            ViewHoldHistory.ClickCustom(driver);
            for(int i = 0; i < 20; i++)
            {
                try
                {
                    Sleep(500);
                    FindBy(By.XPath("(//tbody/tr/td/a)[1]")).ClickCustom(driver);
                    releaseHoldBtn.ClickCustom(driver);
                }
                catch { break; }
            }
            btnClose.ClickCustom(driver);
        }

        public NewOutComePage ClickNewOutcome()
        {
            Sleep(1000);
            NewOutcome.ClickCustom(driver);
            return new NewOutComePage(driver);
        }
    }
}
