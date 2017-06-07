using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IVRPOC
{

    public enum Options
    {
        CreateAccount,
        [Terms(new string[] { "savings balance", "Savings Account Balance" })]
        [Describe("Savings Account Balance")]
        SavingsAccountBalance,
        [Terms(new string[] { "current balance", "Current Account Balance" })]
        [Describe("Current Account Balance")]
        CurrentAccountBalance,
        [Terms(new string[] { "creditcard payment", "CreditCard Payment" })]
        [Describe("CreditCard Payment")]
        CreditCardPayment,
        [Terms(new string[] { "delete", "delete an account" })]
        [Describe("Delete an account")]
        DeleteAccount,
    };
    public enum CreateAccountOptions {
        SavingsAccount,
        CurrentAccount
    };
    public enum Confirmation
    {
        Yes,
        [Terms(new string[] { "no", "no" })]
        [Describe("No")]
        No
    }

    [Serializable]
    class Balance
    {    
        [Prompt("Please enter your account number")]
        public string AccountNumber;
        [Prompt("Please enter your pin")]
        public string PIN;
        public string Availablebalance;
    };

    [Serializable]
    class Currentbalance
    {
        [Prompt("Please enter your account number")]
        public string AccountNumber;
        [Prompt("Please enter your pin")]
        public string PIN;
        public string CurrentAvailablebalance;

    };

    [Serializable]
    class CreditCardPayment
    {
        [Prompt("Please enter your creditcard number")]
        public string CreditcardNumber;
        [Prompt("Please enter how much amount do you want to pay")]
        public string Pay;
        public string CreditCardPaymentSuccessMessage;
    };

    [Serializable]
    class DeleteAccount
    {
        [Prompt("Are you sure want to delete your account?")]
        public string DeleteConfirmationMessage;
        public string DeleteSuccessMessage;
    };
    [Serializable]
    class Customer
    {
        //Create Account Template
        [Prompt("Please send any of these commands like **IVR** (or) **ivr**.")]   
        public string StartingWord;
        public Options? Option;
        public CreateAccountOptions? AccountType;
        [Prompt("Please enter your {&}")]
        public string FullName;
        [Prompt("Please enter your {&} like "+
             "* CustomerType, DOB, Nationality, Mother's Name, Applicant's Martial Status*")]    
        public string PersonalDetails;
        [Prompt("Please enter your {&} like "+
            "* LandMark, District, State, City, PIN, Mobile Number, Email Address*")]
        public string CorrespondenceAddress;
        [Prompt("Please enter your {&} like " +
             "* LandMark, District, State, City, PIN, Mobile Number, Email Address*")]
        public string PermanentAddress;
        public string SocialSecurityNumber;
        [Prompt("Please enter your {&} like * Name, Account Number * ")]
        public string NomineeDetails;    
        [Prompt("Please enter the amount like how much do you want to deposit in your account?")]
        public string SavingsAmount;
        [Prompt("Do you want to create account with the above details?")]
        public string confirmation;
        //[Template(TemplateUsage.EnumSelectOne, "Do you want create account with the above details?{||}", ChoiceStyle = ChoiceStyleOptions.Default)]
        //public Confirmation? confirmation;
        //[Prompt("I'm sorry. I didn't understand you. Please type **back **, if you can edit your details or type **yes** you can commit your details.")]
        //public string No;   



        //Savings Account Balance Template 
        public Balance Savings_Balance;

        //Current Account Balance Template
        public Currentbalance Current_Balance;

        //Delete account Template
        public DeleteAccount Delete;
        [Template(TemplateUsage.EnumSelectOne, "Please select your {&} {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
        public string AccountNumber;

        //CreditCard Payment Template
        public CreditCardPayment CreditCard_Payment;


        public static IForm<Customer> BuildForm()
        {
            OnCompletionAsyncDelegate<Customer> accountStatus = async (context, state) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                await context.PostAsync("We are currently processing your account details. We will message you the status.");
               
            };
            var builder = new FormBuilder<Customer>();

            ActiveDelegate<Customer> isCreate = (customer) => customer.Option == Options.CreateAccount;
            ActiveDelegate<Customer> isBalance = (customer) => customer.Option == Options.SavingsAccountBalance;
            ActiveDelegate<Customer> isCurrentBalance = (customer) => customer.Option == Options.CurrentAccountBalance;
            ActiveDelegate<Customer> isCreditCardPayment = (customer) => customer.Option == Options.CreditCardPayment;
            ActiveDelegate<Customer> isDelete = (customer) => customer.Option == Options.DeleteAccount;
            //ActiveDelegate<Customer> isConfirmation = (customer) => customer.confirmation == Confirmation.No;
            return builder
                       //.Message("Welcome to the BankIVR bot! To start an conversation with this bot send **ivr** or **IVR** command.\r \n if you need help, send the **Help** command")
                       //.Message("Welcome to the BankIVR bot!"+ "(Hi)")
                       .Field(nameof(Customer.StartingWord), validate: async (state, response) =>
                       {
                           var result = new ValidateResult { IsValid = true, Value = response };
                           string str = (response as string);
                           if ("ivr".Equals(str, StringComparison.InvariantCultureIgnoreCase))
                           {
                               result.IsValid = true;

                               //return result;

                           }
                           else
                           {
                               result.Feedback = "I'm sorry. I didn't understand you.";
                               result.IsValid = false;
                               //return result;
                           }
                           return result;
                       })

                        .Field(nameof(Customer.Option))
                        .Field("Savings_Balance.AccountNumber", isBalance, validate: async (state, response) =>
                        {
                            var result = new ValidateResult { IsValid = true, Value = response };
                            string accountnumber = (response as string);
                            int accountnumberlength = accountnumber.Length;
                            if (accountnumberlength <11|| accountnumberlength >17)
                            {
                                result.Feedback = "Please enter your valid savings account number";
                                result.IsValid = false;
                            }

                            return result;
                        })
                        .Field("Savings_Balance.PIN", isBalance)
                        .Field(new FieldReflector<Customer>("Savings_Balance.Availablebalance")
                        .SetType(null)
                        .SetActive((state) => state.Option == Options.SavingsAccountBalance)
                        .SetDefine(async (state, field) =>
                               {
                                   if (state.Savings_Balance != null)
                                   {
                                       if (state.Savings_Balance.AccountNumber != null && state.Savings_Balance.PIN != null)
                                       {
                                           string availableBalance = SQLDatabaseService.checkingAccountBalance(state.Savings_Balance.AccountNumber, state.Savings_Balance.PIN);
                                           if (availableBalance != null && availableBalance != "")
                                           {  
                                                                                            
                                               field.SetPrompt(new PromptAttribute($"Total available savings account balance is ${availableBalance:F2}"));                                          
                                               return true;
                                           }
                                           else
                                           {                                             
                                               return false;
                                           }
                                       }
                                       else
                                       {
                                           field.SetPrompt(new PromptAttribute($"I'm sorry. I didn't understand you."));
                                           return true;
                                       }
                                   }
                                   else
                                   {
                                       field.SetPrompt(new PromptAttribute($"I'm sorry. I didn't understand you."));
                                       return true;
                                   }
                                   
                               }))

                         .Field("Current_Balance.AccountNumber", isCurrentBalance, validate: async (state, response) =>
                               {
                                  var result = new ValidateResult { IsValid = true, Value = response };
                                  string accountnumber = (response as string);
                                  int accountnumberlength = accountnumber.Length;
                                  if (accountnumberlength < 11 || accountnumberlength > 17)
                                    {
                                       result.Feedback = "Please enter your valid current account number";
                                        result.IsValid = false;
                                    }
                           return result;
                         })
                        .Field("Current_Balance.PIN", isCurrentBalance)
                        .Field(new FieldReflector<Customer>("Current_Balance.CurrentAvailablebalance")
                        .SetType(null)
                        .SetActive((state) => state.Option == Options.CurrentAccountBalance)
                        .SetDefine(async (state, field) =>
                        {
                            if (state.Current_Balance != null)
                            {
                                if (state.Current_Balance.AccountNumber != null && state.Current_Balance.PIN != null)
                                {
                                    string availableBalance = SQLDatabaseService.checkingCurrentAccountBalance(state.Current_Balance.AccountNumber, state.Current_Balance.PIN);
                                    if (availableBalance != null && availableBalance != "")
                                    {
                                        field.SetPrompt(new PromptAttribute($"Total available current account balance is ${availableBalance:F2}"));
                                        return true;
                                    }
                                    else
                                    {                                      
                                        return false;
                                    }
                                }
                                else
                                {
                                    field.SetPrompt(new PromptAttribute($"I'm sorry. I didn't understand you."));
                                    return true;
                                }
                            }
                            else
                            {
                                field.SetPrompt(new PromptAttribute($"I'm sorry. I didn't understand you."));
                                return true;
                            }

                        }))      
                        .Field("CreditCard_Payment.CreditcardNumber", isCreditCardPayment,
                        validate: async (state, response) =>
                        {
                            var result = new ValidateResult { IsValid = true, Value = response };
                            string creditnumber = (response as string);
                            if (creditnumber.Length.ToString() != "16")
                            {
                                result.Feedback = "Please enter your valid credit card number";
                                result.IsValid = false;
                            }

                            return result;
                        })
                        .Field("CreditCard_Payment.Pay", isCreditCardPayment)
                        .Field(new FieldReflector<Customer>("CreditCard_Payment.CreditCardPaymentSuccessMessage")
                        .SetType(null)
                        .SetActive((state) => state.Option == Options.CreditCardPayment)
                        .SetDefine(async (state, field) =>
                        {
                            field.SetPrompt(new PromptAttribute($"Successfully paid your credit card payment."+"(Yes)"));
                            return true;
                        }))
                        .Field(nameof(Customer.AccountType))
                        .Field(new FieldReflector<Customer>(nameof(Customer.AccountNumber))
                             .SetType(null)
                             .SetActive((state) => state.Option == Options.DeleteAccount)
                             .SetDefine(async (state, field) =>
                             {
                                 if (state.AccountType != null)
                                 {
                                     MessagesController.accountnumlist = SQLDatabaseService.getAccountNumbers(state.AccountType);
                               
                                     if (MessagesController.accountnumlist != null && MessagesController.accountnumlist.Count() > 0)
                                     {
                                         foreach (var account in MessagesController.accountnumlist)
                                         {

                                             field

                                                  .AddDescription(account.AccountNumber.ToString(), account.AccountNumber.ToString())

                                                  .AddTerms(account.AccountNumber.ToString(), account.AccountNumber.ToString(), account.AccountNumber.ToString());

                                         }
                                         return true;
                                     }
                                     else
                                     {
                                         field.SetPrompt(new PromptAttribute($"I'm sorry. I didn't understand you."));
                                         return false;
                                     }
                                 }
                                 else
                                 {
                                     return true;
                                 }
                           }))                        
                         .Field("Delete.DeleteConfirmationMessage", isDelete)                    
                         .Field(new FieldReflector<Customer>("Delete.DeleteSuccessMessage")
                            .SetType(null)
                            .SetActive((state) => state.Option == Options.DeleteAccount)
                            .SetDefine(async (state, field) =>
                            {
                                if (state.Delete != null)
                                {
                                    if (state.AccountNumber != null && state.Delete.DeleteConfirmationMessage.ToLower() == "yes")
                                    {
                                        bool result = SQLDatabaseService.DeleteAccountNumber(state.AccountNumber);
                                        if (result == true)
                                        {
                                            field.SetPrompt(new PromptAttribute($"Successfully deleted your account."));
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        field.SetPrompt(new PromptAttribute($"I'm sorry. I didn't understand you."));
                                        return true;
                                    }
                                }
                                else
                                {
                                    return true;
                                }
                            }))
                        .Field(nameof(Customer.FullName))
                        .Field(nameof(Customer.PersonalDetails))
                        .Field(nameof(Customer.CorrespondenceAddress))
                        .Field(nameof(Customer.PermanentAddress))
                        .Field(nameof(Customer.SocialSecurityNumber))
                        .Field(nameof(Customer.NomineeDetails))
                        .Field(nameof(Customer.SavingsAmount))
                        .Message("**These are  your account details: ** {AccountType} {FullName} {PersonalDetails} {CorrespondenceAddress} {PermanentAddress} {SocialSecurityNumber} {NomineeDetails} {SavingsAmount}", isCreate)
                         .Field(nameof(Customer.confirmation),//),
                             validate: async (state, response) =>
                              {
                                  var result = new ValidateResult { IsValid = true, Value = response };
                                  var userselection = (response as string).Trim();
                                  if (userselection.ToString().ToLower() == "no")
                                  {
                                      result.Feedback = "I'm sorry. I didn't understand you. Please type **back**, if you can edit your details or type **yes** you can commit your details.";
                                      result.IsValid = false;
                                  }
                                  return result;
                              })
                        //.Field(nameof(confirmation))
                        //.Field("Back", isConfirmation)
                        .OnCompletion(accountStatus)                   
                        .Build();
        }
    };


}

