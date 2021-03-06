﻿using System;
using LibationWinForms.Dialogs.Login;

namespace LibationWinForms.Login
{
	public class WinformResponder : AudibleApi.ILoginCallback
	{
		public string Get2faCode()
		{
			using var dialog = new _2faCodeDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return dialog.Code;
			return null;
		}

		public string GetCaptchaAnswer(byte[] captchaImage)
		{
			using var dialog = new CaptchaDialog(captchaImage);
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return dialog.Answer;
			return null;
		}

		public (string email, string password) GetLogin()
		{
			using var dialog = new AudibleLoginDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return (dialog.Email, dialog.Password);
			return (null, null);
		}
	}
}