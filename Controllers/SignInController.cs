using AmazonPay;
using AmazonPay.Responses;
using AmazonPay.StandardPaymentRequests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BancHubAmzon.Controllers
{
    public class SignInController : Controller
    {
        string access_token = "";
        private static AmazonPay.CommonRequests.Configuration clientConfig = null;
        private static Client client = null;
        private static Dictionary<string, string> apiResponse = new Dictionary<string, string>();
      
        // GET: SignIn
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult amazonPayLogin()
        {
            return View();
        }
        
        public ActionResult AddressAndWallet()
        {
            access_token = Request.QueryString["access_token"];

            clientConfig = new AmazonPay.CommonRequests.Configuration();

            clientConfig.WithAccessKey(ConfigurationManager.AppSettings["AccessKey"].ToString())
                .WithSecretKey(ConfigurationManager.AppSettings["SecretKey"].ToString())
                .WithMerchantId(ConfigurationManager.AppSettings["MerchantId"].ToString())
                .WithClientId(ConfigurationManager.AppSettings["ClientId"].ToString())
                .WithSandbox(true)
                .WithRegion(Regions.supportedRegions.us)
                .WithCurrencyCode(Regions.currencyCode.USD);
            client = new Client(clientConfig);
            Session.Add("ClientObject", client);
            return View();
        }
        [HttpPost]
        public ActionResult GetSetOrderReference(string amazonOrderReferenceId, string amount, string addressConsentToken = "")
        {

            SetOrderReferenceDetailsRequest setRequestParameters = new SetOrderReferenceDetailsRequest();
            setRequestParameters.WithAmazonOrderReferenceId(amazonOrderReferenceId)
                .WithAmount(decimal.Parse(amount))
                .WithCurrencyCode(Regions.currencyCode.USD)
                .WithSellerNote("Note");

            OrderReferenceDetailsResponse setResponse = client.SetOrderReferenceDetails(setRequestParameters);
            if (!setResponse.GetSuccess())
            {
                apiResponse["setOrderReferenceDetailsResponse"] = "SetOrderReferenceDetails API call Failed" + Environment.NewLine + setResponse.GetJson();
            }
            else
            {
                apiResponse["setOrderReferenceDetailsResponse"] = setResponse.GetJson();
            };
            System.Web.HttpContext.Current.Session.Add("amazonOrderReferenceId", amazonOrderReferenceId);
            System.Web.HttpContext.Current.Session.Add("amount", amount);

            return Json(apiResponse);
        }
        public static void SetOrderReference(string amazonOrderReferenceId, string amount)
        {
            SetOrderReferenceDetailsRequest setRequestParameters = new SetOrderReferenceDetailsRequest();
            setRequestParameters.WithAmazonOrderReferenceId(amazonOrderReferenceId)
                .WithAmount(decimal.Parse(amount))
                .WithCurrencyCode(Regions.currencyCode.USD)
                .WithSellerNote("Note");

            OrderReferenceDetailsResponse setResponse = client.SetOrderReferenceDetails(setRequestParameters);

            if (!setResponse.GetSuccess())
            {
                apiResponse["setOrderReferenceDetailsResponse"] = "SetOrderReferenceDetails API call Failed" + Environment.NewLine + setResponse.GetJson();
            }
            else
            {
                apiResponse["setOrderReferenceDetailsResponse"] = setResponse.GetJson();
            }
        }
        
        [HttpPost]
        public ActionResult ConfirmOrder()
        {
            ConfirmOrderReferenceRequest getRequestParameters = new ConfirmOrderReferenceRequest();
            getRequestParameters.WithAmazonOrderReferenceId(Session["amazonOrderReferenceId"].ToString());
            var amazonOrderReferenceId = Session["amazonOrderReferenceId"].ToString();

            ConfirmOrderReferenceResponse confirmOrderReferenceResponse = client.ConfirmOrderReference(getRequestParameters);

            if (!confirmOrderReferenceResponse.GetSuccess())
            {
                apiResponse["confirmOrderReferenceResponse"] = "SetOrderReferenceDetails API call Failed" + Environment.NewLine + confirmOrderReferenceResponse.GetJson();
            }
            else
            {
                apiResponse["confirmOrderReferenceResponse"] = confirmOrderReferenceResponse.GetJson();
            }
            return Json(apiResponse);
        }
       

    }
}