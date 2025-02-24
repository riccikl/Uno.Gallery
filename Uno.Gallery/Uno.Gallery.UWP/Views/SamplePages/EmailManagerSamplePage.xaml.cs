﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Uno.Gallery.ViewModels;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Uno.Gallery.Views.Samples
{
	[SamplePage(SampleCategory.NonUIFeatures, "Email Manager",
		Description = "Sends mail through device's mail app or fall backs to mailto: URI.",
		DocumentationLink = "https://platform.uno/docs/articles/features/windows-applicationmodel-email.html",
		DataType = typeof(EmailManagerSamplePageViewModel))]
	public sealed partial class EmailManagerSamplePage : Page
	{
		public EmailManagerSamplePage()
		{
			this.InitializeComponent();
		}
	}

	public class EmailManagerSamplePageViewModel : ViewModelBase
	{
		// EmailManager only supports the ShowComposeNewEmailInternalAsync, with the properties: To (only addresses, whitout names), CC (only addresses, whitout names), BCC (only addresses, whitout names), Subject, Body
		public string To { get => GetProperty<string>(); set => SetProperty(value); }
		public string CC { get => GetProperty<string>(); set => SetProperty(value); }
		public string BCC { get => GetProperty<string>(); set => SetProperty(value); }
		public string Subject { get => GetProperty<string>(); set => SetProperty(value); }
		public string Body { get => GetProperty<string>(); set => SetProperty(value); }
		public ICommand ComposeCommand => new Command(Compose);

		private async void Compose()
		{
			try
			{
				// Subject and Body null/empty checked here in order to show a controlled message instead of generic "Value cannot be null" error.
				if (string.IsNullOrWhiteSpace(Subject)) throw new Exception("The subject cannot be empty.");
				if (string.IsNullOrWhiteSpace(Body)) throw new Exception("The body cannot be empty.");

				var email = new EmailMessage
				{
					Subject = Subject,
					Body = Body
				};
				AddCSVToEmailRecipients(To, email.To);
				AddCSVToEmailRecipients(CC, email.CC);
				AddCSVToEmailRecipients(BCC, email.Bcc);

				await EmailManager.ShowComposeNewEmailAsync(email);
			}
			catch(Exception ex)
			{
				ContentDialog errorDialog = new ContentDialog
				{
					Title = "Error",
					Content = "Error trying to fire show compose new email: " + Environment.NewLine + ex.Message,
					CloseButtonText = "OK"
				};
				await errorDialog.ShowAsync();
			}
		}

		private void AddCSVToEmailRecipients(string csvAddresses, IList<EmailRecipient> target)
		{
			// Not all platforms in all the projects supports the StringSplitOptions.TrimEntries, so we trim each item afterwards and filter to ensure not empty addresses
			var addresses = csvAddresses?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(address => address.Trim()).Where(address => !string.IsNullOrWhiteSpace(address)).ToList();
			// Add each address to target list
			addresses?.ForEach(address =>
			{
				target.Add(new EmailRecipient(address));
			});
		}
	}
}
