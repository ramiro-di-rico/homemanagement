﻿using Autofac;
using HomeManagement.App.Behaviours;
using HomeManagement.App.Common;
using HomeManagement.App.Data.Entities;
using HomeManagement.App.Managers;
using HomeManagement.App.ViewModels;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeManagement.App.Views.Transactions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddTransactionPage : ContentPage
	{
        private Account account;
        private AddTransactionViewModel viewModel;
        ILocalizationManager localizationManager = App._container.Resolve<ILocalizationManager>();

        private AddTransactionPage()
        {
            InitializeComponent();

            Title = localizationManager.Translate("NewMovement");
        }

        public AddTransactionPage(Account account) : this()
        {
            this.account = account;

            viewModel = new AddTransactionViewModel(account);

            BindingContext = viewModel;

            viewModel.OnAdded += OnTransactionAdded;

            viewModel.OnCancel += OnCancel;

            viewModel.OnError += ViewModel_OnError;

            viewModel.OnInitializationFinished += ViewModel_OnInitializationFinished;
        }

        private void ViewModel_OnInitializationFinished(object sender, System.EventArgs e)
        {
            var autoCompleteBehavior = TransactionNameEntry.Behaviors.Last() as AutoCompleteBehavior;
            autoCompleteBehavior.Suggestions = viewModel.TransactionsNamesSuggestions.ToList();
            TransactionNameEntry.Focus();
        }

        private void ViewModel_OnError(object sender, System.EventArgs e)
        {
            DisplayAlert("Error", "Algunos de los datos ingresados no son validos", "cancel");
        }

        private void OnCancel(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnTransactionAdded(object sender, System.EventArgs e)
        {
            MessagingCenter.Send(this, Constants.Messages.UpdateOnAppearing);
            Navigation.PopAsync();
        }

        private void TransactionNameEntry_Completed(object sender, System.EventArgs e)
        {
            PriceEntry.Focus();
        }
    }
}